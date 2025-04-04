Public Class frmSQLEditor
    Private Sub frmSQLEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToParent()
    End Sub

    Private Sub txtEditor_KeyUp(sender As Object, e As KeyEventArgs) Handles txtEditor.KeyUp
        If e.KeyCode = Keys.Escape Then
            Me.DialogResult = DialogResult.Cancel
        End If
    End Sub
End Class