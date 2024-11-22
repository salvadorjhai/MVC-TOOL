Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Text

Public Class cSQLServer
    Implements IDisposable

    Event onError(sender As Object, e As Exception)

    Public Class Parameter
        Inherits List(Of SqlParameter)
        Implements ICloneable

        ''' <summary>
        ''' Add or replaces parameter if existing
        ''' </summary>
        ''' <param name="parameterName">parameter name prefix with @ symbol</param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Function AddParameter(parameterName As String, value As Object) As Parameter
            Dim ele = Me.Where(Function(x) x.ParameterName = parameterName).FirstOrDefault
            If ele Is Nothing Then
                Me.Add(New SqlParameter(parameterName, value))
            Else
                ele.Value = value
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Build insert string
        ''' </summary>
        ''' <param name="tableName"></param>
        ''' <returns></returns>
        Function ToInsertString(tableName As String) As String
            Dim query, columns, values As New StringBuilder
            query.AppendLine("INSERT INTO [" & tableName & "] (")
            For i = 0 To Me.Count - 1
                Me(i).ParameterName = Me(i).ParameterName.Replace("@", "").Trim
                '
                If columns.ToString.Trim.Length <> 0 Then columns.Append(", ")
                If values.ToString.Trim.Length <> 0 Then values.Append(", ")
                '
                columns.Append("[").Append(Me(i).ParameterName).Append("]")
                values.Append("@").Append(Me(i).ParameterName)
                '
            Next
            query.AppendLine(columns.ToString).AppendLine(") VALUES (").AppendLine(values.ToString).Append(");")
            Return query.ToString
        End Function

        ''' <summary>
        ''' build update string
        ''' </summary>
        ''' <param name="tableName"></param>
        ''' <param name="whereCondition"></param>
        ''' <returns></returns>
        Function ToUpdateString(tableName As String, whereCondition As String) As String
            Dim query, columns, values As New StringBuilder
            query.AppendLine("UPDATE [" & tableName & "] SET ")
            For i = 0 To Me.Count - 1
                Me(i).ParameterName = Me(i).ParameterName.Replace("@", "").Trim
                '
                If columns.ToString.Trim.Length <> 0 Then columns.Append(", ")
                columns.Append("[").Append(Me(i).ParameterName).Append("]=")
                '
                columns.Append("@").Append(Me(i).ParameterName)
                '
            Next
            query.AppendLine(columns.ToString)
            query.AppendLine(whereCondition)

            Return query.ToString
        End Function

        Public Overrides Function ToString() As String
            Dim query, columns, values As New StringBuilder
            For i = 0 To Me.Count - 1
                If columns.ToString.Trim.Length <> 0 Then columns.Append(", ")
                columns.Append("[").Append(Me(i).ParameterName).Append("]=").Append(Me(i).Value)
            Next
            query.AppendLine(columns.ToString)
            Return query.ToString
        End Function

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function
    End Class

    Class Pager
        Property currentPage As Integer = 0
        Property maxPage As Integer = 0
        Property limitPerPage As Integer = 200
        Property dbx As cSQLServer = Nothing

        Property columns As String = ""
        Property table As String = ""
        Property where As String = ""
        Property orderby As String = ""
        Property param As List(Of SqlParameter) = Nothing

        Private mIsInitialized As Boolean = False
        ReadOnly Property IsInitialized As Boolean
            Get
                Return mIsInitialized
            End Get
        End Property

        Private mTotal As Integer = 0
        ReadOnly Property TotalRecord As Integer
            Get
                Return mTotal
            End Get
        End Property

        ReadOnly Property FullQuery As String
            Get
                Return $"SELECT {columns} FROM [{table}] {where} {orderby}"
            End Get
        End Property

        ReadOnly Property FullQueryWithLimit As String
            Get
                Dim startRow = ((currentPage - 1) * limitPerPage) + 1
                Dim endRow = (currentPage * limitPerPage) + 1
                Dim sql = $"SELECT *
                            FROM (
                                SELECT ROW_NUMBER() OVER ({orderby}) AS RowNum, {columns}
                                FROM [{table}] {where} 
                            ) AS RowConstrainedResult
                            WHERE RowNum >= {startRow} AND RowNum < {endRow}
                            {orderby}"
                Return sql
            End Get
        End Property

        ''' <summary>
        ''' Initialize pager 
        ''' <code>
        ''' Dim pager As New cSQLServer.Pager(DBX_SERVER, _tableName)
        ''' Dim prg As Long = 0
        ''' Dim dt As DataTable = pager.GetNext
        ''' Do While True
        '''     If dt Is Nothing Then Exit Do
        '''     If dt.Rows.Count = 0 Then Exit Do
        '''     For o = 0 To dt.Rows.Count - 1
        '''         prg += 1
        '''         callback_onProgressReport2(Me, String.Format("Rescanning {0} of {1}", prg, pager.TotalRecord))
        '''         Dim data As New itemData(dt.Rows(o))
        '''         NewAction(Sub() scrapDetails(data.Clone))
        '''         If IsCancelled Then Exit For
        '''     Next
        '''     WaitAllThreads(PROPS.UserSetting.thread_timeout)
        '''     If IsCancelled Then Exit Do
        '''     dt = pager.GetNext
        ''' Loop
        ''' </code>
        ''' </summary>
        ''' <param name="con"></param>
        ''' <param name="table"></param>
        ''' <param name="columns">*</param>
        ''' <param name="where">WHERE ...</param>
        ''' <param name="orderby">ORDER BY ...</param>
        ''' <param name="param"></param>
        ''' <param name="currentPage"></param>
        ''' <param name="limitPerPage"></param>
        Sub New(con As cSQLServer, table As String, Optional columns As String = "*", Optional where As String = "", Optional orderby As String = "ORDER BY id ASC",
                Optional param As List(Of SqlParameter) = Nothing,
                Optional currentPage As Integer = 0, Optional limitPerPage As Integer = 200)

            Me.dbx = con
            Me.currentPage = currentPage
            Me.limitPerPage = limitPerPage

            Me.columns = columns
            Me.table = table
            Me.where = where
            Me.orderby = orderby
            Me.param = param

            mIsInitialized = True
            mTotal = dbx.ExecuteScalar($"SELECT count(1) FROM [{table}] {where}", param)
            Try
                maxPage = Math.Ceiling(mTotal / limitPerPage)
            Catch ex As Exception
            End Try
        End Sub

        Private Function _get() As DataTable
            Dim res = dbx.ExecuteToDatatable(FullQueryWithLimit, param)
            Try
                mTotal = dbx.ExecuteScalar($"SELECT count(1) FROM [{table}] {where}", param)
                maxPage = Math.Ceiling(mTotal / limitPerPage)
            Catch ex As Exception
            End Try
            Return res
        End Function

        ''' <summary>
        ''' return previous set of data
        ''' </summary>
        ''' <returns></returns>
        Function GetPrevious() As DataTable
            If currentPage <= 0 Then Return Nothing
            currentPage -= 1
            Return _get()
        End Function

        ''' <summary>
        ''' return next set of data
        ''' </summary>
        ''' <returns></returns>
        Function GetNext() As DataTable
            If currentPage = 0 Then currentPage = 1
            If currentPage > maxPage Then Return Nothing
            Dim dt = _get()
            currentPage += 1
            Return dt
        End Function

    End Class

    Private m_con As SqlConnection = Nothing
    Property Connection As SqlConnection
        Get
            Return m_con
        End Get
        Set(value As SqlConnection)
            m_con = value
        End Set
    End Property

    Private mConnectionString As String = ""
    ReadOnly Property ConnectionString As String
        Get
            Return mConnectionString
        End Get
    End Property

    ''' <summary>
    ''' Initialize SQL Server Connection
    ''' </summary>
    ''' <param name="connectionString">Server=myServerName\myInstanceName;Database=myDataBase;User Id=myUsername;Password=myPassword;</param>
    Sub New(connectionString)
        mConnectionString = connectionString
        If IsNothing(m_con) Then m_con = New SqlConnection(mConnectionString)
    End Sub

    Sub Open()
        If m_con.State <> ConnectionState.Open Then m_con.Open()
    End Sub
    Sub Close()
        If IsNothing(m_con) Then Return
        m_con.Close()
    End Sub

    ''' <summary>
    ''' Test connection
    ''' </summary>
    ''' <returns></returns>
    Shared Function TestConnection(connString As String) As Boolean
        Using xcon As New SqlConnection(connString)
            Try
                If xcon.State <> ConnectionState.Open Then xcon.Open()
                Do While xcon.State = ConnectionState.Connecting
                Loop
                If xcon.State = ConnectionState.Open Then Return True
            Catch ex As Exception
            End Try
        End Using
        Return False
    End Function

    ''' <summary>
    ''' build sql server connection string
    ''' </summary>
    ''' <param name="server">server,port or server\instancename</param>
    ''' <param name="database"></param>
    ''' <param name="userId"></param>
    ''' <param name="password"></param>
    ''' <returns></returns>
    Shared Function BuildConnectionString(server As String, database As String, userId As String, password As String) As String
        Dim user As String = IIf(String.IsNullOrWhiteSpace(userId), "", $"User Id={userId};")
        Dim pass As String = IIf(String.IsNullOrWhiteSpace(password), "", $"Password={password};")
        Dim windowsAuth As String = IIf(String.IsNullOrWhiteSpace(userId) = False, "", "Integrated Security=True;")

        Return $"Server={server}; Database={database}; {user}{pass} Connect Timeout=900; {windowsAuth} Asynchronous Processing=True; MultipleActiveResultSets=True; User Instance=False; Persist Security Info=False; Packet Size=4096"
    End Function

    ''' <summary>
    ''' format given date/time to sql datetime string format
    ''' </summary>
    ''' <param name="d"></param>
    ''' <param name="dtFormat"></param>
    ''' <returns></returns>
    Shared Function SqlDateTimeFormat(d As Date, Optional dtFormat As String = "yyyy-MM-dd HH:mm:ss") As String
        Return Format(d, dtFormat)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function ExecuteScalar(sql As String, params As Dictionary(Of String, Object)) As Object
        For retry = 1 To 25
            Try
                If m_con.State <> ConnectionState.Open Then m_con.Open()
                Do While m_con.State = ConnectionState.Connecting
                Loop
                If m_con.State <> ConnectionState.Open Then Continue For
                '
                Using cmd As SqlCommand = m_con.CreateCommand()
                    If params IsNot Nothing AndAlso params.Count > 0 Then
                        For i = 0 To params.Count - 1
                            Dim k = IIf(params.Keys(i).StartsWith("@"), params.Keys(i), $"@{params.Keys(i)}") ' must contain @
                            cmd.Parameters.AddWithValue(params.Keys(i), IIf(IsNothing(params.Values(i)), DBNull.Value, params.Values(i)))
                        Next
                    End If
                    cmd.CommandText = sql
                    cmd.CommandTimeout = 90
                    Return cmd.ExecuteScalar
                End Using
            Catch ex As Exception
                Task.Delay(5000).Wait()
            End Try
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function ExecuteNonQuery(sql As String, params As Dictionary(Of String, Object)) As Integer
        For retry = 1 To 5
            Try
                If m_con.State <> ConnectionState.Open Then m_con.Open()
                Do While m_con.State = ConnectionState.Connecting
                Loop
                If m_con.State <> ConnectionState.Open Then Continue For
            Catch ex As Exception
            End Try
            '
            Using cmd As SqlCommand = m_con.CreateCommand()
                If params IsNot Nothing AndAlso params.Count > 0 Then
                    For i = 0 To params.Count - 1
                        Dim k = IIf(params.Keys(i).StartsWith("@"), params.Keys(i), $"@{params.Keys(i)}") ' must contain @
                        cmd.Parameters.AddWithValue(params.Keys(i), IIf(IsNothing(params.Values(i)), DBNull.Value, params.Values(i)))
                    Next
                End If
                cmd.CommandText = sql
                cmd.CommandTimeout = 90
                Return cmd.ExecuteNonQuery()
            End Using
        Next

        Return -1
    End Function

    ''' <summary>
    ''' Execute query and return a datatabe
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function ExecuteToDatatable(sql As String, params As Dictionary(Of String, Object)) As DataTable
        Dim dt As DataTable = Nothing
        For retry = 1 To 5
            Try
                Using xcon As New SqlConnection(ConnectionString)
                    If xcon.State <> ConnectionState.Open Then xcon.Open()
                    Do While xcon.State = ConnectionState.Connecting
                    Loop
                    If xcon.State <> ConnectionState.Open Then Continue For

                    Using cmd As SqlCommand = xcon.CreateCommand()
                        If params IsNot Nothing AndAlso params.Count > 0 Then
                            For i = 0 To params.Count - 1
                                Dim k = IIf(params.Keys(i).StartsWith("@"), params.Keys(i), $"@{params.Keys(i)}") ' must contain @
                                cmd.Parameters.AddWithValue(params.Keys(i), IIf(IsNothing(params.Values(i)), DBNull.Value, params.Values(i)))
                            Next
                        End If
                        cmd.CommandText = sql
                        cmd.CommandTimeout = 90
                        Using dr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.KeyInfo)
                            dt = New DataTable
                            dt.Load(dr)
                            Return dt
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Task.Delay(5000).Wait()
            End Try
        Next
        Return dt
    End Function

    ''' <summary>
    ''' Execute query and return a single row
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function QuerySingleResult(sql As String, params As Dictionary(Of String, Object)) As DataRow
        Dim dt As DataTable = ExecuteToDatatable(sql, params)
        If dt IsNot Nothing Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0)
            End If
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Insert given paramters to table and return row
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="columnData"></param>
    ''' <returns></returns>
    Function InsertAndReturnRow(tableName As String, columnData As Dictionary(Of String, Object)) As DataRow
        Dim query As New List(Of String)

        'without @
        Dim col1 As List(Of String) = columnData.Keys.Select(Function(x) $"[{IIf(x.StartsWith("@"), x.Substring(1), x).ToString()}]").ToList
        'with @
        Dim col2 As List(Of String) = columnData.Keys.Select(Function(x) IIf(x.StartsWith("@"), x, $"@{x}").ToString()).ToList

        query.Add($"INSERT INTO [{tableName}] ")
        query.Add($"({String.Join(", ", col1)}) ")
        query.Add($"VALUES ({String.Join(", ", col2)}); ")
        query.Add($"")
        query.Add($"SELECT * FROM [{tableName}] WHERE id = SCOPE_IDENTITY(); ")

        Return QuerySingleResult(String.Join(vbCrLf, query), columnData)
    End Function

    ''' <summary>
    ''' Update table with given parameter with condition and return updated row
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="id"></param>
    ''' <param name="columnData"></param>
    ''' <returns></returns>
    Function UpdateAndReturnRow(tableName As String, id As Long, Optional columnData As Dictionary(Of String, Object) = Nothing) As DataRow
        Dim query As New List(Of String)

        '[key]=@key
        Dim col1 As List(Of String) = columnData.Keys.Select(Function(x)
                                                                 Dim k = IIf(x.StartsWith("@"), x.Substring(1), x).ToString()
                                                                 Return $"[{k}]=@{k}"
                                                             End Function).ToList

        query.Add($"UPDATE [{tableName}]")
        query.Add($"SET {String.Join($", {vbCrLf}  ", col1)} ")
        query.Add($"WHERE id = {id} ;")
        query.Add($"")
        query.Add($"SELECT * FROM [{tableName}] WHERE id = {id} ;")

        Return QuerySingleResult(String.Join(vbCrLf, query), columnData)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function ExecuteNonQuery(sql As String, Optional params As List(Of SqlParameter) = Nothing) As Integer
        For retry = 1 To 5
            Try
                If m_con.State <> ConnectionState.Open Then m_con.Open()
                Do While m_con.State = ConnectionState.Connecting
                Loop
                If m_con.State <> ConnectionState.Open Then Continue For
            Catch ex As Exception
            End Try
            '
            Using cmd As SqlCommand = m_con.CreateCommand()
                If params IsNot Nothing Then cmd.Parameters.AddRange(params.ToArray)
                cmd.CommandText = sql
                cmd.CommandTimeout = 90
                Return cmd.ExecuteNonQuery()
            End Using
        Next

        Return -1
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function ExecuteScalar(sql As String, Optional params As List(Of SqlParameter) = Nothing) As Object
        For retry = 1 To 25
            Try
                If m_con.State <> ConnectionState.Open Then m_con.Open()
                Do While m_con.State = ConnectionState.Connecting
                Loop
                If m_con.State <> ConnectionState.Open Then Continue For
                '
                Using cmd As SqlCommand = m_con.CreateCommand()
                    If params IsNot Nothing Then cmd.Parameters.AddRange(params.ToArray)
                    cmd.CommandText = sql
                    cmd.CommandTimeout = 90
                    Return cmd.ExecuteScalar
                End Using
            Catch ex As Exception
                Task.Delay(5000).Wait()
            End Try
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' Execute query and return a datatabe
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function ExecuteToDatatable(sql As String, Optional params As List(Of SqlParameter) = Nothing) As DataTable
        Dim dt As DataTable = Nothing
        For retry = 1 To 5
            Try
                Using xcon As New SqlConnection(ConnectionString)
                    If xcon.State <> ConnectionState.Open Then xcon.Open()
                    Do While xcon.State = ConnectionState.Connecting
                    Loop
                    If xcon.State <> ConnectionState.Open Then Continue For
                    '
                    Using cmd As SqlCommand = xcon.CreateCommand()
                        If params IsNot Nothing Then cmd.Parameters.AddRange(params.ToArray)
                        cmd.CommandText = sql
                        cmd.CommandTimeout = 90
                        Using dr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.KeyInfo)
                            dt = New DataTable
                            dt.Load(dr)
                            Return dt
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Task.Delay(5000).Wait()
            End Try
        Next
        Return dt
    End Function

    ''' <summary>
    ''' Execute sql query and get value list of first column
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function ExecuteToList(sql As String, Optional params As List(Of SqlParameter) = Nothing) As List(Of String)
        Dim lst As New List(Of String)
        Using dt As DataTable = ExecuteToDatatable(sql, params)
            If dt IsNot Nothing Then
                For i = 0 To dt.Rows.Count - 1
                    lst.Add(dt.Rows(i).Item(0))
                Next
            End If
        End Using
        Return lst
    End Function

    ''' <summary>
    ''' Execute query and return a single row
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function QuerySingleResult(sql As String, Optional params As List(Of SqlParameter) = Nothing) As DataRow
        Dim dt As DataTable = ExecuteToDatatable(sql, params)
        If dt IsNot Nothing Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0)
            End If
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Insert or replace existing value
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="params"></param>
    ''' <param name="whereCondition"></param>
    ''' <param name="ReturnLastRowID"></param>
    ''' <returns></returns>
    Function InsertOrReplace(tableName As String, params As Parameter, whereCondition As String, Optional ReturnLastRowID As Boolean = True) As Long
        Dim paramCopy As New Parameter
        Dim query, columns, values As New StringBuilder
        query.AppendLine("UPDATE [" & tableName & "] SET ")
        For i = 0 To params.Count - 1
            'remove '@' first
            params(i).ParameterName = params(i).ParameterName.Replace("@", "").Trim
            '
            If columns.ToString.Trim.Length <> 0 Then columns.Append(", ")
            columns.Append("[").Append(params(i).ParameterName).Append("]=@").Append($"parameter_{i}")

            paramCopy.Add(New SqlParameter($"@parameter_{i}", params(i).Value))
        Next
        query.AppendLine(columns.ToString)
        query.AppendLine(whereCondition)
        query.AppendLine("IF @@ROWCOUNT=0")
        query.AppendLine("INSERT INTO [" & tableName & "] (")

        ' clear
        columns.Clear() : values.Clear()

        For i = 0 To params.Count - 1
            'remove '@' first
            'params(i).ParameterName = params(i).ParameterName.Replace("@", "").Trim
            '
            If columns.ToString.Trim.Length <> 0 Then columns.Append(", ")
            If values.ToString.Trim.Length <> 0 Then values.Append(", ")

            columns.Append("[").Append(params(i).ParameterName).Append("]")
            values.Append("@").Append($"parameter_{i}") ' remove spaces

            'paramCopy.Add(New SqlParameter("@" & params(i).ParameterName.Replace(" ", ""), params(i).Value))
        Next
        query.AppendLine(columns.ToString).AppendLine(") VALUES (").AppendLine(values.ToString).Append(");")

        If ExecuteNonQuery(query.ToString, paramCopy) = 1 Then
            If ReturnLastRowID Then
                Return ExecuteScalar("SELECT @@IDENTITY")
            Else
                Return 0
            End If
        End If
        Return -1
    End Function

    ''' <summary>
    ''' Insert given paramters to table and return last inserted id
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="params"></param>
    ''' <param name="ReturnLastRowID"></param>
    ''' <returns>0 - Good, -1 Not Good, other is last row id</returns>
    Function InsertParam(tableName As String, Optional params As List(Of SqlParameter) = Nothing, Optional ReturnLastRowID As Boolean = True) As Long
        Dim paramCopy As New List(Of SqlParameter)
        Dim query, columns, values As New StringBuilder
        query.AppendLine("INSERT INTO [" & tableName & "] (")
        For i = 0 To params.Count - 1
            'remove '@' first
            params(i).ParameterName = params(i).ParameterName.Replace("@", "").Trim
            '
            If columns.ToString.Trim.Length <> 0 Then columns.Append(", ")
            If values.ToString.Trim.Length <> 0 Then values.Append(", ")

            columns.Append("[").Append(params(i).ParameterName).Append("]")
            values.Append("@").Append($"parameter_{i}") ' remove spaces

            paramCopy.Add(New SqlParameter($"@parameter_{i}", params(i).Value))
        Next
        query.AppendLine(columns.ToString).AppendLine(") VALUES (").AppendLine(values.ToString).Append(");")
        If ExecuteNonQuery(query.ToString, paramCopy) = 1 Then
            If ReturnLastRowID Then
                Return ExecuteScalar("SELECT @@IDENTITY")
            Else
                Return 0
            End If
        End If
        Return -1
    End Function

    ''' <summary>
    ''' Insert given parameters to table and return inserted data (as datarow)
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="params"></param>
    ''' <param name="queryFromViewName"></param>
    ''' <returns></returns>
    Function InsertParam2(tableName As String, Optional params As List(Of SqlParameter) = Nothing, Optional queryFromViewName As String = "") As DataRow
        Dim lastId As Long = InsertParam(tableName, params)
        If lastId = -1 Then
            Return Nothing
        End If
        If queryFromViewName.Trim.Length <> 0 Then tableName = queryFromViewName
        Return QuerySingleResult($"SELECT * FROM {tableName} WHERE {tableName}.id = {lastId}")
    End Function

    ''' <summary>
    ''' Update table with given parameter and condition and return number of affected rows
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="whereCondition">WHERE ID = 1</param>
    ''' <param name="params"></param>
    ''' <returns></returns>
    Function UpdateParam(tableName As String, whereCondition As String, Optional params As List(Of SqlParameter) = Nothing) As Long
        Dim paramCopy As New List(Of SqlParameter)
        Dim query, columns, values As New StringBuilder
        query.AppendLine("UPDATE [" & tableName & "] SET ")
        For i = 0 To params.Count - 1
            'remove '@' first
            params(i).ParameterName = params(i).ParameterName.Replace("@", "").Trim
            '
            If columns.ToString.Trim.Length <> 0 Then columns.Append(", ")
            columns.Append("[").Append(params(i).ParameterName).Append("]=@").Append($"parameter_{i}")

            paramCopy.Add(New SqlParameter($"@parameter_{i}", params(i).Value))
        Next
        query.AppendLine(columns.ToString)
        query.AppendLine(whereCondition)
        Return ExecuteNonQuery(query.ToString, paramCopy)
    End Function

    ''' <summary>
    ''' Update table with given parameter and condition and return number of affected rows
    ''' </summary>
    ''' <param name="tableName">table name</param>
    ''' <param name="params">columns to update</param>
    ''' <param name="whereCondition">where condition</param>
    ''' <param name="where_parameters">condition parameters</param>
    ''' <returns></returns>
    Function UpdateParam(tableName As String, params As List(Of SqlParameter), whereCondition As String, Optional where_parameters As List(Of SqlParameter) = Nothing) As Long
        Dim paramCopy As New List(Of SqlParameter)
        Dim query, columns, values As New StringBuilder
        query.AppendLine("UPDATE [" & tableName & "] SET ")
        For i = 0 To params.Count - 1
            'remove '@' first
            params(i).ParameterName = params(i).ParameterName.Replace("@", "").Trim
            '
            If columns.ToString.Trim.Length <> 0 Then columns.Append(", ")
            columns.Append("[").Append(params(i).ParameterName).Append("]=@").Append($"parameter_{i}")

            paramCopy.Add(New SqlParameter($"@parameter_{i}", params(i).Value))
        Next

        For i = 0 To where_parameters.Count - 1
            paramCopy.Add(New SqlParameter("@" & where_parameters(i).ParameterName.Replace("@", "").Trim, where_parameters(i).Value))
        Next

        query.AppendLine(columns.ToString)
        query.AppendLine(whereCondition)
        Return ExecuteNonQuery(query.ToString, paramCopy)
    End Function

    ''' <summary>
    ''' Update table with given parameter and return single data
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="whereCondition">WHERE ID = 1</param>
    ''' <param name="params"></param>
    ''' <param name="queryFromViewName"></param>
    ''' <returns></returns>
    Function UpdateParam2(tableName As String, whereCondition As String, Optional params As List(Of SqlParameter) = Nothing, Optional queryFromViewName As String = "") As DataRow
        Dim dt As DataTable = UpdateParam3(tableName, whereCondition, params, queryFromViewName)
        If dt IsNot Nothing Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0)
            End If
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Update table with given parameter and return actual data of affected rows
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="whereCondition">WHERE ID = 1</param>
    ''' <param name="params"></param>
    ''' <param name="queryFromViewName"></param>
    ''' <returns></returns>
    Function UpdateParam3(tableName As String, whereCondition As String, Optional params As List(Of SqlParameter) = Nothing, Optional queryFromViewName As String = "") As DataTable
        If UpdateParam(tableName, whereCondition, params) > 0 Then
            If queryFromViewName.Trim.Length <> 0 Then tableName = queryFromViewName
            Return ExecuteToDatatable($"SELECT * FROM [{tableName}] {whereCondition}")
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Usage: columnDataExists("", New SqlParameter("columnName", "value"), 0)
    ''' </summary>
    ''' <param name="table">table name</param>
    ''' <param name="keypair">New SqlParameter("columnName", "value")</param>
    ''' <param name="recId">id</param>
    ''' <returns></returns>
    Function columnDataExists(table As String, keypair As SqlParameter, Optional recId As Integer = 0) As Boolean
        ' check if location exists
        Dim p1 As New List(Of SqlParameter)
        p1.Add(New SqlParameter("", keypair.Value))
        p1.Add(New SqlParameter("", recId))
        Dim exists As Integer = 0
        exists = ExecuteScalar(String.Format("SELECT count(*) FROM {0} WHERE {1} LIKE ? AND id <> ?", {table, keypair.ParameterName}), p1)
        Return IIf(exists > 0, True, False)
    End Function

    ''' <summary>
    ''' Returns list of Tables (tablename, sql)
    ''' </summary>
    ''' <returns>array(TABLE_NAME, TABLE_TYPE)</returns>
    Function GetTables() As List(Of String())
        Dim lst As New List(Of String())
        Dim dt As DataTable = ExecuteToDatatable("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'")
        If dt IsNot Nothing Then
            For i = 0 To dt.Rows.Count - 1
                lst.Add({dt.Rows(i)("TABLE_NAME"), dt.Rows(i)("TABLE_TYPE")})
            Next
        End If
        Return lst
    End Function

    ''' <summary>
    ''' Return list of table columns (name, type)
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <returns>COLUMN_NAME, DATA_TYPE</returns>
    Function GetTableColumns(tableName As String) As List(Of String())
        Dim lst As New List(Of String())
        Dim dt As DataTable = ExecuteToDatatable("select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" & tableName & "'")
        If dt IsNot Nothing Then
            For i = 0 To dt.Rows.Count - 1
                lst.Add({dt.Rows(i)("COLUMN_NAME"),
                        dt.Rows(i)("DATA_TYPE")})
            Next
        End If
        Return lst
    End Function

    ''' <summary>
    ''' Return list of table column (name)
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <returns></returns>
    Function GetTableColumnName(tableName As String) As List(Of String)
        Dim lst As New List(Of String)
        Dim dt As DataTable = ExecuteToDatatable("select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" & tableName & "'")
        If dt IsNot Nothing Then
            For i = 0 To dt.Rows.Count - 1
                lst.Add(dt.Rows(i)("COLUMN_NAME"))
            Next
        End If
        Return lst
    End Function

    ''' <summary>
    ''' Check if table/view exists
    ''' </summary>
    ''' <param name="tbl_name"></param>
    ''' <returns></returns>
    Function IsTableExisting(tbl_name As String) As Boolean
        Dim exists As DataRow = Nothing
        Dim param As New List(Of SqlParameter)
        param.Add(New SqlParameter("@tbl_name", tbl_name))
        exists = QuerySingleResult("select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@tbl_name", param)
        Return (exists IsNot Nothing)
    End Function

    ''' <summary>
    ''' Create table from given column and type (if not exists)
    ''' </summary>
    ''' <param name="columnAndType">columnName Type(bigint, int, float, numeric, image, datetime, nvarchar(n))</param>
    ''' <param name="tableName"></param>
    ''' <returns></returns>
    Function CreateTable(columnAndType As String(), tableName As String) As String
        Dim anyStr As New System.Text.StringBuilder
        anyStr.Append("id                     bigint IDENTITY(1, 1)")

        For i = 0 To columnAndType.Count - 1
            If anyStr.Length <> 0 Then anyStr.Append(", ") : anyStr.AppendLine()
            anyStr.Append(columnAndType(i))
        Next

        Dim sql1 As New StringBuilder
        sql1.AppendLine("IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '" & tableName & "')")
        sql1.AppendLine("BEGIN")
        sql1.AppendLine($"CREATE TABLE [{tableName}] (")
        sql1.AppendLine(anyStr.ToString)
        sql1.AppendLine(", CONSTRAINT PK_" & RegularExpressions.Regex.Replace(tableName, "[^a-z0-9]", "_").ToUpper & " PRIMARY KEY (id)")
        sql1.AppendLine(");")
        sql1.AppendLine("END")

        ExecuteNonQuery(sql1.ToString)

        Return sql1.ToString

    End Function

    ''' <summary>
    ''' Create table from given class object (if not exists).
    ''' supported types: integer/long, boolean, double/decimal, datetime, byte(), string
    ''' Any additional column will be added if not existing
    ''' <code>
    ''' DBX_SERVER.CreateTableFrom(Of itemData)(itemData._tableName, nothing, nothing)
    ''' </code>
    ''' </summary>
    ''' <typeparam name="t"></typeparam>
    ''' <param name="tableName">table name</param>
    ''' <param name="uniqueColumns">list of unique columns (each)</param>
    ''' <param name="constraintColumns">combined unique column (as a whole)</param>
    ''' <param name="searchColumns">index column for fast search (each)</param>
    ''' <returns></returns>
    Function CreateTableFrom(Of t)(tableName As String, uniqueColumns As IEnumerable(Of String), constraintColumns As IEnumerable(Of String), searchColumns As IEnumerable(Of String)) As String
        Dim anyStr As New System.Text.StringBuilder
        Dim preSql As String = ""

        ' compare table columns to current
        Dim tbl1 As List(Of String) = GetTableColumnName(tableName)
        Dim sql1 As New StringBuilder
        Dim PK As String = RegularExpressions.Regex.Replace(tableName, "[^a-z0-9]", "_")
        Dim pds = GetType(t).GetProperties
        Dim lstIndexed As New List(Of String)

        If IsNothing(uniqueColumns) = False AndAlso uniqueColumns.Count > 0 Then lstIndexed = uniqueColumns.Select(Function(x) x.Trim.ToLower).Where(Function(x) x.Length > 0).ToList

        ' not existsing
        If tbl1.Count = 0 Then
            Dim s1 As New List(Of String)
            sql1 = New StringBuilder

            s1.Add("id".PadRight(30) & " bigint IDENTITY(1, 1) PRIMARY KEY")

            For Each prop In pds
                If prop.Name.StartsWith("_") Then Continue For

                Dim TP = prop.PropertyType
                If prop.PropertyType.Name.Contains("Null") Then TP = Nullable.GetUnderlyingType(prop.PropertyType)

                If TP Is GetType(Net.CookieContainer) Then Continue For

                Dim columnName As String = prop.Name.PadRight(30)
                Dim constraint As String = ""
                If lstIndexed.Contains(prop.Name.ToLower) Then
                    constraint = "NOT NULL UNIQUE"
                End If

                If TP Is GetType(Integer) Or
                    TP Is GetType(Long) Then

                    s1.Add($"{columnName} int {constraint}")

                ElseIf TP Is GetType(Boolean) Then

                    s1.Add($"{columnName} bit DEFAULT 0")

                ElseIf TP Is GetType(Double) Or
                    TP Is GetType(Decimal) Then

                    s1.Add($"{columnName} float")

                ElseIf TP Is GetType(DateTime) Then

                    s1.Add($"{columnName} datetime")

                ElseIf TP Is GetType(Byte()) Then

                    s1.Add($"{columnName} image")

                Else
                    If prop.Name.ToLower.Contains("refurl") Or prop.Name.ToLower.EndsWith("url") Then
                        s1.Add($"{columnName} nvarchar(2083)")

                    ElseIf prop.Name.ToLower.Contains("note") Or
                        prop.Name.ToLower.Contains("html") Or
                        prop.Name.ToLower.Contains("image") Or
                        prop.Name.ToLower.Contains("description") Or
                        prop.Name.ToLower.Contains("json") Then
                        s1.Add($"{columnName} nvarchar(max)")

                    Else
                        s1.Add($"{columnName} nvarchar(255) {constraint}")

                    End If
                End If
            Next

            If IsNothing(constraintColumns) = False AndAlso constraintColumns.Count > 0 Then
                s1.Add($"CONSTRAINT UC_IDX_{PK} UNIQUE ({String.Join(", ", constraintColumns)}) ")
            End If

            'sql1.AppendLine("IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '" & tableName & "')")
            'sql1.AppendLine("BEGIN")
            sql1.AppendLine($"CREATE TABLE [{tableName}] (")
            sql1.AppendLine(String.Join(", " & vbCrLf, s1.Select(Function(x) x.Trim).ToList))
            sql1.AppendLine(");")

            ' create search index
            If IsNothing(searchColumns) = False AndAlso searchColumns.Count > 0 Then
                For i = 0 To searchColumns.Count - 1
                    sql1.AppendLine($"CREATE INDEX [SC_IDX_{searchColumns(i).ToUpper}] ON [{tableName}] ({searchColumns(i)}); ")
                Next
            End If

            sql1.AppendLine($"
            -- used for query
            DROP VIEW IF EXISTS [qry_{tableName}];

            CREATE VIEW [qry_{tableName}] AS
            SELECT ROW_NUMBER() OVER (ORDER BY id) AS [RowNum], *
            FROM [{tableName}]")

            If sql1.Length <> 0 Then
                Return sql1.ToString
            End If

        Else
            Dim l2 As New List(Of String)
            ' update column if different
            For Each prop In pds
                If prop.Name.StartsWith("_") Then Continue For

                Dim TP = prop.PropertyType
                If prop.PropertyType.Name.Contains("Null") Then TP = Nullable.GetUnderlyingType(prop.PropertyType)

                If TP Is GetType(Net.CookieContainer) Then Continue For

                ' check if already exists
                If tbl1.Contains(prop.Name) Then Continue For

                Dim columnName As String = prop.Name.PadRight(30)
                Dim constraint As String = ""
                If lstIndexed.Contains(prop.Name.ToLower) Then
                    constraint = "NOT NULL UNIQUE"
                End If

                anyStr.Clear()
                anyStr.Append($"ALTER TABLE [{tableName}]").AppendLine()

                If TP Is GetType(Integer) Or
                    TP Is GetType(Long) Then

                    anyStr.Append($"ADD {columnName} int {constraint}")

                ElseIf TP Is GetType(Boolean) Then

                    anyStr.Append($"ADD {columnName} bit DEFAULT 0")

                ElseIf TP Is GetType(Double) Or
                    TP Is GetType(Decimal) Then

                    anyStr.Append($"ADD {columnName} float")

                ElseIf TP Is GetType(DateTime) Then

                    anyStr.Append($"ADD {columnName} datetime")

                ElseIf TP Is GetType(Byte()) Then

                    anyStr.Append($"ADD {columnName} image")

                Else
                    If prop.Name.ToLower.Contains("refurl") Or prop.Name.ToLower.EndsWith("url") Then

                        anyStr.Append($"ADD {columnName} nvarchar(2083)")

                    ElseIf prop.Name.ToLower.Contains("note") Or
                        prop.Name.ToLower.Contains("image") Or
                        prop.Name.ToLower.Contains("html") Or
                        prop.Name.ToLower.Contains("description") Or
                        prop.Name.ToLower.Contains("json") Then

                        anyStr.Append($"ADD {columnName} nvarchar(max)")

                    Else
                        anyStr.Append($"ADD {columnName} nvarchar(255) {constraint}")
                    End If
                End If

                If anyStr.Length <> 0 Then
                    l2.Add(anyStr.ToString)
                End If

            Next

            Return String.Join(vbCrLf, l2)

        End If

        'If constraintColumns.Count > 0 Then
        '    Try
        '        If uniqueIndex Then
        '            Dim sqlIndexName As String = PK & "_idx_" & indexedColumn
        '            Dim sqlIndex As String = "CREATE UNIQUE INDEX " & sqlIndexName & " ON " & tableName & " (" & indexedColumn & ")"
        '            Dim sql As String = <![CDATA[
        '                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'MyTableIndex' AND object_id = OBJECT_ID('tablename'))
        '                    BEGIN
        '                        SQLINDEX
        '                    END

        '                ]]>.Value.Replace("MyTableIndex", sqlIndexName).Replace("tablename", tableName).Replace("SQLINDEX", sqlIndex)

        '            ExecuteNonQuery(sql)
        '        Else

        '            Dim sqlIndexName As String = PK & "_idx_" & indexedColumn
        '            Dim sqlIndex As String = "CREATE INDEX " & sqlIndexName & " ON " & tableName & " (" & indexedColumn & ")"
        '            Dim sql As String = <![CDATA[
        '                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'MyTableIndex' AND object_id = OBJECT_ID('tablename'))
        '                    BEGIN
        '                        SQLINDEX
        '                    END

        '                ]]>.Value.Replace("MyTableIndex", sqlIndexName).Replace("tablename", tableName).Replace("SQLINDEX", sqlIndex)

        '            ExecuteNonQuery(sql)
        '        End If
        '    Catch ex As Exception
        '        Debug.Print(ex.Message)
        '    End Try
        'End If

        Return sql1.ToString
    End Function

    ''' <summary>
    ''' Recreate table
    ''' <code>
    ''' DBX_SERVER.ReCreateTableFrom(Of itemData)(itemData._tableName, nothing, nothing)
    ''' DBX_SERVER.ReCreateTableFrom(Of itemData)(itemData._tableName, {}, {"firstname", "lastname"})
    ''' </code>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="tableName"></param>
    ''' <param name="uniqueColumns">list of unique columns (each)</param>
    ''' <param name="constraintColumns">combined unique column (as a whole)</param>
    ''' <param name="searchColumns">index column for fast search (each)</param>
    ''' <returns></returns>
    Function ReCreateTableFrom(Of T)(tableName As String, uniqueColumns As IEnumerable(Of String), constraintColumns As IEnumerable(Of String), searchColumns As IEnumerable(Of String)) As String
        Dim anyStr As New System.Text.StringBuilder
        Dim preSql As String = ""

        ' compare table columns to current
        Dim tbl1 As List(Of String) = GetTableColumnName(tableName)
        Dim isNew As Boolean = tbl1.Count = 0
        Dim sql1 As New StringBuilder
        Dim tmpTableName As String = $"{tableName}_TEMP"
        Dim lst1 As New List(Of String) ' insert into
        Dim PK As String = RegularExpressions.Regex.Replace(tmpTableName, "[^a-z0-9]", "_", RegularExpressions.RegexOptions.IgnoreCase)
        Dim PK0 As String = RegularExpressions.Regex.Replace(tableName, "[^a-z0-9]", "_", RegularExpressions.RegexOptions.IgnoreCase)

        Dim pds = GetType(T).GetProperties
        Dim lstIndexed As New List(Of String)

        If IsNothing(uniqueColumns) = False AndAlso uniqueColumns.Count > 0 Then lstIndexed = uniqueColumns.Select(Function(x) x.Trim.ToLower).Where(Function(x) x.Length > 0).ToList

        Dim colNames As New List(Of String)
        Dim s1 As New List(Of String)
        sql1 = New StringBuilder

        s1.Add("id".PadRight(30) & " bigint IDENTITY(1, 1) PRIMARY KEY")

        For Each prop In pds
            If prop.Name.StartsWith("_") Then Continue For

            Dim TP = prop.PropertyType
            If prop.PropertyType.Name.Contains("Null") Then TP = Nullable.GetUnderlyingType(prop.PropertyType)

            If TP Is GetType(Net.CookieContainer) Then Continue For

            Dim columnName As String = prop.Name.PadRight(30)
            Dim constraint As String = ""
            If lstIndexed.Contains(prop.Name.ToLower) Then
                constraint = "NOT NULL UNIQUE"
            End If

            ' add if existing
            If tbl1.Contains(prop.Name) Or isNew = True Then colNames.Add(prop.Name)

            If TP Is GetType(Integer) Or
                    TP Is GetType(Long) Then

                s1.Add($"{columnName} int {constraint}")

            ElseIf TP Is GetType(Boolean) Then

                s1.Add($"{columnName} bit DEFAULT 0")

            ElseIf TP Is GetType(Double) Or
                    TP Is GetType(Decimal) Then

                s1.Add($"{columnName} float")

            ElseIf TP Is GetType(DateTime) Then

                s1.Add($"{columnName} datetime")

            ElseIf TP Is GetType(Byte()) Then

                s1.Add($"{columnName} image")

            Else
                If prop.Name.ToLower.Contains("refurl") Or prop.Name.ToLower.EndsWith("url") Then
                    s1.Add($"{columnName} nvarchar(2083)")

                ElseIf prop.Name.ToLower.Contains("note") Or
                        prop.Name.ToLower.Contains("image") Or
                        prop.Name.ToLower.Contains("html") Or
                        prop.Name.ToLower.Contains("description") Or
                        prop.Name.ToLower.Contains("json") Then

                    s1.Add($"{columnName} nvarchar(max)")

                Else
                    s1.Add($"{columnName} nvarchar(255) {constraint}")

                End If
            End If
        Next

        If IsNothing(constraintColumns) = False AndAlso constraintColumns.Count > 0 Then
            s1.Add($"CONSTRAINT UC_IDX_{PK} UNIQUE ({String.Join(", ", constraintColumns)}) ")
        End If

        sql1.AppendLine($"CREATE TABLE [{tmpTableName}] (")
        sql1.AppendLine(String.Join(", " & vbCrLf, s1.Select(Function(x) x.Trim).ToList))
        sql1.AppendLine(");")

        Dim l1 As New List(Of String)
        ' drop temp table if exists
        l1.Add($"DROP TABLE IF EXISTS [{tmpTableName}];")
        l1.Add("")
        ' create temp table
        l1.Add(sql1.ToString)
        l1.Add("")

        ' insert to temp table and drop
        l1.Add($"INSERT INTO [{tmpTableName}] ({String.Join(",", colNames)}) " & vbCrLf & $"     SELECT {String.Join(", ", colNames)} FROM [{tableName}];")
        l1.Add("")

        l1.Add($"DROP TABLE IF EXISTS [{tableName}];")
        l1.Add("")

        ' rename
        l1.Add($"exec sp_rename [{tmpTableName}], [{tableName}];")
        l1.Add("")

        If IsNothing(constraintColumns) = False AndAlso constraintColumns.Count > 0 Then
            l1.Add($"exec sp_rename [UC_IDX_{PK}], [UC_IDX_{PK0}];")
            l1.Add("")

        End If

        ' create search index
        If IsNothing(searchColumns) = False AndAlso searchColumns.Count > 0 Then
            For i = 0 To searchColumns.Count - 1
                l1.Add($"CREATE INDEX [SC_IDX_{searchColumns(i).ToUpper}] ON [{tableName}] ({searchColumns(i)}); ")
            Next
        End If

        Dim sq = String.Join(vbCrLf & vbCrLf, l1)

        Return String.Join(vbCrLf, l1)
    End Function

    Function TransactionBegin() As cSQLServer
        ExecuteNonQuery("BEGIN TRANSACTION")
        Return Me
    End Function
    Function TransactionCommit() As cSQLServer
        ExecuteNonQuery("COMMIT TRANSACTION")
        Return Me
    End Function
    Function TransactionRollback() As cSQLServer
        ExecuteNonQuery("ROLLBACK TRANSACTION")
        Return Me
    End Function


    ''' <summary>
    ''' Execute query and return a single row
    ''' <code>
    ''' exists = DBX_SERVER.QuerySingleResult(_tableName, "UID", UID, "id")
    ''' If IsNothing(exists) = False Then existsId = exists("id")
    ''' </code>
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="key"></param>
    ''' <param name="value"></param>
    ''' <param name="columns">*</param>
    ''' <returns></returns>
    Function QuerySingleResult(tableName As String, key As String, value As String, Optional columns As String = "*") As DataRow
        Dim sql As String = $"SELECT {columns} FROM [{tableName}] WHERE [{key}]=@{key}"
        Dim dt As DataTable = ExecuteToDatatable(sql, New Dictionary(Of String, Object) From {{$"@{key}", value}})
        If dt IsNot Nothing Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0)
            End If
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Insert given paramters to table and return last inserted id
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="columnData"></param>
    ''' <param name="ReturnLastRowID"></param>
    ''' <returns>0 - Good, -1 Not Good, other is last row id</returns>
    Function InsertParam(tableName As String, Optional columnData As Dictionary(Of String, Object) = Nothing, Optional ReturnLastRowID As Boolean = True) As Long
        Dim params As New Dictionary(Of String, Object)
        Dim query As New List(Of String)

        'without @
        Dim col1 As List(Of String) = columnData.Keys.Select(Function(x) $"[{IIf(x.StartsWith("@"), x.Substring(1), x).ToString()}]").ToList
        'with @
        Dim col2 As New List(Of String)

        For i = 0 To columnData.Keys.Count - 1
            Dim k = IIf(columnData.Keys(i).StartsWith("@"), columnData.Keys(i).Substring(1), columnData.Keys(i)).ToString()
            col2.Add($"@P{i}")
            params.Add($"@P{i}", columnData.ElementAt(i).Value)
        Next

        query.Add($"INSERT INTO [{tableName}] ")
        query.Add($"({String.Join(", ", col1)}) ")
        query.Add($"VALUES ({String.Join(", ", col2)}); ")
        query.Add($"")
        If ReturnLastRowID Then
            query.Add($"SELECT SCOPE_IDENTITY(); ")
        Else
            query.Add($"SELECT 1; ")
        End If

        Return ExecuteScalar(String.Join(vbCrLf, query), params)
    End Function

    ''' <summary>
    ''' Update table with given parameter and condition and return number of affected rows
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="whereCondition">WHERE ID = 1</param>
    ''' <param name="columnData"></param>
    ''' <returns></returns>
    Function UpdateParam(tableName As String, whereCondition As String, Optional columnData As Dictionary(Of String, Object) = Nothing) As Long
        Dim query As New List(Of String)
        Dim params As New Dictionary(Of String, Object)

        Dim col1 As New List(Of String)

        For i = 0 To columnData.Keys.Count - 1
            Dim k = IIf(columnData.Keys(i).StartsWith("@"), columnData.Keys(i).Substring(1), columnData.Keys(i)).ToString()
            col1.Add($"[{k}]=@P{i}")
            params.Add($"@P{i}", columnData.ElementAt(i).Value)
        Next

        query.Add($"UPDATE [{tableName}]")
        query.Add($"SET {String.Join($", {vbCrLf}  ", col1)} ")
        query.Add($"{whereCondition}")

        Return ExecuteNonQuery(String.Join(vbCrLf, query), params)
    End Function

    ''' <summary>
    ''' Insert given parameters to table and return inserted data (as datarow)
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="columnData"></param>
    ''' <param name="queryFromViewName"></param>
    ''' <returns></returns>
    Function InsertParam2(tableName As String, Optional columnData As Dictionary(Of String, Object) = Nothing, Optional queryFromViewName As String = "") As DataRow
        Dim lastId As Long = InsertParam(tableName, columnData)
        If lastId = -1 Then
            Return Nothing
        End If
        If queryFromViewName.Trim.Length <> 0 Then tableName = queryFromViewName
        Return QuerySingleResult($"SELECT * FROM {tableName} WHERE {tableName}.id = {lastId}")
    End Function

    ''' <summary>
    ''' Update table with given parameter and return single data
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="whereCondition">WHERE ID = 1</param>
    ''' <param name="columnData"></param>
    ''' <param name="queryFromViewName"></param>
    ''' <returns></returns>
    Function UpdateParam2(tableName As String, whereCondition As String, Optional columnData As Dictionary(Of String, Object) = Nothing, Optional queryFromViewName As String = "") As DataRow
        Dim params = columnData.ToSQLServerParameter
        Dim dt As DataTable = UpdateParam3(tableName, whereCondition, params, queryFromViewName)
        If dt IsNot Nothing Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0)
            End If
        End If
        Return Nothing
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If m_con IsNot Nothing Then m_con.Close()
            End If
            If m_con IsNot Nothing Then
                m_con.Dispose()
            End If
            m_con = Nothing
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

Module cSQLServerMod

    <Runtime.CompilerServices.Extension()>
    Function ToSQLServerParameter(dic As Dictionary(Of String, Object)) As List(Of SqlParameter)
        Dim p As New List(Of SqlParameter)
        For i = 0 To dic.Keys.Count - 1
            p.Add(New SqlParameter(dic.ElementAt(i).Key, dic(dic.ElementAt(i).Key)))
        Next
        Return p
    End Function

End Module