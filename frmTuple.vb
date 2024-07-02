Public Class frmTuple
    Private Sub frmTuple_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()

        For i = 1 To 10
            cboItem.Items.Add("Item" & i)
        Next
        cboItem.SelectedIndex = 0
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub
End Class