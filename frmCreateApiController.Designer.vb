<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCreateApiController
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtControllerName = New System.Windows.Forms.TextBox()
        Me.txtModels = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.chkList = New System.Windows.Forms.CheckBox()
        Me.chkView = New System.Windows.Forms.CheckBox()
        Me.chkInsert = New System.Windows.Forms.CheckBox()
        Me.chkUpdate = New System.Windows.Forms.CheckBox()
        Me.chkUpsert = New System.Windows.Forms.CheckBox()
        Me.btnGenerate = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.chkFind = New System.Windows.Forms.CheckBox()
        Me.chkDelete = New System.Windows.Forms.CheckBox()
        Me.cboRoute = New System.Windows.Forms.ComboBox()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(58, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Controller:"
        '
        'txtControllerName
        '
        Me.txtControllerName.Location = New System.Drawing.Point(70, 6)
        Me.txtControllerName.Name = "txtControllerName"
        Me.txtControllerName.Size = New System.Drawing.Size(204, 21)
        Me.txtControllerName.TabIndex = 1
        Me.txtControllerName.Text = "Product"
        '
        'txtModels
        '
        Me.txtModels.Location = New System.Drawing.Point(70, 60)
        Me.txtModels.Multiline = True
        Me.txtModels.Name = "txtModels"
        Me.txtModels.Size = New System.Drawing.Size(204, 131)
        Me.txtModels.TabIndex = 3
        Me.txtModels.Text = "Products" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Brands"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 63)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(44, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Models:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(24, 36)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(40, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Route:"
        '
        'chkList
        '
        Me.chkList.AutoSize = True
        Me.chkList.Checked = True
        Me.chkList.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkList.Location = New System.Drawing.Point(280, 37)
        Me.chkList.Name = "chkList"
        Me.chkList.Size = New System.Drawing.Size(42, 17)
        Me.chkList.TabIndex = 4
        Me.chkList.Text = "List"
        Me.chkList.UseVisualStyleBackColor = True
        '
        'chkView
        '
        Me.chkView.AutoSize = True
        Me.chkView.Checked = True
        Me.chkView.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkView.Location = New System.Drawing.Point(280, 60)
        Me.chkView.Name = "chkView"
        Me.chkView.Size = New System.Drawing.Size(84, 17)
        Me.chkView.TabIndex = 5
        Me.chkView.Text = "View (By Id)"
        Me.chkView.UseVisualStyleBackColor = True
        '
        'chkInsert
        '
        Me.chkInsert.AutoSize = True
        Me.chkInsert.Location = New System.Drawing.Point(280, 105)
        Me.chkInsert.Name = "chkInsert"
        Me.chkInsert.Size = New System.Drawing.Size(94, 17)
        Me.chkInsert.TabIndex = 7
        Me.chkInsert.Text = "Insert (Model)"
        Me.chkInsert.UseVisualStyleBackColor = True
        '
        'chkUpdate
        '
        Me.chkUpdate.AutoSize = True
        Me.chkUpdate.Location = New System.Drawing.Point(280, 128)
        Me.chkUpdate.Name = "chkUpdate"
        Me.chkUpdate.Size = New System.Drawing.Size(100, 17)
        Me.chkUpdate.TabIndex = 8
        Me.chkUpdate.Text = "Update (Model)"
        Me.chkUpdate.UseVisualStyleBackColor = True
        '
        'chkUpsert
        '
        Me.chkUpsert.AutoSize = True
        Me.chkUpsert.Checked = True
        Me.chkUpsert.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkUpsert.Location = New System.Drawing.Point(280, 151)
        Me.chkUpsert.Name = "chkUpsert"
        Me.chkUpsert.Size = New System.Drawing.Size(97, 17)
        Me.chkUpsert.TabIndex = 9
        Me.chkUpsert.Text = "Upsert (Model)"
        Me.chkUpsert.UseVisualStyleBackColor = True
        '
        'btnGenerate
        '
        Me.btnGenerate.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGenerate.ForeColor = System.Drawing.Color.DarkRed
        Me.btnGenerate.Location = New System.Drawing.Point(356, 258)
        Me.btnGenerate.Name = "btnGenerate"
        Me.btnGenerate.Size = New System.Drawing.Size(111, 35)
        Me.btnGenerate.TabIndex = 11
        Me.btnGenerate.Text = "Generate"
        Me.btnGenerate.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Location = New System.Drawing.Point(12, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(459, 240)
        Me.TabControl1.TabIndex = 11
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.chkFind)
        Me.TabPage1.Controls.Add(Me.chkDelete)
        Me.TabPage1.Controls.Add(Me.cboRoute)
        Me.TabPage1.Controls.Add(Me.txtControllerName)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.chkUpsert)
        Me.TabPage1.Controls.Add(Me.txtModels)
        Me.TabPage1.Controls.Add(Me.chkUpdate)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.chkInsert)
        Me.TabPage1.Controls.Add(Me.chkList)
        Me.TabPage1.Controls.Add(Me.chkView)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(451, 214)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "General"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'chkFind
        '
        Me.chkFind.AutoSize = True
        Me.chkFind.Location = New System.Drawing.Point(280, 83)
        Me.chkFind.Name = "chkFind"
        Me.chkFind.Size = New System.Drawing.Size(99, 17)
        Me.chkFind.TabIndex = 6
        Me.chkFind.Text = "Find (By string)"
        Me.chkFind.UseVisualStyleBackColor = True
        '
        'chkDelete
        '
        Me.chkDelete.AutoSize = True
        Me.chkDelete.Location = New System.Drawing.Point(280, 174)
        Me.chkDelete.Name = "chkDelete"
        Me.chkDelete.Size = New System.Drawing.Size(93, 17)
        Me.chkDelete.TabIndex = 10
        Me.chkDelete.Text = "Delete (By Id)"
        Me.chkDelete.UseVisualStyleBackColor = True
        '
        'cboRoute
        '
        Me.cboRoute.FormattingEnabled = True
        Me.cboRoute.Items.AddRange(New Object() {"api", "api/[controller]", "---- ", "api/[controller]/[action]"})
        Me.cboRoute.Location = New System.Drawing.Point(70, 33)
        Me.cboRoute.Name = "cboRoute"
        Me.cboRoute.Size = New System.Drawing.Size(204, 21)
        Me.cboRoute.TabIndex = 2
        Me.cboRoute.Text = "api"
        '
        'frmCreateApiController
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(483, 308)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.btnGenerate)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmCreateApiController"
        Me.Text = "CREATE CONTROLLER FOR API"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents txtControllerName As TextBox
    Friend WithEvents txtModels As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents chkList As CheckBox
    Friend WithEvents chkView As CheckBox
    Friend WithEvents chkInsert As CheckBox
    Friend WithEvents chkUpdate As CheckBox
    Friend WithEvents chkUpsert As CheckBox
    Friend WithEvents btnGenerate As Button
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents cboRoute As ComboBox
    Friend WithEvents chkDelete As CheckBox
    Friend WithEvents chkFind As CheckBox
End Class
