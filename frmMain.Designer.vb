<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripSplitButton()
        Me.APIGeneratorcjTemplateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton5 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ModelBuilderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DataAccessBuilderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ControllerBuilderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UIControllerBuilderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.DataAccessOnUIToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DataAccessOnControllerAPIToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton3 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.DefaultToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ModalPopupToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ModalPopup2TupleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.BlankModalToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HtmlTagHelpersOnlyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Select2ViewBagToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ModalPopupcjTemplateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.InFormDynamicTableToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabsGeneratorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.DynamicMultiInputToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripButton2 = New System.Windows.Forms.ToolStripSplitButton()
        Me.GETToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FormPOSTToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FormPOSTJSToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DatatableGETToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Select2AjaxToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BsSuggestToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton4 = New System.Windows.Forms.ToolStripButton()
        Me.txtSource = New System.Windows.Forms.RichTextBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.txtDest = New System.Windows.Forms.RichTextBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ToolStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton1, Me.ToolStripSeparator1, Me.ToolStripButton5, Me.ToolStripSeparator3, Me.ToolStripButton3, Me.ToolStripButton2, Me.ToolStripSeparator2, Me.ToolStripButton4})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(764, 25)
        Me.ToolStrip1.TabIndex = 0
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.APIGeneratorcjTemplateToolStripMenuItem})
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(176, 22)
        Me.ToolStripButton1.Text = "API Generator (Controller)"
        '
        'APIGeneratorcjTemplateToolStripMenuItem
        '
        Me.APIGeneratorcjTemplateToolStripMenuItem.Name = "APIGeneratorcjTemplateToolStripMenuItem"
        Me.APIGeneratorcjTemplateToolStripMenuItem.Size = New System.Drawing.Size(217, 22)
        Me.APIGeneratorcjTemplateToolStripMenuItem.Text = "API Generator (cj template)"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton5
        '
        Me.ToolStripButton5.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ModelBuilderToolStripMenuItem, Me.DataAccessBuilderToolStripMenuItem, Me.ControllerBuilderToolStripMenuItem, Me.UIControllerBuilderToolStripMenuItem, Me.ToolStripSeparator6, Me.DataAccessOnUIToolStripMenuItem, Me.DataAccessOnControllerAPIToolStripMenuItem})
        Me.ToolStripButton5.Image = CType(resources.GetObject("ToolStripButton5.Image"), System.Drawing.Image)
        Me.ToolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton5.Name = "ToolStripButton5"
        Me.ToolStripButton5.Size = New System.Drawing.Size(151, 22)
        Me.ToolStripButton5.Text = "DataAccess Generator"
        '
        'ModelBuilderToolStripMenuItem
        '
        Me.ModelBuilderToolStripMenuItem.Name = "ModelBuilderToolStripMenuItem"
        Me.ModelBuilderToolStripMenuItem.Size = New System.Drawing.Size(236, 22)
        Me.ModelBuilderToolStripMenuItem.Text = "Model Builder"
        '
        'DataAccessBuilderToolStripMenuItem
        '
        Me.DataAccessBuilderToolStripMenuItem.Name = "DataAccessBuilderToolStripMenuItem"
        Me.DataAccessBuilderToolStripMenuItem.Size = New System.Drawing.Size(236, 22)
        Me.DataAccessBuilderToolStripMenuItem.Text = "DataAccess Builder"
        '
        'ControllerBuilderToolStripMenuItem
        '
        Me.ControllerBuilderToolStripMenuItem.Name = "ControllerBuilderToolStripMenuItem"
        Me.ControllerBuilderToolStripMenuItem.Size = New System.Drawing.Size(236, 22)
        Me.ControllerBuilderToolStripMenuItem.Text = "API Controller Builder"
        '
        'UIControllerBuilderToolStripMenuItem
        '
        Me.UIControllerBuilderToolStripMenuItem.Name = "UIControllerBuilderToolStripMenuItem"
        Me.UIControllerBuilderToolStripMenuItem.Size = New System.Drawing.Size(236, 22)
        Me.UIControllerBuilderToolStripMenuItem.Text = "UI Controller Builder"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(233, 6)
        '
        'DataAccessOnUIToolStripMenuItem
        '
        Me.DataAccessOnUIToolStripMenuItem.Name = "DataAccessOnUIToolStripMenuItem"
        Me.DataAccessOnUIToolStripMenuItem.Size = New System.Drawing.Size(236, 22)
        Me.DataAccessOnUIToolStripMenuItem.Text = "DataAccess on Controller"
        '
        'DataAccessOnControllerAPIToolStripMenuItem
        '
        Me.DataAccessOnControllerAPIToolStripMenuItem.Name = "DataAccessOnControllerAPIToolStripMenuItem"
        Me.DataAccessOnControllerAPIToolStripMenuItem.Size = New System.Drawing.Size(236, 22)
        Me.DataAccessOnControllerAPIToolStripMenuItem.Text = "DataAccess on Controller (API)"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton3
        '
        Me.ToolStripButton3.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DefaultToolStripMenuItem, Me.ModalPopupToolStripMenuItem, Me.ModalPopup2TupleToolStripMenuItem, Me.ToolStripSeparator4, Me.BlankModalToolStripMenuItem, Me.HtmlTagHelpersOnlyToolStripMenuItem, Me.Select2ViewBagToolStripMenuItem, Me.ToolStripSeparator5, Me.ModalPopupcjTemplateToolStripMenuItem, Me.ToolStripMenuItem1, Me.ToolStripMenuItem2, Me.ToolStripSeparator8, Me.InFormDynamicTableToolStripMenuItem, Me.TabsGeneratorToolStripMenuItem, Me.ToolStripSeparator7, Me.DynamicMultiInputToolStripMenuItem})
        Me.ToolStripButton3.Image = CType(resources.GetObject("ToolStripButton3.Image"), System.Drawing.Image)
        Me.ToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton3.Name = "ToolStripButton3"
        Me.ToolStripButton3.Size = New System.Drawing.Size(104, 22)
        Me.ToolStripButton3.Text = "Form Builder"
        '
        'DefaultToolStripMenuItem
        '
        Me.DefaultToolStripMenuItem.Name = "DefaultToolStripMenuItem"
        Me.DefaultToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.DefaultToolStripMenuItem.Text = "Default"
        '
        'ModalPopupToolStripMenuItem
        '
        Me.ModalPopupToolStripMenuItem.Name = "ModalPopupToolStripMenuItem"
        Me.ModalPopupToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.ModalPopupToolStripMenuItem.Text = "Modal Popup"
        Me.ModalPopupToolStripMenuItem.ToolTipText = "Generate Table+Modal from given class (single model only)"
        '
        'ModalPopup2TupleToolStripMenuItem
        '
        Me.ModalPopup2TupleToolStripMenuItem.Name = "ModalPopup2TupleToolStripMenuItem"
        Me.ModalPopup2TupleToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.ModalPopup2TupleToolStripMenuItem.Text = "Modal Popup 2 (Tuple)"
        Me.ModalPopup2TupleToolStripMenuItem.ToolTipText = "Generate Table+Modal from given class (multiple model)"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(219, 6)
        '
        'BlankModalToolStripMenuItem
        '
        Me.BlankModalToolStripMenuItem.Name = "BlankModalToolStripMenuItem"
        Me.BlankModalToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.BlankModalToolStripMenuItem.Text = "Blank Modal"
        '
        'HtmlTagHelpersOnlyToolStripMenuItem
        '
        Me.HtmlTagHelpersOnlyToolStripMenuItem.Name = "HtmlTagHelpersOnlyToolStripMenuItem"
        Me.HtmlTagHelpersOnlyToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.HtmlTagHelpersOnlyToolStripMenuItem.Text = "@HtmlTag Helpers Only"
        '
        'Select2ViewBagToolStripMenuItem
        '
        Me.Select2ViewBagToolStripMenuItem.Name = "Select2ViewBagToolStripMenuItem"
        Me.Select2ViewBagToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.Select2ViewBagToolStripMenuItem.Text = "Select2 - ViewBag"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(219, 6)
        '
        'ModalPopupcjTemplateToolStripMenuItem
        '
        Me.ModalPopupcjTemplateToolStripMenuItem.Name = "ModalPopupcjTemplateToolStripMenuItem"
        Me.ModalPopupcjTemplateToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.ModalPopupcjTemplateToolStripMenuItem.Text = "Modal Popup (cj template)"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(222, 22)
        Me.ToolStripMenuItem1.Text = "Modal Popup (jee template)"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(222, 22)
        Me.ToolStripMenuItem2.Text = "Modal Popup (select2)"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(219, 6)
        '
        'InFormDynamicTableToolStripMenuItem
        '
        Me.InFormDynamicTableToolStripMenuItem.Name = "InFormDynamicTableToolStripMenuItem"
        Me.InFormDynamicTableToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.InFormDynamicTableToolStripMenuItem.Text = "In Form (Dynamic Table)"
        '
        'TabsGeneratorToolStripMenuItem
        '
        Me.TabsGeneratorToolStripMenuItem.Name = "TabsGeneratorToolStripMenuItem"
        Me.TabsGeneratorToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.TabsGeneratorToolStripMenuItem.Text = "Tabs Generator"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(219, 6)
        '
        'DynamicMultiInputToolStripMenuItem
        '
        Me.DynamicMultiInputToolStripMenuItem.Name = "DynamicMultiInputToolStripMenuItem"
        Me.DynamicMultiInputToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.DynamicMultiInputToolStripMenuItem.Text = "Dynamic Multi Input (Table)"
        '
        'ToolStripButton2
        '
        Me.ToolStripButton2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GETToolStripMenuItem, Me.FormPOSTToolStripMenuItem, Me.FormPOSTJSToolStripMenuItem, Me.DatatableGETToolStripMenuItem, Me.Select2AjaxToolStripMenuItem, Me.BsSuggestToolStripMenuItem})
        Me.ToolStripButton2.Image = CType(resources.GetObject("ToolStripButton2.Image"), System.Drawing.Image)
        Me.ToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton2.Name = "ToolStripButton2"
        Me.ToolStripButton2.Size = New System.Drawing.Size(160, 22)
        Me.ToolStripButton2.Text = "jQuery AJAX Generator"
        Me.ToolStripButton2.ToolTipText = "jQuery AJAX Generator (Model->Form->AJAX)"
        '
        'GETToolStripMenuItem
        '
        Me.GETToolStripMenuItem.Name = "GETToolStripMenuItem"
        Me.GETToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.GETToolStripMenuItem.Text = "GET"
        '
        'FormPOSTToolStripMenuItem
        '
        Me.FormPOSTToolStripMenuItem.Name = "FormPOSTToolStripMenuItem"
        Me.FormPOSTToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.FormPOSTToolStripMenuItem.Text = "Form POST"
        '
        'FormPOSTJSToolStripMenuItem
        '
        Me.FormPOSTJSToolStripMenuItem.Name = "FormPOSTJSToolStripMenuItem"
        Me.FormPOSTJSToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.FormPOSTJSToolStripMenuItem.Text = "Form POST (JS)"
        '
        'DatatableGETToolStripMenuItem
        '
        Me.DatatableGETToolStripMenuItem.Name = "DatatableGETToolStripMenuItem"
        Me.DatatableGETToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.DatatableGETToolStripMenuItem.Text = "Datatable GET"
        '
        'Select2AjaxToolStripMenuItem
        '
        Me.Select2AjaxToolStripMenuItem.Name = "Select2AjaxToolStripMenuItem"
        Me.Select2AjaxToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.Select2AjaxToolStripMenuItem.Text = "Select2 Ajax"
        '
        'BsSuggestToolStripMenuItem
        '
        Me.BsSuggestToolStripMenuItem.Name = "BsSuggestToolStripMenuItem"
        Me.BsSuggestToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.BsSuggestToolStripMenuItem.Text = "bsSuggest"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton4
        '
        Me.ToolStripButton4.Image = CType(resources.GetObject("ToolStripButton4.Image"), System.Drawing.Image)
        Me.ToolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton4.Name = "ToolStripButton4"
        Me.ToolStripButton4.Size = New System.Drawing.Size(117, 22)
        Me.ToolStripButton4.Text = "Datatable Builder"
        '
        'txtSource
        '
        Me.txtSource.BackColor = System.Drawing.SystemColors.Window
        Me.txtSource.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtSource.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSource.Location = New System.Drawing.Point(0, 0)
        Me.txtSource.Name = "txtSource"
        Me.txtSource.Size = New System.Drawing.Size(253, 394)
        Me.txtSource.TabIndex = 1
        Me.txtSource.Text = resources.GetString("txtSource.Text")
        Me.txtSource.WordWrap = False
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtSource)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtDest)
        Me.SplitContainer1.Size = New System.Drawing.Size(764, 394)
        Me.SplitContainer1.SplitterDistance = 253
        Me.SplitContainer1.SplitterWidth = 6
        Me.SplitContainer1.TabIndex = 2
        '
        'txtDest
        '
        Me.txtDest.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtDest.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDest.Location = New System.Drawing.Point(0, 0)
        Me.txtDest.Name = "txtDest"
        Me.txtDest.Size = New System.Drawing.Size(505, 394)
        Me.txtDest.TabIndex = 2
        Me.txtDest.Text = "Hello World!"
        Me.txtDest.WordWrap = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 419)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(764, 22)
        Me.StatusStrip1.TabIndex = 4
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(749, 17)
        Me.ToolStripStatusLabel1.Spring = True
        Me.ToolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(764, 441)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMain"
        Me.Text = "MVC Tooling - JHAPPS"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents txtSource As RichTextBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents txtDest As RichTextBox
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents ToolStripButton3 As ToolStripDropDownButton
    Friend WithEvents DefaultToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ModalPopupToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripButton4 As ToolStripButton
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents ToolStripButton5 As ToolStripDropDownButton
    Friend WithEvents ModelBuilderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DataAccessBuilderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ModalPopup2TupleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripButton2 As ToolStripSplitButton
    Friend WithEvents FormPOSTToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DatatableGETToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents BlankModalToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HtmlTagHelpersOnlyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Select2AjaxToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Select2ViewBagToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FormPOSTJSToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents ModalPopupcjTemplateToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripButton1 As ToolStripSplitButton
    Friend WithEvents APIGeneratorcjTemplateToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents TabsGeneratorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ControllerBuilderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents UIControllerBuilderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BsSuggestToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As ToolStripSeparator
    Friend WithEvents DynamicMultiInputToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DataAccessOnUIToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GETToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents InFormDynamicTableToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As ToolStripSeparator
    Friend WithEvents DataAccessOnControllerAPIToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
    Friend WithEvents ToolStripMenuItem2 As ToolStripMenuItem
End Class
