Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class frmSQLBulkCopy


    Dim connHistory1 As New HashSet(Of String)
    Dim connHistory2 As New HashSet(Of String)
    Dim conn1 As String = ""
    Dim conn2 As String = ""
    Dim isconnected = False

    Private Sub frmSQLBulkCopy_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()
        LoadConHistory()
    End Sub

    Sub SaveConHistory()
        Dim js = JsonConvert.SerializeObject(New With {
            .SourceConnections = connHistory1.ToArray(),
            .DestinationConnections = connHistory2.ToArray(),
            .default1 = conn1,
            .default2 = conn2
        }, Formatting.Indented)

        File.WriteAllText(".\connhistory2", js, New UTF8Encoding(False))

        CheckForIllegalCrossThreadCalls = False
    End Sub

    Sub LoadConHistory()
        If File.Exists(".\connhistory2") Then
            Dim js = JRaw.Parse(File.ReadAllText(".\connhistory2", New UTF8Encoding(False)))
            connHistory1 = New HashSet(Of String)(js.SelectToken("SourceConnections").Select(Function(x) x.ToString).ToArray)
            connHistory2 = New HashSet(Of String)(js.SelectToken("DestinationConnections").Select(Function(x) x.ToString).ToArray)
            conn1 = js.SelectToken("default1").ToString
            conn2 = js.SelectToken("default2").ToString

            txtSource.Items.AddRange(connHistory1.ToArray)
            txtDest.Items.AddRange(connHistory2.ToArray)

            txtSource.Text = conn1
            txtDest.Text = conn2

        End If
    End Sub

    Sub UpdateProgress(status As String)
        If Me.InvokeRequired() Then
            Me.Invoke(Sub() UpdateProgress(status))
        Else
            lblStatus.Text = status
            Application.DoEvents()
        End If
    End Sub

    Sub AddToLog(status As String)
        If Me.InvokeRequired() Then
            Me.Invoke(Sub() AddToLog(status))
        Else
            txtSchema.AppendText(status & vbCrLf)
            Application.DoEvents()
        End If
    End Sub

    Function TESTCONNECTION(conn As String) As Boolean
        Try
            Using con = New OleDbConnection(conn)
                con.Open()
            End Using
            Return True
        Catch ex As Exception
            AddToLog($"Failed to connect on {conn} | {ex.Message}")
        End Try
        Return False
    End Function

    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        If isconnected = False Then
            If String.IsNullOrWhiteSpace(txtSource.Text) Then
                Beep()
                Return
            End If

            If String.IsNullOrWhiteSpace(txtDest.Text) Then
                Beep()
                Return
            End If
            If txtSource.SelectedIndex < 0 Then connHistory1.Add(txtSource.Text)
            If txtDest.SelectedIndex < 0 Then connHistory2.Add(txtDest.Text)

            If TESTCONNECTION(txtSource.Text) = False Then
                Beep()
                Return
            End If

            If TESTCONNECTION(txtDest.Text) = False Then
                Beep()
                Return
            End If

            conn1 = txtSource.Text
            conn2 = txtDest.Text

            SaveConHistory()

            AddToLog($"Connected to source: {txtSource.Text}")
            AddToLog($"Connected to dest: {txtDest.Text}")

            TriggerConnected()
        Else
            TriggerConnected()

            AddToLog($"DISCONNECTED")

        End If

    End Sub

    Sub TriggerConnected()
        isconnected = Not isconnected
        txtSource.Enabled = Not isconnected
        txtDest.Enabled = Not isconnected
        btnEditSrc.Enabled = Not isconnected
        btnBuildCNSource.Enabled = Not isconnected
        btnEditDest.Enabled = Not isconnected
        btnBuildCNDest.Enabled = Not isconnected

        btnConnect.Text = IIf(isconnected, "Disconnect", "Connect")

    End Sub

    Private Sub btnEditSrc_Click(sender As Object, e As EventArgs) Handles btnEditSrc.Click
        Using frm = New frmSQLEditor
            frm.txtEditor.Text = txtSource.Text
            frm.ShowDialog()
            txtSource.Text = frm.txtEditor.Text
        End Using
    End Sub

    Private Sub btnEditDest_Click(sender As Object, e As EventArgs) Handles btnEditDest.Click
        Using frm = New frmSQLEditor
            frm.txtEditor.Text = txtDest.Text
            frm.ShowDialog()
            txtDest.Text = frm.txtEditor.Text
        End Using
    End Sub

    Private Sub btnBuildCNSource_Click(sender As Object, e As EventArgs) Handles btnBuildCNSource.Click
        ' Create an empty .udl file
        Dim udlFile As String = ".\temp.udl"
        File.WriteAllText(udlFile, "")

        ' Open the .udl file using default system handler (this opens the Data Link UI)
        Dim proc As Process = Process.Start(New ProcessStartInfo(udlFile) With {.WindowStyle = ProcessWindowStyle.Normal})
        proc.WaitForExit()

        ' Read the connection string (last line in the UDL file)
        Dim cnn = File.ReadAllLines(udlFile).LastOrDefault()

        File.Delete(udlFile)

        If String.IsNullOrWhiteSpace(cnn) = False Then txtSource.Text = cnn
    End Sub

    Private Sub btnBuildCNDest_Click(sender As Object, e As EventArgs) Handles btnBuildCNDest.Click
        ' Create an empty .udl file
        Dim udlFile As String = ".\temp.udl"
        File.WriteAllText(udlFile, "")

        ' Open the .udl file using default system handler (this opens the Data Link UI)
        Dim proc As Process = Process.Start(New ProcessStartInfo(udlFile) With {.WindowStyle = ProcessWindowStyle.Normal})
        proc.WaitForExit()

        ' Read the connection string (last line in the UDL file)
        Dim cnn = File.ReadAllLines(udlFile).LastOrDefault()

        File.Delete(udlFile)

        If String.IsNullOrWhiteSpace(cnn) = False Then txtDest.Text = cnn
    End Sub

    Private Async Sub btnExecute_Click(sender As Object, e As EventArgs) Handles btnExecute.Click

        If isconnected = False Then
            MsgBox("Not connected !", vbExclamation)
            Return
        End If

        Dim forbid = {"delete", "truncate"}
        If forbid.Any(Function(x) txtSQL.Text.ToLower.Contains(x)) Then
            MsgBox("Delete/Truncate not allowed", vbExclamation)
            Return
        End If

        If Regex.Matches(txtSQL.Text, "SELECT", RegexOptions.IgnoreCase).Count > 1 AndAlso txtSQL.Text.Contains(";") = False Then
            MsgBox("Please split multiple sql with ending terminator ;", vbExclamation)
            Return
        End If

        Dim sqls = Regex.Split(txtSQL.Text, ";").Select(Function(x) x.Trim).Where(Function(x) x.Length > 0).ToList

        For s = 0 To sqls.Count - 1
            Dim s1 = Regex.Match(sqls(s), "FROM \[(.*?)\] ", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then s1 = Regex.Match(sqls(s), "FROM (.*?) ", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then s1 = Regex.Match(sqls(s), "FROM (.*?)$", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then
                AddToLog($"Skipped/No Tablename found !")
                Return
            End If

            Dim tblName = s1
            Dim sqlQuery = sqls(s)

            ' get all tables in sql server database
            Try
                Dim dt As DataTable = Await GetDatatable(sqlQuery)
                dtGrid.DataSource = Nothing
                dtGrid.DataSource = dt
            Catch ex As Exception
                AddToLog(ex.Message)
            End Try
        Next

        GC.Collect()
        GC.WaitForFullGCApproach()
        GC.Collect()

    End Sub

    Async Function GetDatatable(sqlQuery As String) As Task(Of DataTable)

        Dim sqls = Regex.Split(sqlQuery, ";").Select(Function(x) x.Trim).Where(Function(x) x.Length > 0).ToList

        For s = 0 To sqls.Count - 1
            Dim s1 = Regex.Match(sqls(s), "FROM \[(.*?)\] ", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then s1 = Regex.Match(sqls(s), "FROM (.*?) ", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then s1 = Regex.Match(sqls(s), "FROM (.*?)$", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then
                AddToLog($"Skipped/No Tablename found !")
                Return Nothing
            End If

            Dim tblName = s1

            ' get all tables in sql server database
            Try
                Dim t1 = Now
                Dim dt As DataTable = Nothing
                Dim tables As New List(Of String)
                Using conn As New OleDbConnection(txtSource.Text)
                    conn.Open()
                    Using cmd As New OleDbCommand(sqlQuery, conn)
                        cmd.CommandTimeout = Integer.Parse(optCommandTimeout.Text)

                        dt = New DataTable
                        dt.Load(cmd.ExecuteReader(CommandBehavior.KeyInfo))


                        conn.Close()

                        Dim t2 = Now
                        AddToLog($"SQL Executed {(t2 - t1).TotalSeconds}s | table {tblName} | Total Records : {dt.Rows.Count}")
                        UpdateProgress($"SQL Executed {(t2 - t1).TotalSeconds}s | table {tblName} | Total Records : {dt.Rows.Count}")

                        Return dt
                    End Using
                End Using
            Catch ex As Exception
                AddToLog(ex.Message)
            End Try
        Next

        Return Nothing
    End Function

    Private Async Sub btnExportCSV_Click(sender As Object, e As EventArgs) Handles btnExportCSV.Click

        If isconnected = False Then
            MsgBox("Not connected !", vbExclamation)
            Return
        End If

        If optNoStaging.Checked Then
            If MsgBox("Are you sure you want to overwrite existing tables without staging?", vbExclamation + vbYesNo + vbDefaultButton2) <> MsgBoxResult.Yes Then
                Return
            End If
        End If

        Dim forbid = {"delete", "truncate"}
        If forbid.Any(Function(x) txtSQL.Text.ToLower.Contains(x)) Then
            MsgBox("Delete/Truncate Not allowed", vbExclamation)
            Return
        End If

        If Regex.Matches(txtSQL.Text, "Select", RegexOptions.IgnoreCase).Count > 1 AndAlso txtSQL.Text.Contains(";") = False Then
            MsgBox("Please split multiple sql With ending terminator ;", vbExclamation)
            Return
        End If

        Dim sqls = Regex.Split(txtSQL.Text, ";").Select(Function(x) x.Trim).Where(Function(x) x.Length > 0).ToList

        For s = 0 To sqls.Count - 1
            Dim s1 = Regex.Match(sqls(s), "FROM \[(.*?)\] ", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then s1 = Regex.Match(sqls(s), "FROM (.*?) ", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then s1 = Regex.Match(sqls(s), "FROM (.*?)$", RegexOptions.IgnoreCase).Groups(1).Value.Trim
            If String.IsNullOrWhiteSpace(s1) Then
                AddToLog($"Skipped/No Tablename found !")
                Return
            End If

            Dim tblName = s1
            Dim sqlQuery = sqls(s)
            Dim staging = $"STAGING_{tblName.Split("..").LastOrDefault}"

            If optNoStaging.Checked Then staging = tblName

            AddToLog($"Copying data from {tblName} to {staging} ... ")

            ' get all tables in sql server database
            Try

                Dim dt As DataTable = Await GetDatatable(sqlQuery)

                GC.Collect()

                If optDirectImport.Checked = False Then
                    Using con = New OleDbConnection(txtDest.Text)
                        con.Open()
                        Dim trans = con.BeginTransaction
                        Using cmd = con.CreateCommand()
                            cmd.Transaction = trans
                            cmd.CommandText = $"drop table if exists [{staging}]; -- select top 0 * into [{staging}] from {tblName}"
                            Dim res = cmd.ExecuteNonQuery

                            ' Create staging table using DataTable schema
                            Dim createTableSql = $"CREATE TABLE [{staging}] ("
                            For Each column As DataColumn In dt.Columns
                                Dim sqlType = GetSqlTypeFromDataColumn(column)
                                createTableSql += $"[{column.ColumnName}] {sqlType}, "
                            Next
                            createTableSql = createTableSql.TrimEnd(", ".ToCharArray()) + ")"

                            cmd.CommandText = createTableSql
                            cmd.ExecuteNonQuery()

                            trans.Commit()
                        End Using

                    End Using
                End If

                Dim t1 = Now
                Dim newcon = ConvertToSqlConnectionString(txtDest.Text)
                Using bulkCopy As New SqlBulkCopy(newcon, SqlBulkCopyOptions.KeepIdentity)
                    bulkCopy.DestinationTableName = staging
                    bulkCopy.BatchSize = 5000
                    bulkCopy.BulkCopyTimeout = 0 ' no timeout
                    bulkCopy.WriteToServer(dt)
                End Using
                Dim t2 = Now
                AddToLog($"SqlBulkCopy Executed {(t2 - t1).TotalSeconds}s | table {tblName} | Total Records : {dt.Rows.Count}")
                AddToLog($"=======================")

            Catch ex As Exception
                AddToLog(ex.Message)
            End Try
        Next



    End Sub

    Private Function GetSqlTypeFromDataColumn(column As DataColumn) As String
        Dim type = column.DataType

        If type Is GetType(String) Then
            Dim maxLength = If(column.MaxLength > 0 AndAlso column.MaxLength < 8000, column.MaxLength, "MAX")
            Return $"NVARCHAR({maxLength})"
        ElseIf type Is GetType(Integer) Then
            Return "INT"
        ElseIf type Is GetType(Long) Then
            Return "BIGINT"
        ElseIf type Is GetType(Short) Then
            Return "SMALLINT"
        ElseIf type Is GetType(Byte) Then
            Return "TINYINT"
        ElseIf type Is GetType(Decimal) Then
            Return "DECIMAL(18,6)"
        ElseIf type Is GetType(Double) Then
            Return "FLOAT"
        ElseIf type Is GetType(Single) Then
            Return "REAL"
        ElseIf type Is GetType(Boolean) Then
            Return "BIT"
        ElseIf type Is GetType(DateTime) Then
            Return "DATETIME"
        ElseIf type Is GetType(Guid) Then
            Return "UNIQUEIDENTIFIER"
        Else
            Return "NVARCHAR(MAX)"
        End If
    End Function

    Public Function ConvertToSqlConnectionString(oleConnString As String) As String
        Dim builder As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        ' Split by semicolon
        For Each part In oleConnString.Split(";"c)
            If String.IsNullOrWhiteSpace(part) Then Continue For

            Dim keyValue = part.Split("=".ToCharArray, 2)
            If keyValue.Length = 2 Then
                Dim key = keyValue(0).Trim()
                Dim value = keyValue(1).Trim()
                If Not builder.ContainsKey(key) Then builder.Add(key, value)
            End If
        Next

        ' Map OleDb keys to SqlClient keys
        Dim dataSource As String = ""
        Dim initialCatalog As String = ""
        Dim userID As String = ""
        Dim password As String = ""
        Dim integratedSecurity As Boolean = False

        If builder.ContainsKey("Data Source") Then dataSource = builder("Data Source")
        If builder.ContainsKey("Server") Then dataSource = builder("Server")

        If builder.ContainsKey("Initial Catalog") Then initialCatalog = builder("Initial Catalog")
        If builder.ContainsKey("Database") Then initialCatalog = builder("Database")

        If builder.ContainsKey("User ID") Then userID = builder("User ID")
        If builder.ContainsKey("UID") Then userID = builder("UID")

        If builder.ContainsKey("Password") Then password = builder("Password")
        If builder.ContainsKey("PWD") Then password = builder("PWD")

        If builder.ContainsKey("Trusted_Connection") Then
            integratedSecurity = (builder("Trusted_Connection").ToLower() = "yes" OrElse
                                  builder("Trusted_Connection").ToLower() = "true")
        End If

        ' Build SqlConnection string
        Dim sql As New Text.StringBuilder()

        sql.Append($"Data Source={dataSource};")

        If initialCatalog <> "" Then
            sql.Append($"Initial Catalog={initialCatalog};")
        End If

        If integratedSecurity Then
            sql.Append("Integrated Security=True;")
        Else
            sql.Append($"User ID={userID};Password={password};")
        End If

        sql.Append("TrustServerCertificate=True;")
        sql.Append("Persist Security Info=True;")

        Return sql.ToString()
    End Function

End Class