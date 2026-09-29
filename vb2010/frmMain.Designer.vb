<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim ChartArea3 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend3 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series5 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series6 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.Comm = New System.IO.Ports.SerialPort(Me.components)
        Me.tmrControl = New System.Windows.Forms.Timer(Me.components)
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.inputQLimit = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtRecord = New System.Windows.Forms.TextBox()
        Me.btnStartStop = New System.Windows.Forms.Button()
        Me.txtQStatus = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.txtIStatus = New System.Windows.Forms.Label()
        Me.txtVStatus = New System.Windows.Forms.Label()
        Me.txtCommIndicate = New System.Windows.Forms.Label()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.cboComSelect = New System.Windows.Forms.ComboBox()
        Me.btnApply = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.inputILimit = New System.Windows.Forms.TextBox()
        Me.inputVLimit = New System.Windows.Forms.TextBox()
        Me.Chart1 = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.tmrDisplay = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.Chart1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Comm
        '
        Me.Comm.ReadTimeout = 1000
        Me.Comm.WriteTimeout = 1000
        '
        'tmrControl
        '
        Me.tmrControl.Enabled = True
        Me.tmrControl.Interval = 200
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.GroupBox1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtRecord)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnStartStop)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtQStatus)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableLayoutPanel1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCommIndicate)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnConnect)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboComSelect)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.Chart1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1299, 740)
        Me.SplitContainer1.SplitterDistance = 264
        Me.SplitContainer1.TabIndex = 11
        '
        'inputQLimit
        '
        Me.inputQLimit.Location = New System.Drawing.Point(180, 88)
        Me.inputQLimit.Name = "inputQLimit"
        Me.inputQLimit.Size = New System.Drawing.Size(53, 25)
        Me.inputQLimit.TabIndex = 23
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(17, 91)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(159, 15)
        Me.Label1.TabIndex = 22
        Me.Label1.Text = "Charge Limit (mAh):"
        '
        'txtRecord
        '
        Me.txtRecord.HideSelection = False
        Me.txtRecord.Location = New System.Drawing.Point(10, 411)
        Me.txtRecord.Multiline = True
        Me.txtRecord.Name = "txtRecord"
        Me.txtRecord.ReadOnly = True
        Me.txtRecord.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtRecord.Size = New System.Drawing.Size(246, 317)
        Me.txtRecord.TabIndex = 21
        '
        'btnStartStop
        '
        Me.btnStartStop.Location = New System.Drawing.Point(137, 172)
        Me.btnStartStop.Name = "btnStartStop"
        Me.btnStartStop.Size = New System.Drawing.Size(107, 41)
        Me.btnStartStop.TabIndex = 20
        Me.btnStartStop.Text = "Start"
        Me.btnStartStop.UseVisualStyleBackColor = True
        '
        'txtQStatus
        '
        Me.txtQStatus.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.txtQStatus.Location = New System.Drawing.Point(3, 178)
        Me.txtQStatus.Name = "txtQStatus"
        Me.txtQStatus.Size = New System.Drawing.Size(128, 28)
        Me.txtQStatus.TabIndex = 19
        Me.txtQStatus.Text = "Stopped"
        Me.txtQStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.12664!))
        Me.TableLayoutPanel1.Controls.Add(Me.txtIStatus, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.txtVStatus, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(264, 112)
        Me.TableLayoutPanel1.TabIndex = 18
        '
        'txtIStatus
        '
        Me.txtIStatus.AutoSize = True
        Me.txtIStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtIStatus.Font = New System.Drawing.Font("Arial", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.txtIStatus.Location = New System.Drawing.Point(3, 52)
        Me.txtIStatus.Name = "txtIStatus"
        Me.txtIStatus.Size = New System.Drawing.Size(258, 60)
        Me.txtIStatus.TabIndex = 3
        Me.txtIStatus.Text = "--"
        Me.txtIStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtVStatus
        '
        Me.txtVStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtVStatus.Font = New System.Drawing.Font("Arial", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.txtVStatus.Location = New System.Drawing.Point(3, 0)
        Me.txtVStatus.Name = "txtVStatus"
        Me.txtVStatus.Size = New System.Drawing.Size(258, 52)
        Me.txtVStatus.TabIndex = 2
        Me.txtVStatus.Text = "--"
        Me.txtVStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtCommIndicate
        '
        Me.txtCommIndicate.AutoSize = True
        Me.txtCommIndicate.Location = New System.Drawing.Point(118, 131)
        Me.txtCommIndicate.Name = "txtCommIndicate"
        Me.txtCommIndicate.Size = New System.Drawing.Size(0, 15)
        Me.txtCommIndicate.TabIndex = 17
        '
        'btnConnect
        '
        Me.btnConnect.Location = New System.Drawing.Point(137, 118)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(107, 41)
        Me.btnConnect.TabIndex = 16
        Me.btnConnect.Text = "Open Port"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'cboComSelect
        '
        Me.cboComSelect.FormattingEnabled = True
        Me.cboComSelect.Location = New System.Drawing.Point(20, 128)
        Me.cboComSelect.Name = "cboComSelect"
        Me.cboComSelect.Size = New System.Drawing.Size(92, 23)
        Me.cboComSelect.TabIndex = 15
        '
        'btnApply
        '
        Me.btnApply.Location = New System.Drawing.Point(67, 119)
        Me.btnApply.Name = "btnApply"
        Me.btnApply.Size = New System.Drawing.Size(107, 41)
        Me.btnApply.TabIndex = 14
        Me.btnApply.Text = "Apply"
        Me.btnApply.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(25, 60)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(151, 15)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "Voltage Limit (V):"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(1, 29)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(175, 15)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "Current Setting (mA):"
        '
        'inputILimit
        '
        Me.inputILimit.Location = New System.Drawing.Point(180, 26)
        Me.inputILimit.Name = "inputILimit"
        Me.inputILimit.Size = New System.Drawing.Size(53, 25)
        Me.inputILimit.TabIndex = 11
        '
        'inputVLimit
        '
        Me.inputVLimit.Location = New System.Drawing.Point(180, 57)
        Me.inputVLimit.Name = "inputVLimit"
        Me.inputVLimit.Size = New System.Drawing.Size(53, 25)
        Me.inputVLimit.TabIndex = 10
        '
        'Chart1
        '
        Me.Chart1.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.None
        ChartArea3.Name = "ChartArea1"
        Me.Chart1.ChartAreas.Add(ChartArea3)
        Me.Chart1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Chart1.IsSoftShadows = False
        Legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom
        Legend3.Enabled = False
        Legend3.Name = "Legend1"
        Me.Chart1.Legends.Add(Legend3)
        Me.Chart1.Location = New System.Drawing.Point(0, 0)
        Me.Chart1.Name = "Chart1"
        Series5.ChartArea = "ChartArea1"
        Series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine
        Series5.Legend = "Legend1"
        Series5.Name = "SeriesV"
        Series6.ChartArea = "ChartArea1"
        Series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine
        Series6.Legend = "Legend1"
        Series6.Name = "SeriesI"
        Me.Chart1.Series.Add(Series5)
        Me.Chart1.Series.Add(Series6)
        Me.Chart1.Size = New System.Drawing.Size(1031, 740)
        Me.Chart1.TabIndex = 11
        Me.Chart1.Text = "Chart1"
        '
        'tmrDisplay
        '
        Me.tmrDisplay.Enabled = True
        Me.tmrDisplay.Interval = 1000
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.inputQLimit)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.btnApply)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.inputILimit)
        Me.GroupBox1.Controls.Add(Me.inputVLimit)
        Me.GroupBox1.Location = New System.Drawing.Point(11, 236)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(244, 169)
        Me.GroupBox1.TabIndex = 24
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Parameters"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1299, 740)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "frmMain"
        Me.Text = "Small battery charge control"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.Chart1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Comm As System.IO.Ports.SerialPort
    Friend WithEvents tmrControl As System.Windows.Forms.Timer
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents txtVStatus As System.Windows.Forms.Label
    Friend WithEvents txtCommIndicate As System.Windows.Forms.Label
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents cboComSelect As System.Windows.Forms.ComboBox
    Friend WithEvents btnApply As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents inputILimit As System.Windows.Forms.TextBox
    Friend WithEvents inputVLimit As System.Windows.Forms.TextBox
    Friend WithEvents Chart1 As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents txtIStatus As System.Windows.Forms.Label
    Friend WithEvents txtRecord As System.Windows.Forms.TextBox
    Friend WithEvents btnStartStop As System.Windows.Forms.Button
    Friend WithEvents txtQStatus As System.Windows.Forms.Label
    Friend WithEvents inputQLimit As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tmrDisplay As System.Windows.Forms.Timer
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox

End Class
