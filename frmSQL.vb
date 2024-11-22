Public Class frmSQL

    Property connectionString As String = ""

    Private Sub frmSQL_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()
        ComboBox1.Enabled = False
        btnAccept.Enabled = False
    End Sub

    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        Dim con = cSQLServer.BuildConnectionString(txtSource.Text, "", txtUser.Text, txtPass.Text)
        If cSQLServer.TestConnection(con) Then
            Using db = New cSQLServer(con)
                Dim dt = db.ExecuteToDatatable("SELECT name FROM sys.databases;")
                ComboBox1.Items.Clear()
                For i = 0 To dt.Rows.Count - 1
                    ComboBox1.Items.Add(dt.Rows(i)("name"))
                Next
                ComboBox1.Enabled = True
                btnAccept.Enabled = True
            End Using
        End If
    End Sub

    Private Sub btnAccept_Click(sender As Object, e As EventArgs) Handles btnAccept.Click
        If ComboBox1.SelectedIndex < 0 Then Return

        Dim con = cSQLServer.BuildConnectionString(txtSource.Text, ComboBox1.Text, txtUser.Text, txtPass.Text)
        If cSQLServer.TestConnection(con) Then
            connectionString = con
        End If

        If String.IsNullOrWhiteSpace(connectionString) Then Return
        Me.DialogResult = DialogResult.OK
    End Sub
End Class