<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSQLBulkCopy
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSQLBulkCopy))
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.txtSource = New System.Windows.Forms.ToolStripComboBox()
        Me.btnEditSrc = New System.Windows.Forms.ToolStripButton()
        Me.btnBuildCNSource = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripLabel2 = New System.Windows.Forms.ToolStripLabel()
        Me.txtDest = New System.Windows.Forms.ToolStripComboBox()
        Me.btnEditDest = New System.Windows.Forms.ToolStripButton()
        Me.btnBuildCNDest = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnConnect = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnExportCSV = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.optIdentityInsertOff = New System.Windows.Forms.ToolStripMenuItem()
        Me.optCheckedOnly = New System.Windows.Forms.ToolStripMenuItem()
        Me.optTruncateTable = New System.Windows.Forms.ToolStripMenuItem()
        Me.optSkipCompleted = New System.Windows.Forms.ToolStripMenuItem()
        Me.optLimitExport = New System.Windows.Forms.ToolStripMenuItem()
        Me.optIgnoreErrors = New System.Windows.Forms.ToolStripMenuItem()
        Me.optCommandTimeout = New System.Windows.Forms.ToolStripTextBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.btnExecute = New System.Windows.Forms.Button()
        Me.txtSQL = New System.Windows.Forms.RichTextBox()
        Me.dtGrid = New System.Windows.Forms.DataGridView()
        Me.txtSchema = New System.Windows.Forms.RichTextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.dtGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.txtSource, Me.btnEditSrc, Me.btnBuildCNSource, Me.ToolStripSeparator4, Me.ToolStripLabel2, Me.txtDest, Me.btnEditDest, Me.btnBuildCNDest, Me.ToolStripSeparator5, Me.btnConnect, Me.ToolStripSeparator1, Me.btnExportCSV, Me.ToolStripSeparator2, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(961, 25)
        Me.ToolStrip1.TabIndex = 8
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(45, 22)
        Me.ToolStripLabel1.Text = "source:"
        '
        'txtSource
        '
        Me.txtSource.Name = "txtSource"
        Me.txtSource.Size = New System.Drawing.Size(250, 25)
        Me.txtSource.Text = "Provider=SQLOLEDB.1;Password=sa;Persist Security Info=True;User ID=sa;Initial Cat" &
    "alog=ebs;Data Source=192.168.3.240"
        Me.txtSource.ToolTipText = "database folder or file"
        '
        'btnEditSrc
        '
        Me.btnEditSrc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnEditSrc.Image = CType(resources.GetObject("btnEditSrc.Image"), System.Drawing.Image)
        Me.btnEditSrc.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnEditSrc.Name = "btnEditSrc"
        Me.btnEditSrc.Size = New System.Drawing.Size(23, 22)
        Me.btnEditSrc.Text = "Show Connectionstring"
        '
        'btnBuildCNSource
        '
        Me.btnBuildCNSource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnBuildCNSource.Image = CType(resources.GetObject("btnBuildCNSource.Image"), System.Drawing.Image)
        Me.btnBuildCNSource.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnBuildCNSource.Name = "btnBuildCNSource"
        Me.btnBuildCNSource.Size = New System.Drawing.Size(23, 22)
        Me.btnBuildCNSource.Text = "Build connection string"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripLabel2
        '
        Me.ToolStripLabel2.Name = "ToolStripLabel2"
        Me.ToolStripLabel2.Size = New System.Drawing.Size(33, 22)
        Me.ToolStripLabel2.Text = "Dest:"
        '
        'txtDest
        '
        Me.txtDest.Name = "txtDest"
        Me.txtDest.Size = New System.Drawing.Size(250, 25)
        Me.txtDest.Text = "Provider=MSOLEDBSQL;Password=sa;Persist Security Info=True;User ID=sa;Initial Cat" &
    "alog=ebs;Data Source=DESKTOP-ABGBBCK\MSSQLSERVER01;Connection Timeout=720;Encryp" &
    "t=False;Packet Size=4096;"
        '
        'btnEditDest
        '
        Me.btnEditDest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnEditDest.Image = CType(resources.GetObject("btnEditDest.Image"), System.Drawing.Image)
        Me.btnEditDest.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnEditDest.Name = "btnEditDest"
        Me.btnEditDest.Size = New System.Drawing.Size(23, 22)
        Me.btnEditDest.Text = "Show Connectionstring"
        '
        'btnBuildCNDest
        '
        Me.btnBuildCNDest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnBuildCNDest.Image = CType(resources.GetObject("btnBuildCNDest.Image"), System.Drawing.Image)
        Me.btnBuildCNDest.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnBuildCNDest.Name = "btnBuildCNDest"
        Me.btnBuildCNDest.Size = New System.Drawing.Size(23, 22)
        Me.btnBuildCNDest.Text = "Build connection string"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(6, 25)
        '
        'btnConnect
        '
        Me.btnConnect.Image = CType(resources.GetObject("btnConnect.Image"), System.Drawing.Image)
        Me.btnConnect.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(72, 22)
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.ToolTipText = "Connect to selected database"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'btnExportCSV
        '
        Me.btnExportCSV.Image = CType(resources.GetObject("btnExportCSV.Image"), System.Drawing.Image)
        Me.btnExportCSV.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnExportCSV.Name = "btnExportCSV"
        Me.btnExportCSV.Size = New System.Drawing.Size(137, 22)
        Me.btnExportCSV.Text = "Export to Destination"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.optIdentityInsertOff, Me.optCheckedOnly, Me.optTruncateTable, Me.optSkipCompleted, Me.optLimitExport, Me.optIgnoreErrors, Me.optCommandTimeout})
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(29, 22)
        Me.ToolStripButton1.Text = "Configurations"
        '
        'optIdentityInsertOff
        '
        Me.optIdentityInsertOff.Checked = True
        Me.optIdentityInsertOff.CheckOnClick = True
        Me.optIdentityInsertOff.CheckState = System.Windows.Forms.CheckState.Checked
        Me.optIdentityInsertOff.Name = "optIdentityInsertOff"
        Me.optIdentityInsertOff.Size = New System.Drawing.Size(190, 22)
        Me.optIdentityInsertOff.Text = "IDENTITY_INSERT OFF"
        '
        'optCheckedOnly
        '
        Me.optCheckedOnly.Checked = True
        Me.optCheckedOnly.CheckOnClick = True
        Me.optCheckedOnly.CheckState = System.Windows.Forms.CheckState.Checked
        Me.optCheckedOnly.Name = "optCheckedOnly"
        Me.optCheckedOnly.Size = New System.Drawing.Size(190, 22)
        Me.optCheckedOnly.Text = "Checked Only"
        '
        'optTruncateTable
        '
        Me.optTruncateTable.CheckOnClick = True
        Me.optTruncateTable.Name = "optTruncateTable"
        Me.optTruncateTable.Size = New System.Drawing.Size(190, 22)
        Me.optTruncateTable.Text = "Truncate Table"
        Me.optTruncateTable.ToolTipText = "truncate table/remove all records from table"
        '
        'optSkipCompleted
        '
        Me.optSkipCompleted.Checked = True
        Me.optSkipCompleted.CheckOnClick = True
        Me.optSkipCompleted.CheckState = System.Windows.Forms.CheckState.Checked
        Me.optSkipCompleted.Name = "optSkipCompleted"
        Me.optSkipCompleted.Size = New System.Drawing.Size(190, 22)
        Me.optSkipCompleted.Text = "Skip Completed"
        '
        'optLimitExport
        '
        Me.optLimitExport.Checked = True
        Me.optLimitExport.CheckOnClick = True
        Me.optLimitExport.CheckState = System.Windows.Forms.CheckState.Checked
        Me.optLimitExport.Name = "optLimitExport"
        Me.optLimitExport.Size = New System.Drawing.Size(190, 22)
        Me.optLimitExport.Text = "Limit Export to 10000"
        '
        'optIgnoreErrors
        '
        Me.optIgnoreErrors.CheckOnClick = True
        Me.optIgnoreErrors.Name = "optIgnoreErrors"
        Me.optIgnoreErrors.Size = New System.Drawing.Size(190, 22)
        Me.optIgnoreErrors.Text = "Ignore Errors"
        '
        'optCommandTimeout
        '
        Me.optCommandTimeout.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.optCommandTimeout.Name = "optCommandTimeout"
        Me.optCommandTimeout.Size = New System.Drawing.Size(100, 23)
        Me.optCommandTimeout.Text = "90"
        Me.optCommandTimeout.ToolTipText = "Command Timeout"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtSchema)
        Me.SplitContainer1.Size = New System.Drawing.Size(961, 516)
        Me.SplitContainer1.SplitterDistance = 442
        Me.SplitContainer1.TabIndex = 9
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.btnExecute)
        Me.SplitContainer2.Panel1.Controls.Add(Me.txtSQL)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.dtGrid)
        Me.SplitContainer2.Size = New System.Drawing.Size(961, 442)
        Me.SplitContainer2.SplitterDistance = 166
        Me.SplitContainer2.TabIndex = 0
        '
        'btnExecute
        '
        Me.btnExecute.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExecute.Image = CType(resources.GetObject("btnExecute.Image"), System.Drawing.Image)
        Me.btnExecute.Location = New System.Drawing.Point(912, 128)
        Me.btnExecute.Name = "btnExecute"
        Me.btnExecute.Size = New System.Drawing.Size(36, 28)
        Me.btnExecute.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.btnExecute, "Execute to grid")
        Me.btnExecute.UseVisualStyleBackColor = True
        '
        'txtSQL
        '
        Me.txtSQL.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtSQL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtSQL.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSQL.Location = New System.Drawing.Point(0, 0)
        Me.txtSQL.Name = "txtSQL"
        Me.txtSQL.Size = New System.Drawing.Size(961, 166)
        Me.txtSQL.TabIndex = 4
        Me.txtSQL.Text = "select * from arstrxdtl where trxseqid in (" & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(9) & "select trxseqid from arstrxhdr" & Global.Microsoft.VisualBasic.ChrW(9) & "where" &
    " trxid='EB' AND billperiod in ('202508', '202509', '202510')" & Global.Microsoft.VisualBasic.ChrW(10) & ")"
        '
        'dtGrid
        '
        Me.dtGrid.AllowUserToAddRows = False
        Me.dtGrid.AllowUserToDeleteRows = False
        Me.dtGrid.BackgroundColor = System.Drawing.Color.SlateGray
        Me.dtGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dtGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dtGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dtGrid.Location = New System.Drawing.Point(0, 0)
        Me.dtGrid.Name = "dtGrid"
        Me.dtGrid.ReadOnly = True
        Me.dtGrid.Size = New System.Drawing.Size(961, 272)
        Me.dtGrid.TabIndex = 3
        '
        'txtSchema
        '
        Me.txtSchema.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtSchema.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtSchema.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSchema.Location = New System.Drawing.Point(0, 0)
        Me.txtSchema.Name = "txtSchema"
        Me.txtSchema.Size = New System.Drawing.Size(961, 70)
        Me.txtSchema.TabIndex = 4
        Me.txtSchema.Text = ""
        Me.txtSchema.WordWrap = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 541)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(961, 22)
        Me.StatusStrip1.TabIndex = 10
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lblStatus
        '
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(946, 17)
        Me.lblStatus.Spring = True
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmSQLBulkCopy
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(961, 563)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(977, 602)
        Me.Name = "frmSQLBulkCopy"
        Me.Text = "SQL Bulk Copy"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        CType(Me.dtGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents btnEditSrc As ToolStripButton
    Friend WithEvents btnBuildCNSource As ToolStripButton
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents ToolStripLabel2 As ToolStripLabel
    Friend WithEvents btnEditDest As ToolStripButton
    Friend WithEvents btnBuildCNDest As ToolStripButton
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents btnConnect As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripButton1 As ToolStripDropDownButton
    Friend WithEvents optIdentityInsertOff As ToolStripMenuItem
    Friend WithEvents optCheckedOnly As ToolStripMenuItem
    Friend WithEvents optTruncateTable As ToolStripMenuItem
    Friend WithEvents optSkipCompleted As ToolStripMenuItem
    Friend WithEvents optLimitExport As ToolStripMenuItem
    Friend WithEvents optIgnoreErrors As ToolStripMenuItem
    Friend WithEvents optCommandTimeout As ToolStripTextBox
    Friend WithEvents txtSource As ToolStripComboBox
    Friend WithEvents txtDest As ToolStripComboBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents btnExecute As Button
    Friend WithEvents txtSQL As RichTextBox
    Friend WithEvents dtGrid As DataGridView
    Friend WithEvents txtSchema As RichTextBox
    Friend WithEvents btnExportCSV As ToolStripButton
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents lblStatus As ToolStripStatusLabel
End Class
