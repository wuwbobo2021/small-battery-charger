#![no_std]
#![no_main]

use embassy_executor::Spawner;
use embassy_futures::join::join;
use embassy_stm32::gpio::{Level, Output, Speed};
use embassy_stm32::time::Hertz;
use embassy_stm32::usb::Driver;
use embassy_stm32::{Config, bind_interrupts, peripherals, usb};
use embassy_usb::Builder;
use embassy_usb::class::cdc_acm::{CdcAcmClass, State};

use defmt::info;
use defmt_rtt as _;

use static_cell::StaticCell;

mod charger;

bind_interrupts!(struct Irqs {
    USB_LP_CAN1_RX0 => usb::InterruptHandler<peripherals::USB>;
});

#[embassy_executor::main]
async fn main(_spawner: Spawner) {
    let mut config = Config::default();
    {
        use embassy_stm32::rcc::*;
        config.rcc.hse = Some(Hse {
            freq: Hertz(8_000_000),
            // Oscillator for bluepill, Bypass for nucleos.
            mode: HseMode::Oscillator,
        });
        config.rcc.pll = Some(Pll {
            src: PllSource::HSE,
            prediv: PllPreDiv::DIV1,
            mul: PllMul::MUL9,
        });
        config.rcc.sys = Sysclk::PLL1_P;
        config.rcc.ahb_pre = AHBPrescaler::DIV1;
        config.rcc.apb1_pre = APBPrescaler::DIV2;
        config.rcc.apb2_pre = APBPrescaler::DIV1;
        config.rcc.adc_pre = ADCPrescaler::DIV6; // ADCCLK 12MHz
    }
    let mut p = embassy_stm32::init(config);

    info!("Startup");

    {
        // BluePill board has a pull-up resistor on the D+ line.
        // Pull the D+ pin down to send a RESET condition to the USB bus.
        // This forced reset is needed only for development, without it host
        // will not reset your device when you upload new firmware.
        let _dp = Output::new(p.PA12.reborrow(), Level::Low, Speed::Low);
        // Timer::after_millis(10).await;
        for _ in 0..72 * 1000 * 10 {
            cortex_m::asm::nop();
        }
    }

    // Create the driver, from the HAL.
    let driver = Driver::new(p.USB, Irqs, p.PA12, p.PA11);

    // Create embassy-usb Config
    let config = embassy_usb::Config::new(0xc0de, 0xcafe);
    //config.max_packet_size_0 = 64;

    // Create embassy-usb DeviceBuilder using the driver and config.
    // It needs some buffers for building the descriptors.
    let mut config_descriptor = [0; 256];
    let mut bos_descriptor = [0; 256];
    let mut control_buf = [0; 7];

    let mut state = State::new();

    let mut builder = Builder::new(
        driver,
        config,
        &mut config_descriptor,
        &mut bos_descriptor,
        &mut [], // no msos descriptors
        &mut control_buf,
    );

    // Create classes on the builder.
    let class = CdcAcmClass::new(&mut builder, &mut state, 64);

    // Build the builder.
    let mut usb = builder.build();

    // Run the USB device.
    let usb_fut = usb.run();

    let (sender, receiver) = class.split();
    static REC_BUF: StaticCell<[u8; 128]> = StaticCell::new();
    let rec_buf = REC_BUF.init([0; 128]);
    let receiver = receiver.into_buffered(rec_buf);

    let charger = charger::Charger::new(p.PA1, p.PA3, p.PA5, p.PA8, p.PA9, p.TIM1, p.ADC1, p.IWDG);
    let mut charger_comm = charger::ChargerComm::new(charger, sender, receiver);

    info!("Initialized peripherals");

    let charger_comm_fut = async {
        loop {
            charger_comm.get_sender().wait_connection().await;
            info!("Connected");
            let res = charger_comm.run().await;
            if let Err(_e) = res {
                info!("COMM loop exited");
            }
        }
    };

    // Run everything concurrently.
    join(usb_fut, charger_comm_fut).await;
}

#[panic_handler]
fn panic(_info: &core::panic::PanicInfo) -> ! {
    info!("Panic!");
    loop {} // wait for the watchdog to reset the chip
}
