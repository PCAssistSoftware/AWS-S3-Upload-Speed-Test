<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    Friend WithEvents grpCredentials As GroupBox
    Friend WithEvents lblAccessKey As Label
    Friend WithEvents txtAccessKey As TextBox
    Friend WithEvents lblSecretKey As Label
    Friend WithEvents txtSecretKey As TextBox
    Friend WithEvents chkShowSecret As CheckBox
    Friend WithEvents lblRegion As Label
    Friend WithEvents cboRegion As ComboBox
    Friend WithEvents lblBucket As Label
    Friend WithEvents txtBucket As TextBox

    Friend WithEvents grpTest As GroupBox
    Friend WithEvents lblFileSize As Label
    Friend WithEvents cboFileSize As ComboBox
    Friend WithEvents lblKeyPrefix As Label
    Friend WithEvents txtKeyPrefix As TextBox
    Friend WithEvents chkDeleteAfter As CheckBox

    Friend WithEvents grpSmallFiles As GroupBox
    Friend WithEvents lblFileCount As Label
    Friend WithEvents numFileCount As NumericUpDown
    Friend WithEvents lblMinSize As Label
    Friend WithEvents numMinSizeKB As NumericUpDown
    Friend WithEvents lblMaxSize As Label
    Friend WithEvents numMaxSizeKB As NumericUpDown

    Friend WithEvents btnStart As Button
    Friend WithEvents btnRunSeries As Button
    Friend WithEvents btnRunSmallFiles As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents progressBar1 As ProgressBar
    Friend WithEvents lblStatus As Label
    Friend WithEvents txtLog As TextBox
    Friend WithEvents btnClearLog As Button

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.grpCredentials = New GroupBox()
        Me.lblAccessKey = New Label()
        Me.txtAccessKey = New TextBox()
        Me.lblSecretKey = New Label()
        Me.txtSecretKey = New TextBox()
        Me.chkShowSecret = New CheckBox()
        Me.lblRegion = New Label()
        Me.cboRegion = New ComboBox()
        Me.lblBucket = New Label()
        Me.txtBucket = New TextBox()

        Me.grpTest = New GroupBox()
        Me.lblFileSize = New Label()
        Me.cboFileSize = New ComboBox()
        Me.lblKeyPrefix = New Label()
        Me.txtKeyPrefix = New TextBox()
        Me.chkDeleteAfter = New CheckBox()

        Me.grpSmallFiles = New GroupBox()
        Me.lblFileCount = New Label()
        Me.numFileCount = New NumericUpDown()
        Me.lblMinSize = New Label()
        Me.numMinSizeKB = New NumericUpDown()
        Me.lblMaxSize = New Label()
        Me.numMaxSizeKB = New NumericUpDown()

        Me.btnStart = New Button()
        Me.btnRunSeries = New Button()
        Me.btnRunSmallFiles = New Button()
        Me.btnCancel = New Button()
        Me.progressBar1 = New ProgressBar()
        Me.lblStatus = New Label()
        Me.txtLog = New TextBox()
        Me.btnClearLog = New Button()

        Me.SuspendLayout()

        ' grpCredentials
        Me.grpCredentials.Location = New Point(12, 12)
        Me.grpCredentials.Size = New Size(620, 150)
        Me.grpCredentials.Text = "AWS Credentials"

        Me.lblAccessKey.Location = New Point(15, 28)
        Me.lblAccessKey.Size = New Size(120, 20)
        Me.lblAccessKey.Text = "Access Key ID:"

        Me.txtAccessKey.Location = New Point(145, 25)
        Me.txtAccessKey.Size = New Size(415, 23)

        Me.lblSecretKey.Location = New Point(15, 58)
        Me.lblSecretKey.Size = New Size(120, 20)
        Me.lblSecretKey.Text = "Secret Access Key:"

        Me.txtSecretKey.Location = New Point(145, 55)
        Me.txtSecretKey.Size = New Size(345, 23)
        Me.txtSecretKey.PasswordChar = "*"c

        Me.chkShowSecret.Location = New Point(500, 57)
        Me.chkShowSecret.Size = New Size(60, 20)
        Me.chkShowSecret.Text = "Show"

        Me.lblRegion.Location = New Point(15, 88)
        Me.lblRegion.Size = New Size(120, 20)
        Me.lblRegion.Text = "Region:"

        Me.cboRegion.Location = New Point(145, 85)
        Me.cboRegion.Size = New Size(200, 23)
        Me.cboRegion.DropDownStyle = ComboBoxStyle.DropDownList

        Me.lblBucket.Location = New Point(15, 118)
        Me.lblBucket.Size = New Size(120, 20)
        Me.lblBucket.Text = "Bucket Name:"

        Me.txtBucket.Location = New Point(145, 115)
        Me.txtBucket.Size = New Size(415, 23)

        Me.grpCredentials.Controls.Add(Me.lblAccessKey)
        Me.grpCredentials.Controls.Add(Me.txtAccessKey)
        Me.grpCredentials.Controls.Add(Me.lblSecretKey)
        Me.grpCredentials.Controls.Add(Me.txtSecretKey)
        Me.grpCredentials.Controls.Add(Me.chkShowSecret)
        Me.grpCredentials.Controls.Add(Me.lblRegion)
        Me.grpCredentials.Controls.Add(Me.cboRegion)
        Me.grpCredentials.Controls.Add(Me.lblBucket)
        Me.grpCredentials.Controls.Add(Me.txtBucket)

        ' grpTest
        Me.grpTest.Location = New Point(12, 172)
        Me.grpTest.Size = New Size(620, 110)
        Me.grpTest.Text = "Test Settings"

        Me.lblFileSize.Location = New Point(15, 28)
        Me.lblFileSize.Size = New Size(120, 20)
        Me.lblFileSize.Text = "File Size:"

        Me.cboFileSize.Location = New Point(145, 25)
        Me.cboFileSize.Size = New Size(150, 23)
        Me.cboFileSize.DropDownStyle = ComboBoxStyle.DropDownList

        Me.lblKeyPrefix.Location = New Point(15, 58)
        Me.lblKeyPrefix.Size = New Size(120, 20)
        Me.lblKeyPrefix.Text = "Key Prefix:"

        Me.txtKeyPrefix.Location = New Point(145, 55)
        Me.txtKeyPrefix.Size = New Size(300, 23)

        Me.chkDeleteAfter.Location = New Point(145, 85)
        Me.chkDeleteAfter.Size = New Size(400, 20)
        Me.chkDeleteAfter.Text = "Delete test object from bucket after upload"
        Me.chkDeleteAfter.Checked = True

        Me.grpTest.Controls.Add(Me.lblFileSize)
        Me.grpTest.Controls.Add(Me.cboFileSize)
        Me.grpTest.Controls.Add(Me.lblKeyPrefix)
        Me.grpTest.Controls.Add(Me.txtKeyPrefix)
        Me.grpTest.Controls.Add(Me.chkDeleteAfter)

        ' grpSmallFiles
        Me.grpSmallFiles.Location = New Point(12, 292)
        Me.grpSmallFiles.Size = New Size(620, 80)
        Me.grpSmallFiles.Text = "Small Files Test Settings"

        Me.lblFileCount.Location = New Point(15, 28)
        Me.lblFileCount.Size = New Size(90, 20)
        Me.lblFileCount.Text = "File Count:"

        Me.numFileCount.Location = New Point(110, 25)
        Me.numFileCount.Size = New Size(80, 23)
        Me.numFileCount.Minimum = 1
        Me.numFileCount.Maximum = 2000
        Me.numFileCount.Value = 100

        Me.lblMinSize.Location = New Point(210, 28)
        Me.lblMinSize.Size = New Size(100, 20)
        Me.lblMinSize.Text = "Min Size (KB):"

        Me.numMinSizeKB.Location = New Point(315, 25)
        Me.numMinSizeKB.Size = New Size(70, 23)
        Me.numMinSizeKB.Minimum = 1
        Me.numMinSizeKB.Maximum = 102400
        Me.numMinSizeKB.Value = 100

        Me.lblMaxSize.Location = New Point(400, 28)
        Me.lblMaxSize.Size = New Size(100, 20)
        Me.lblMaxSize.Text = "Max Size (KB):"

        Me.numMaxSizeKB.Location = New Point(505, 25)
        Me.numMaxSizeKB.Size = New Size(70, 23)
        Me.numMaxSizeKB.Minimum = 1
        Me.numMaxSizeKB.Maximum = 102400
        Me.numMaxSizeKB.Value = 2048

        Me.grpSmallFiles.Controls.Add(Me.lblFileCount)
        Me.grpSmallFiles.Controls.Add(Me.numFileCount)
        Me.grpSmallFiles.Controls.Add(Me.lblMinSize)
        Me.grpSmallFiles.Controls.Add(Me.numMinSizeKB)
        Me.grpSmallFiles.Controls.Add(Me.lblMaxSize)
        Me.grpSmallFiles.Controls.Add(Me.numMaxSizeKB)

        ' buttons / progress / status
        Me.btnStart.Location = New Point(12, 382)
        Me.btnStart.MinimumSize = New Size(140, 30)
        Me.btnStart.AutoSize = True
        Me.btnStart.AutoSizeMode = AutoSizeMode.GrowOnly
        Me.btnStart.Text = "Start Upload Test"

        Me.btnRunSeries.Location = New Point(162, 382)
        Me.btnRunSeries.MinimumSize = New Size(170, 30)
        Me.btnRunSeries.AutoSize = True
        Me.btnRunSeries.AutoSizeMode = AutoSizeMode.GrowOnly
        Me.btnRunSeries.Text = "Run Size Series Test"

        Me.btnRunSmallFiles.Location = New Point(342, 382)
        Me.btnRunSmallFiles.MinimumSize = New Size(170, 30)
        Me.btnRunSmallFiles.AutoSize = True
        Me.btnRunSmallFiles.AutoSizeMode = AutoSizeMode.GrowOnly
        Me.btnRunSmallFiles.Text = "Run Small Files Test"

        Me.btnCancel.Location = New Point(522, 382)
        Me.btnCancel.MinimumSize = New Size(90, 30)
        Me.btnCancel.AutoSize = True
        Me.btnCancel.AutoSizeMode = AutoSizeMode.GrowOnly
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.Enabled = False

        Me.progressBar1.Location = New Point(12, 422)
        Me.progressBar1.Size = New Size(620, 23)

        Me.lblStatus.Location = New Point(12, 452)
        Me.lblStatus.Size = New Size(620, 20)
        Me.lblStatus.Text = "Ready."

        Me.txtLog.Location = New Point(12, 478)
        Me.txtLog.Size = New Size(620, 260)
        Me.txtLog.Multiline = True
        Me.txtLog.ReadOnly = True
        Me.txtLog.ScrollBars = ScrollBars.Vertical
        Me.txtLog.Font = New Font("Consolas", 9)

        Me.btnClearLog.Location = New Point(552, 744)
        Me.btnClearLog.Size = New Size(80, 25)
        Me.btnClearLog.Text = "Clear Log"

        ' Form1
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(644, 780)
        Me.Controls.Add(Me.grpCredentials)
        Me.Controls.Add(Me.grpTest)
        Me.Controls.Add(Me.grpSmallFiles)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.btnRunSeries)
        Me.Controls.Add(Me.btnRunSmallFiles)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.progressBar1)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.btnClearLog)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "AWS S3 Upload Speed Test"

        Me.ResumeLayout(False)
    End Sub

End Class
