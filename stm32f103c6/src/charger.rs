use core::sync::atomic::{AtomicU16, Ordering};

use embassy_stm32::adc::{Adc, SampleTime, VrefInt};
use embassy_stm32::gpio::{AfioRemap, Level, OutputOpenDrain, OutputType, Speed};
use embassy_stm32::peripherals::{ADC1, IWDG, PA1, PA3, PA5, PA8, PA9, TIM1};
use embassy_stm32::time::Hertz;
use embassy_stm32::timer::low_level::CountingMode;
use embassy_stm32::timer::low_level::OutputPolarity::ActiveHigh;
use embassy_stm32::timer::simple_pwm::{PwmPin, PwmPinConfig, SimplePwm, SimplePwmChannel};
use embassy_stm32::wdg::IndependentWatchdog;
use embassy_stm32::{Peri, adc, bind_interrupts};
use embassy_sync::once_lock::OnceLock;
use heapless::Deque;

bind_interrupts!(struct Irqs {
    ADC1_2 => adc::InterruptHandler<ADC1>;
});

/// Collects peripherals for basic charge controlling.
pub(crate) struct Charger<'d> {
    adc: Adc<'d, ADC1>,
    vrefint: VrefInt,
    vdiv_5v: Peri<'d, PA1>,
    vdiv_vd: Peri<'d, PA3>,
    vsamp: Peri<'d, PA5>,
    switch: embassy_stm32::gpio::OutputOpenDrain<'d>,
    pwm_out: SimplePwmChannel<'d, TIM1>,
    watchdog: IndependentWatchdog<'d, IWDG>,
    once_unleash: OnceLock<()>,
}

impl Charger<'static> {
    /// Initialization, disabling the PWM output.
    pub fn new(
        pa1: Peri<'static, PA1>,
        pa3: Peri<'static, PA3>,
        pa5: Peri<'static, PA5>,
        pa8: Peri<'static, PA8>,
        pa9: Peri<'static, PA9>,
        tim1: Peri<'static, TIM1>,
        adc1: Peri<'static, ADC1>,
        iwdg: Peri<'static, IWDG>,
    ) -> Self {
        let mut switch = OutputOpenDrain::new(pa9, Level::Low, Speed::Medium);
        switch.set_low();

        let pwm_pin: PwmPin<'_, TIM1, embassy_stm32::timer::Ch1, AfioRemap<0>> =
            PwmPin::new_with_config(
                pa8,
                PwmPinConfig {
                    output_type: OutputType::OpenDrain,
                    speed: Speed::Medium,
                },
            );
        let mut pwm_out = SimplePwm::new(
            tim1,
            Some(pwm_pin),
            None,
            None,
            None,
            Hertz::hz(140_625),
            CountingMode::EdgeAlignedUp,
        )
        .split()
        .ch1;
        pwm_out.enable();
        pwm_out.set_polarity(ActiveHigh);
        pwm_out.set_duty_cycle_fully_off();

        let adc = Adc::new(adc1);
        let vrefint = adc.enable_vref();

        let watchdog = IndependentWatchdog::new(iwdg, 5_000_000);

        let charger = Self {
            adc,
            vrefint,
            vdiv_5v: pa1,
            vdiv_vd: pa3,
            vsamp: pa5,
            switch,
            pwm_out,
            watchdog,
            once_unleash: OnceLock::new(),
        };
        charger
    }
}

impl<'d> ChargerTrait for Charger<'d> {
    // Gets battery voltage by `V5V - VD`, not detecting circuit break here; unit: mV
    async fn get_mv_bat(&mut self) -> u16 {
        let mv_5v = read_mv(&mut self.adc, &mut self.vrefint, &mut self.vdiv_5v).await * 1000 / 461;
        // defmt::info!("5V: {}", mv_5v);

        let vd = read_mv(&mut self.adc, &mut self.vrefint, &mut self.vdiv_vd).await * 1000 / 594;
        // defmt::info!("VD: {}", vd);

        (mv_5v - vd) as _
    }

    // Charging current, unit: mA
    async fn get_ma_current(&mut self) -> u16 {
        let vsamp = read_mv(&mut self.adc, &mut self.vrefint, &mut self.vsamp).await;
        (vsamp.saturating_sub(2) * 3) as _
    }

    async fn set_output_thousandth(&mut self, thousandth: u16) {
        if thousandth == 0 {
            self.switch.set_low();
            self.pwm_out.set_duty_cycle_fully_off();
        } else {
            // Enables watchdog upon first received command.
            let _ = self.once_unleash.get_or_init(|| {
                self.watchdog.unleash();
                ()
            });
            let duty = self.pwm_out.max_duty_cycle() * thousandth as u32 / 1000;
            self.pwm_out.set_duty_cycle(duty);
            self.switch.set_high();
        }
        self.watchdog.pet(); // pet it when a command is received
    }
}

/// Soft filtering by averaging, using VREFINT reference; returning unit is mV.
async fn read_mv<'d, T: embassy_stm32::adc::Instance>(
    adc: &mut embassy_stm32::adc::Adc<'d, T>,
    vrefint: &mut VrefInt,
    channel: &mut impl embassy_stm32::adc::AdcChannel<T>,
) -> u32 {
    static VREFINT_SAMPLE: AtomicU16 = AtomicU16::new(0);
    static CNT_SINCE_UPDATE: AtomicU16 = AtomicU16::new(0);
    let vrefint_sample_prev = VREFINT_SAMPLE.load(Ordering::SeqCst);
    let vrefint_sample =
        if vrefint_sample_prev == 0 || CNT_SINCE_UPDATE.fetch_add(1, Ordering::SeqCst) > 16 {
            // defmt::info!("update vrefint");
            CNT_SINCE_UPDATE.store(0, Ordering::SeqCst);
            let vrefint_sample = over_sampling_read(adc, vrefint).await;
            VREFINT_SAMPLE.store(vrefint_sample, Ordering::SeqCst);
            vrefint_sample
        } else {
            vrefint_sample_prev
        };

    // From http://www.st.com/resource/en/datasheet/CD00161566.pdf
    // 5.3.4 Embedded reference voltage
    const VREFINT_MV: u32 = 1200; // mV

    let sample = over_sampling_read(adc, channel).await;
    u32::from(sample) * VREFINT_MV / u32::from(vrefint_sample)
}

/// Soft filtering by averaging; returning unit is the same as ADC raw value.
async fn over_sampling_read<'d, T: embassy_stm32::adc::Instance>(
    adc: &mut embassy_stm32::adc::Adc<'d, T>,
    channel: &mut impl embassy_stm32::adc::AdcChannel<T>,
) -> u16 {
    let mut sum = 0u32;
    for _ in 0..32 {
        let _ = adc.read(channel, SampleTime::CYCLES239_5).await;
    }
    for _ in 0..512 {
        sum += adc.read(channel, SampleTime::CYCLES239_5).await as u32;
    }
    (sum >> 9) as u16
}

/// Simple low-level charge protocol.
pub trait ChargerTrait {
    async fn get_mv_bat(&mut self) -> u16;
    async fn get_ma_current(&mut self) -> u16;
    async fn set_output_thousandth(&mut self, thousandth: u16);
}

// Serial protocol for the low-level trait ---------------------------------

const COMM_FRAME_HEAD: u16 = 0xaa58;
const COMM_FRAME_HEAD_LEN: usize = 4;
const CMD_LEN: usize = core::mem::size_of::<ChargerHostCmd>();
const RESP_LEN: usize = core::mem::size_of::<ChargerMcuReply>();

use zerocopy::byteorder::{LittleEndian, U16};

#[derive(zerocopy::FromBytes)]
#[repr(C)]
struct ChargerHostCmd {
    frm_head: U16<LittleEndian>,
    check_sum: U16<LittleEndian>,
    // Don't change the two items above.
    output_pecent: U16<LittleEndian>,
}

#[derive(zerocopy::IntoBytes, zerocopy::Immutable)]
#[repr(C)]
struct ChargerMcuReply {
    frm_head: U16<LittleEndian>,
    check_sum: U16<LittleEndian>,
    // Don't change the two items above.
    mv_bat: U16<LittleEndian>,
    ma_current: U16<LittleEndian>,
}

// MCU(executor) side impl of the simple serial protocol ---------------------------------

pub struct ChargerComm<C, S, R, E>
where
    C: ChargerTrait,
    S: embedded_io_async::Write<Error = E>,
    R: embedded_io_async::Read<Error = E>,
{
    charger: C,
    sender: S,
    receiver: R,
}

impl<C, S, R, E> ChargerComm<C, S, R, E>
where
    C: ChargerTrait,
    S: embedded_io_async::Write<Error = E>,
    R: embedded_io_async::Read<Error = E>,
{
    pub fn new(charger: C, sender: S, receiver: R) -> Self {
        Self {
            charger,
            sender,
            receiver,
        }
    }

    pub fn get_sender(&mut self) -> &mut S {
        &mut self.sender
    }

    pub async fn run(&mut self) -> Result<(), E> {
        let res = self.run_loop().await;
        self.charger.set_output_thousandth(0).await;
        res
    }

    async fn run_loop(&mut self) -> Result<(), E> {
        loop {
            // Receives the next host command.
            let cmd = self.get_next_cmd_with_retry().await?;

            // Executes it.
            self.charger
                .set_output_thousandth(cmd.output_pecent.get().min(1000))
                .await;

            // Checks status and prepare for the reply.
            let mut reply = ChargerMcuReply {
                frm_head: COMM_FRAME_HEAD.into(),
                check_sum: 0.into(),
                mv_bat: self.charger.get_mv_bat().await.into(),
                ma_current: self.charger.get_ma_current().await.into(),
            };

            // Converts the reply to byte array.
            use zerocopy::IntoBytes;
            let mut buf_reply = [0u8; RESP_LEN];
            reply.write_to(&mut buf_reply).unwrap();
            let sum: u16 = buf_reply[COMM_FRAME_HEAD_LEN..]
                .iter()
                .map(|i| *i as u16)
                .sum();
            reply.check_sum = sum.into();
            reply.write_to(&mut buf_reply).unwrap();

            // Sends the reply.
            self.sender.write_all(&buf_reply).await?;
            self.sender.flush().await?;
        }
    }

    // Receives the next host command with a limited retry loop.
    async fn get_next_cmd_with_retry(&mut self) -> Result<ChargerHostCmd, E> {
        let mut receive_err_cnt = 0;
        loop {
            match self.get_next_cmd().await {
                Ok(Some(cmd)) => break Ok(cmd),
                Ok(None) => continue,
                Err(e) => {
                    if receive_err_cnt > 5 {
                        return Err(e);
                    }
                    receive_err_cnt += 1;
                    continue;
                }
            }
        }
    }

    // Receives the next host command.
    async fn get_next_cmd(&mut self) -> Result<Option<ChargerHostCmd>, E> {
        let mut receive_buf: Deque<u8, CMD_LEN> = Deque::new();
        let mut tmp_buf = [0u8];
        loop {
            if self.receiver.read(&mut tmp_buf).await? == 0 {
                return Ok(None);
            }
            receive_buf.push_back(tmp_buf[0]).unwrap();
            if receive_buf.is_full() {
                if let Some(cmd) = Self::check_cmd_packet(receive_buf.make_contiguous()) {
                    return Ok(Some(cmd));
                } else {
                    receive_buf.pop_back();
                }
            }
        }
    }

    fn check_cmd_packet(buf: &[u8]) -> Option<ChargerHostCmd> {
        use zerocopy::FromBytes;
        let Ok(cmd) = ChargerHostCmd::read_from_bytes(buf) else {
            return None;
        };
        let sum = buf[COMM_FRAME_HEAD_LEN..].iter().map(|i| *i as u16).sum();
        if cmd.frm_head.get() != COMM_FRAME_HEAD {
            None
        } else if cmd.check_sum.get() != sum {
            None
        } else {
            Some(cmd)
        }
    }
}
