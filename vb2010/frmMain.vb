Public Class frmMain
    'NOTE!!! Check this when changing circuit. When the voltage reading is
    'higher than this, the program will think the battery is disconnected.
    Const Min_Fake_Voltage As Integer = 4600

    Private Charging As Boolean = False
    Private OutputThousandth As Integer = 0
    Private LastStatus As Status = New Status
    Private Q_mAh As Double = 0

    Private ILimit_mA As Integer = 0
    Private VLimit_mV As Integer = 0
    Private QLimit_mAh As Integer = 0

    Private Sub Log(ByRef msg As String)
        txtRecord.Text += CStr(DateTime.Now) + " " + msg
        txtRecord.Text += vbCrLf + vbCrLf
        txtRecord.SelectionStart = txtRecord.Text.Length - 1
        txtRecord.ScrollToCaret()
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Initialize serial port selector.
        For Each port In My.Computer.Ports.SerialPortNames
            cboComSelect.Items.Add(port)
        Next
        If My.Computer.Ports.SerialPortNames.Count = 1 Then
            cboComSelect.SelectedIndex = 0
        Else
            cboComSelect.SelectedItem = My.Settings.CommPort
        End If

        'Load saved charging parameters.
        inputVLimit.Text = (CSng(My.Settings.VLimit) / 1000.0).ToString
        inputILimit.Text = My.Settings.ILimit.ToString
        inputQLimit.Text = My.Settings.QLimit.ToString
        btnApply_Click(sender, e)

        'Initialze the chart.
        Chart1.Series(0).Color = Color.Blue
        Chart1.Series(0).YAxisType = DataVisualization.Charting.AxisType.Primary
        Chart1.Series(1).Color = Color.Green
        Chart1.Series(1).YAxisType = DataVisualization.Charting.AxisType.Secondary
        With (Chart1.ChartAreas(0).AxisY)
            .Title = "mV"
            .TitleForeColor = Color.Blue
            .LineColor = Color.Blue
            .MajorGrid.LineColor = Color.LightGray
            .IsStartedFromZero = False
        End With
        With Chart1.ChartAreas(0).AxisY2
            .Title = "mA"
            .TitleForeColor = Color.Green
            .LineColor = Color.Green
            .MajorGrid.LineColor = Color.LightGray
        End With
        Chart1.ChartAreas(0).AxisX.MajorGrid.LineColor = Color.LightGray
    End Sub

    Private Sub btnStartStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartStop.Click
        If Charging = False Then
            If Comm.IsOpen() = False Then
                Return
            End If
            OutputThousandth = 0
            Q_mAh = 0
            Charging = True
            btnStartStop.Text = "Stop"
            Log("Started charging，initial voltage: " + CStr(LastStatus.mv_bat) + " mV")
        Else
            StopCharge()
        End If
    End Sub

    Private Sub btnConnect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        Dim port_name As String
        If cboComSelect.SelectedItem IsNot Nothing Then
            port_name = cboComSelect.SelectedItem.ToString
        ElseIf My.Computer.Ports.SerialPortNames.Contains(cboComSelect.Text.Trim) Then
            port_name = cboComSelect.Text.Trim
        Else
            Return
        End If

        If Comm.IsOpen = False Then
            If port_name = "" Then
                Return
            End If
            Comm.PortName = port_name
            Try
                Comm.Open()
                My.Settings.CommPort = port_name
                My.Settings.Save()
                btnConnect.Text = "Close Port"
                cboComSelect.Enabled = False
                Chart1.Series(0).Points.Clear()
                Chart1.Series(1).Points.Clear()
            Catch ex As Exception
                MsgBox(ex.ToString())
                Try
                    Comm.Close()
                Catch ex2 As Exception
                End Try
            End Try
        Else
            CloseComm()
        End If
    End Sub

    Private Sub CloseComm()
        Try
            StopCharge()
            Comm.Close()
            btnConnect.Text = "Open Port"
            cboComSelect.Items.Clear()
            For Each port In My.Computer.Ports.SerialPortNames
                cboComSelect.Items.Add(port)
            Next
            cboComSelect.Enabled = True
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
    End Sub

    Private Sub StopCharge()
        If Charging = False Then Return
        Dim prev_mv_bat As Integer = LastStatus.mv_bat
        OutputThousandth = 0
        If Comm.IsOpen() Then
            Try
                SendCmd(0)
            Catch ex As Exception
                Debug.Print(ex.ToString())
            End Try
        End If
        Charging = False
        btnStartStop.Text = "Start"
        If prev_mv_bat < Min_Fake_Voltage Then
            Log("Stopped charging, " + Q_mAh.ToString("F1") + " mAh charged, reached " + CStr(prev_mv_bat) + " mV before stopping")
        Else
            Log("Stopped charging because of disconnection, " + Q_mAh.ToString("F1") + " mAh charged")
        End If
        Beep() : Beep()
    End Sub

    Private Sub inputILimit_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles inputILimit.TextChanged
        inputILimit.BackColor = Color.LightYellow
    End Sub

    Private Sub inputVLimit_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles inputVLimit.TextChanged
        inputVLimit.BackColor = Color.LightYellow
    End Sub

    Private Sub inputQLimit_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles inputQLimit.TextChanged
        inputQLimit.BackColor = Color.LightYellow
    End Sub

    Private Sub btnApply_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApply.Click
        Dim new_vlimit, new_ilimit, new_qlimit As Single
        Try
            new_vlimit = CSng(inputVLimit.Text)
            new_ilimit = CSng(inputILimit.Text)
            new_qlimit = CSng(inputQLimit.Text)
        Catch ex As Exception
            MsgBox("Invalid input")
            Return
        End Try

        If new_vlimit < 0 Or new_vlimit > 4.3 Or new_ilimit > 1000 Or new_ilimit < 0 Or new_qlimit < 0 Then
            MsgBox("Invalid input")
            inputVLimit.Text = (CSng(VLimit_mV) / 1000.0).ToString()
            inputILimit.Text = ILimit_mA.ToString()
            inputQLimit.Text = QLimit_mAh.ToString()
        Else
            VLimit_mV = CInt(new_vlimit * 1000)
            ILimit_mA = CInt(new_ilimit)
            QLimit_mAh = CInt(new_qlimit)
            My.Settings.VLimit = VLimit_mV
            My.Settings.ILimit = ILimit_mA
            My.Settings.QLimit = QLimit_mAh
            My.Settings.Save()
        End If

        inputILimit.BackColor = Color.White
        inputVLimit.BackColor = Color.White
        inputQLimit.BackColor = Color.White
    End Sub

    'Refreshes the 3 status indicators and the chart on tick.
    Private Sub tmrDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDisplay.Tick
        If Comm.IsOpen = False Then
            txtVStatus.Text = "--"
            txtIStatus.Text = "--"
            txtQStatus.Text = "Stopped"
            Return
        End If

        '显示基本状态
        If LastStatus.mv_bat > Min_Fake_Voltage Then
            txtVStatus.Text = "Disconnected"
        Else
            txtVStatus.Text = (CSng(LastStatus.mv_bat) / 1000.0).ToString("F2") + " V"
        End If
        txtIStatus.Text = LastStatus.ma_current.ToString + " mA"

        '显示充电状态
        If Charging Then
            txtQStatus.Text = Q_mAh.ToString("F1") + " mAh"
        Else
            txtQStatus.Text = "Stopped"
        End If

        Chart1.Series(0).Points.AddY(LastStatus.mv_bat)
        Chart1.Series(1).Points.AddY(LastStatus.ma_current)
    End Sub

    Private Sub tmrControl_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrControl.Tick
        Static fail_cnt As Integer = 0
        Static last_tp As DateTime? = Nothing
        Static last_is_open As Boolean = False

        If Comm.IsOpen = False Then
            If last_is_open Then
                CloseComm() 'refresh port status indicator
            End If
        End If

        last_is_open = Comm.IsOpen
        If Comm.IsOpen = False Then
            Return
        End If

        If Charging = False Then
            OutputThousandth = 0
        End If

        Try
            'Sends the current output strength determined on the last tick,
            'receives voltage and current status and provide the refiltered
            'value for displaying; the control timer itself still uses the
            'values before refiltering.
            Dim st = SendCmd(OutputThousandth)
            fail_cnt = 0
            If LastStatus.mv_bat = 0 Or Math.Abs(st.mv_bat - LastStatus.mv_bat) > 10 Or _
            Math.Abs(st.ma_current - LastStatus.ma_current) > 10 Then
                LastStatus = st
            Else
                LastStatus.mv_bat = _
                    CInt(Math.Round((LastStatus.mv_bat * 80 + st.mv_bat * 20) / 100))
                LastStatus.ma_current = _
                    CInt(Math.Round((LastStatus.ma_current * 80 + st.ma_current * 20) / 100))
            End If
            Debug.Print("Send " + CStr(OutputThousandth))
            If txtCommIndicate.Text = "" Then
                txtCommIndicate.Text = "."
            Else
                txtCommIndicate.Text = ""
            End If

            If Charging = False Then
                Return
            End If

            'Maintains the charged mAh value.
            If last_tp.HasValue Then
                Dim time_passed As TimeSpan = DateTime.Now.Subtract(last_tp.Value)
                Q_mAh = Q_mAh + time_passed.TotalHours * CDbl(st.ma_current)
            End If
            last_tp = DateTime.Now

            'Adjusts the output strength for the next tick according to the returned status.
            If st.mv_bat > Min_Fake_Voltage Then
                If Charging Then
                    StopCharge()
                    MsgBox("Battery disconnected")
                End If
            ElseIf st.mv_bat > VLimit_mV Then
                If st.ma_current > 10 Then
                    Beep()
                End If
                If Charging Then
                    StopCharge()
                    MsgBox("Voltage limit reached")
                End If
            ElseIf Q_mAh > QLimit_mAh Then
                If Charging Then
                    StopCharge()
                    MsgBox("Charge limit reached")
                End If
            ElseIf st.ma_current > ILimit_mA Then
                If st.ma_current - ILimit_mA < 100 Then
                    OutputThousandth -= 1
                    If OutputThousandth < 0 Then
                        OutputThousandth = 0
                    End If
                Else
                    StopCharge()
                    MsgBox("Current control failure")
                End If
            ElseIf st.ma_current < ILimit_mA Then
                If OutputThousandth = 0 Then
                    ' NOTE!!! this is needed because of the firmware simply uses this output
                    ' value for PWM output duty cycle. Check this when changing circuit.
                    OutputThousandth = 600
                End If
                OutputThousandth += 1
                If OutputThousandth > 1000 Then
                    OutputThousandth = 1000
                End If
            End If
        Catch ex As Exception
            fail_cnt = fail_cnt + 1
            If fail_cnt > 3 Then
                Beep() : Beep()
                CloseComm()
                fail_cnt = 0
                Debug.Print(ex.ToString())
            End If
        End Try
    End Sub

    'Sends the output strength to the MCU, receives battery voltage and current status.
    Private Function SendCmd(ByVal output_thousandth As Integer) As Status
        If Comm.IsOpen = False Then
            Throw New IO.IOException("Serial port not connected")
        End If

        If output_thousandth < 0 Then
            output_thousandth = 0
        End If

        Dim buf(5) As Byte 'host packet size is 6 bytes
        buf(0) = &H58
        buf(1) = &HAA
        'Real data begins
        buf(4) = CByte(output_thousandth Mod 256)
        buf(5) = CByte(output_thousandth \ 256)
        'Real data ends
        Dim sum As Short = CShort(buf(4)) + CShort(buf(5))
        buf(2) = CByte(sum Mod 256)
        buf(3) = CByte(sum \ 256)

        Comm.DiscardInBuffer()
        Comm.Write(buf, 0, 6)

        Do
            If Comm.ReadByte() <> &H58 Then Continue Do
            If Comm.ReadByte() <> &HAA Then Continue Do

            Dim buf_receive(5) As Byte 'MCU packet size subtracts 2 bytes (0xAA58) is 6
            For i = 0 To 5
                buf_receive(i) = CByte(Comm.ReadByte())
            Next

            sum = 0
            For i = 2 To 5
                sum = sum + buf_receive(i)
            Next
            If CShort(buf_receive(0)) + 256 * CShort(buf_receive(1)) <> sum Then
                Throw New IO.IOException("Checksum error in received data")
            End If

            Dim st As Status
            st.mv_bat = CInt(buf_receive(2)) + 256 * CInt(buf_receive(3))
            st.ma_current = CInt(buf_receive(4)) + 256 * CInt(buf_receive(5))
            Return st
        Loop
    End Function
End Class

' Represents a response packet from MCU
Structure Status
    Dim mv_bat As Integer
    Dim ma_current As Integer
End Structure