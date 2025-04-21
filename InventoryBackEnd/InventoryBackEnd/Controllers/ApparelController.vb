Imports System.Net
Imports System.Net.Http
Imports System.Security.Claims
Imports System.Web.Http
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json


Namespace Controllers
    <RoutePrefix("API/Apparel")>
    Public Class ApparelController
        Inherits ApiController
        Dim identity As New ClaimsIdentity(User.Identity)
        Private response As HttpResponseMessage
        Dim ds As DataSet = New DataSet

        'SHOW ALL AVAILABLE APPAREL

        <Route("Available/stock", Name:="Get_apparel_stock")>
        Public Function Get_apparel_stock(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Apparel_stock) = New List(Of Apparel_stock)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM apparel_stock
                                                            WHERE Campus_name = '" & Campus_name & "'", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Apparel_stock
                                        With dataObj
                                            .Apparel_ID = dtReader("Apparel_ID").ToString()
                                            .Grade_level = dtReader("Grade_level").ToString()
                                            .Apparel_type = dtReader("Apparel_type").ToString()
                                            .Size = dtReader("Size").ToString()
                                            .Quantity = dtReader("Quantity").ToString()
                                            .Transaction = dtReader("Transaction").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                        End With
                                        stats.Add(dataObj)
                                    End While
                                    Return Ok(stats)
                                Else
                                    Return NotFound()
                                End If
                                dtReader.Close()
                            End Using
                            msqlcom.Dispose()
                        End Using
                        sqlConn.Close()
                    Catch ex As Exception
                        Return InternalServerError(ex)
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'ADD APPARELL AVAILABLE

        <Route("ADD/Apparelavailable", Name:="Post_add_uniform")>
        Public Function Post_add_uniform(ByVal Grade_level As String, ByVal Apparel_type As String, ByVal Size As String, ByVal Quantity As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Check if all required parameters are provided
            If String.IsNullOrEmpty(Grade_level) OrElse String.IsNullOrEmpty(Apparel_type) OrElse String.IsNullOrEmpty(Size) OrElse String.IsNullOrEmpty(Quantity) OrElse String.IsNullOrEmpty(Campus_name) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Please provide all required parameters (Grade_level, Apparel_type, Size, Quantity, Campus_name).", Encoding.UTF8)
                Return response
            End If

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        ' Check if the record already exists with the same Grade_level, Apparel_type, Size, and Campus_name
                        Using cmd As New MySqlCommand()
                            cmd.Connection = sqlConn
                            cmd.CommandText = "SELECT COUNT(*) FROM apparel_stock WHERE Grade_level = @Grade_level AND Apparel_type = @Apparel_type AND Size = @Size AND Campus_name = @Campus_name"

                            cmd.Parameters.AddWithValue("@Grade_level", Grade_level)
                            cmd.Parameters.AddWithValue("@Apparel_type", Apparel_type)
                            cmd.Parameters.AddWithValue("@Size", Size)
                            cmd.Parameters.AddWithValue("@Campus_name", Campus_name)

                            sqlConn.Open()
                            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                            If count > 0 Then
                                ' The record already exists, return an error response
                                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                                response.Content = New StringContent("Error: The apparel you entered already exists.", Encoding.UTF8)
                                Return response
                            End If
                        End Using



                        ' Insert data into the apparel_stock table
                        Using insertCmd As New MySqlCommand()
                            insertCmd.Connection = sqlConn
                            insertCmd.CommandText = "INSERT INTO apparel_stock(Grade_level, Apparel_type, Size, Quantity, Campus_name) VALUES (@Grade_level, @Apparel_type, @Size, @Quantity, @Campus_name)"

                            insertCmd.Parameters.AddWithValue("@Grade_level", Grade_level)
                            insertCmd.Parameters.AddWithValue("@Apparel_type", Apparel_type)
                            insertCmd.Parameters.AddWithValue("@Size", Size)
                            insertCmd.Parameters.AddWithValue("@Quantity", Quantity)
                            insertCmd.Parameters.AddWithValue("@Campus_name", Campus_name)

                            insertCmd.ExecuteNonQuery()
                        End Using

                        ' Insert data into the apparel_logs table
                        Using logCmd As New MySqlCommand()
                            logCmd.Connection = sqlConn
                            logCmd.CommandText = "INSERT INTO apparel_logs(Grade_level, Apparel_type, Size, Transaction, Transaction_date, Quantity, Campus_name) VALUES (@Grade_level, @Apparel_type, @Size, @Transaction, @Transaction_date, @Quantity, @Campus_name )"

                            logCmd.Parameters.AddWithValue("@grade_level", Grade_level)
                            logCmd.Parameters.AddWithValue("@Apparel_type", Apparel_type)
                            logCmd.Parameters.AddWithValue("@Size", Size)
                            logCmd.Parameters.AddWithValue("@Transaction", "ADD RECORDS")
                            logCmd.Parameters.AddWithValue("@Quantity", Quantity)
                            logCmd.Parameters.AddWithValue("@Campus_name", Campus_name)
                            logCmd.Parameters.AddWithValue("@Transaction_date", DateTime.Now) ' Use the current date

                            logCmd.ExecuteNonQuery()
                        End Using

                        response = Request.CreateResponse(HttpStatusCode.OK)
                        response.Content = New StringContent("Successfully Created", Encoding.UTF8)
                        Return response
                    Catch ex As Exception
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                        response.Content = New StringContent("Application Error: There is an error in performing this action, " & ex.ToString(), Encoding.UTF8)
                        Return response
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                    response.Content = New StringContent("Server Error: Unable to connect to the database server", Encoding.UTF8)
                    Return response
                End If
            End Using
        End Function



        ' ADD QUANTITY
        <Route("ADD/STOCK", Name:="Post_update_stock_in")>
        Public Function Post_update_stock_in(ByVal Apparel_ID As Integer, ByVal stockChange As Integer) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()

                        Using cmd As New MySqlCommand()
                            cmd.Connection = sqlConn
                            cmd.CommandText = "UPDATE apparel_stock SET Quantity = Quantity + @stockChange WHERE Apparel_ID = @Apparel_ID"

                            cmd.Parameters.AddWithValue("@Apparel_ID", Apparel_ID)
                            cmd.Parameters.AddWithValue("@stockChange", stockChange)

                            Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                            If rowsAffected > 0 Then
                                ' Insert a record into the clothes_log table
                                Using insertCmd As New MySqlCommand()
                                    insertCmd.Connection = sqlConn
                                    insertCmd.CommandText = "INSERT INTO apparel_logs(Grade_level, Apparel_type, Size, Transaction, Transaction_date, Quantity, Campus_name) " &
                                            "SELECT Grade_level, Apparel_type, Size, 'ADD Quantity', @Transaction_date, @Quantity, Campus_name " &
                                            "FROM apparel_stock WHERE Apparel_ID = @Apparel_ID"

                                    insertCmd.Parameters.AddWithValue("@Apparel_ID", Apparel_ID)
                                    insertCmd.Parameters.AddWithValue("@quantity", stockChange)
                                    insertCmd.Parameters.AddWithValue("@transaction_date", DateTime.Now) ' Use the current date

                                    Dim insertRowsAffected As Integer = insertCmd.ExecuteNonQuery()

                                    If insertRowsAffected > 0 Then
                                        response = Request.CreateResponse(HttpStatusCode.OK)
                                        response.Content = New StringContent("Stock updated successfully (IN)", Encoding.UTF8)
                                    Else
                                        ' Failed to insert into clothes_log
                                        response = Request.CreateResponse(HttpStatusCode.BadRequest)
                                        response.Content = New StringContent("Failed to insert into clothes_log", Encoding.UTF8)
                                    End If
                                End Using
                            Else
                                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                                response.Content = New StringContent("Failed to update stock (IN)", Encoding.UTF8)
                            End If
                        End Using
                    Catch ex As Exception
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                        response.Content = New StringContent("Application Error (IN): There is an error in performing this action, " & ex.ToString(), Encoding.UTF8)
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                    response.Content = New StringContent("Server Error: Unable to connect to the database server", Encoding.UTF8)
                End If
            End Using

            Return response
        End Function

        'SHOW ALL GRADES

        <Route("Grade/list", Name:="Get_grade_list")>
        Public Function Get_grade_list(ByVal campus_name As String) As IHttpActionResult
            Dim stats As List(Of Grade_list) = New List(Of Grade_list)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM grade
                                                            WHERE Campus_name = '" & campus_name & "'", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Grade_list
                                        With dataObj
                                            .ID = dtReader("ID").ToString()
                                            .Grade_level = dtReader("Grade_level").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                        End With
                                        stats.Add(dataObj)
                                    End While
                                    Return Ok(stats)
                                Else
                                    Return NotFound()
                                End If
                                dtReader.Close()
                            End Using
                            msqlcom.Dispose()
                        End Using
                        sqlConn.Close()
                    Catch ex As Exception
                        Return InternalServerError(ex)
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'ADD GRADE LEVEL

        <Route("ADD/Grade", Name:="Post_add_gradelevel")>
        Public Function Post_add_gradelevel(ByVal Grade_level As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Check if Grade_level is empty or contains only spaces
            If String.IsNullOrWhiteSpace(Grade_level) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Grade level is not valid", Encoding.UTF8)
                Return response
            End If

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        ' Check if a record with the same grade_level and campus_name already exists
                        Using cmdCheck As New MySqlCommand()
                            cmdCheck.Connection = sqlConn
                            cmdCheck.CommandText = "SELECT COUNT(*) FROM grade WHERE Grade_level = @Grade_level AND Campus_name = @Campus_name"
                            cmdCheck.Parameters.AddWithValue("@Grade_level", Grade_level)
                            cmdCheck.Parameters.AddWithValue("@Campus_name", Campus_name)
                            Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                            If count > 0 Then
                                ' A record with the same grade_level and campus_name already exists, return an error message
                                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                                response.Content = New StringContent("Error: Grade already exists", Encoding.UTF8)
                                Return response
                            End If
                        End Using

                        ' If no record with the same grade_level and campus_name exists, proceed with the insertion
                        Using cmdInsert As New MySqlCommand()
                            cmdInsert.Connection = sqlConn
                            cmdInsert.CommandText = "INSERT INTO grade(Grade_level, Campus_name) VALUES (@Grade_level, @Campus_name)"
                            cmdInsert.Parameters.AddWithValue("@Grade_level", Grade_level)
                            cmdInsert.Parameters.AddWithValue("@Campus_name", Campus_name)
                            cmdInsert.ExecuteNonQuery()

                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Added Successfully", Encoding.UTF8)
                            Return response
                        End Using
                    Catch ex As Exception
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                        response.Content = New StringContent("Application Error: There is an error in performing this action, " & ex.ToString(), Encoding.UTF8)
                        Return response
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                    response.Content = New StringContent("Server Error: Unable to connect to the database server", Encoding.UTF8)
                    Return response
                End If
            End Using
        End Function


        'UPDATE Grade Level

        <Route("Modify/Gradelvl", Name:="Post_update_gradelvl")>
        Public Function Post_update_gradelvl(ByVal ID As String, ByVal Grade_level As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Check if Grade_level is empty or contains only spaces
            If String.IsNullOrWhiteSpace(Grade_level) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Grade level is not valid", Encoding.UTF8)
                Return response
            End If

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if the grade level already exists
                    Dim checkQuery As String = "SELECT COUNT(*) FROM grade WHERE Grade_level = @Grade_level AND Campus_name = @Campus_name AND ID <> @ID"
                    Using cmdCheck As New MySqlCommand(checkQuery, sqlConn)
                        cmdCheck.Parameters.AddWithValue("@ID", ID)
                        cmdCheck.Parameters.AddWithValue("@Grade_level", Grade_level)
                        cmdCheck.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                        If count > 0 Then
                            ' Grade level already exists, return error message
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Grade level already exists", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' Update the grade level if it doesn't exist
                    Dim updateQuery As String = "UPDATE grade SET Grade_level = @Grade_level, Campus_name = @Campus_name WHERE ID = @ID"
                    Using cmdUpdate As New MySqlCommand(updateQuery, sqlConn)
                        cmdUpdate.Parameters.AddWithValue("@ID", ID)
                        cmdUpdate.Parameters.AddWithValue("@Grade_level", Grade_level)
                        cmdUpdate.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim rowsAffected As Integer = cmdUpdate.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully updated Grade Level", Encoding.UTF8, "text/plain")
                        Else
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                            response.Content = New StringContent("Failed to update Grade Level", Encoding.UTF8)
                        End If
                    End Using
                End Using
            Catch dbEx As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & dbEx.Message, Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            Return response
        End Function


        'SHOW ALL APPAREL TYPE 

        <Route("Appareltype/list", Name:="Get_Apparel_list")>
        Public Function Get_clothes_list(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Apparel_type) = New List(Of Apparel_type)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM apparel_type
                                                            WHERE Campus_name = '" & Campus_name & "'", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Apparel_type
                                        With dataObj
                                            .Apparel_ID = dtReader("Apparel_ID").ToString()
                                            .Apparel_type = dtReader("Apparel_type").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                        End With
                                        stats.Add(dataObj)
                                    End While
                                    Return Ok(stats)
                                Else
                                    Return NotFound()
                                End If
                                dtReader.Close()
                            End Using
                            msqlcom.Dispose()
                        End Using
                        sqlConn.Close()
                    Catch ex As Exception
                        Return InternalServerError(ex)
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'ADD APPAREL TYPE

        <Route("ADD/Appareltypes", Name:="Post_add_Clothes_type")>
        Public Function Post_add_Clothes_type(ByVal Apparel_type As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Trim the Apparel_type to check for spaces only
            If String.IsNullOrWhiteSpace(Apparel_type) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Apparel type is not valid", Encoding.UTF8)
                Return response
            End If

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        ' Check if a record with the same Apparel_type and Campus_name already exists
                        Using cmdCheck As New MySqlCommand()
                            cmdCheck.Connection = sqlConn
                            cmdCheck.CommandText = "SELECT COUNT(*) FROM apparel_type WHERE Apparel_type = @Apparel_type AND Campus_name = @Campus_name"
                            cmdCheck.Parameters.AddWithValue("@Apparel_type", Apparel_type)
                            cmdCheck.Parameters.AddWithValue("@Campus_name", Campus_name)
                            Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                            If count > 0 Then
                                ' A record with the same Apparel_type and Campus_name already exists, return an error message
                                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                                response.Content = New StringContent("Error: Apparel Type already exists", Encoding.UTF8)
                                Return response
                            End If
                        End Using

                        ' If no record with the same Apparel_type and Campus_name exists, proceed with the insertion
                        Using cmdInsert As New MySqlCommand()
                            cmdInsert.Connection = sqlConn
                            cmdInsert.CommandText = "INSERT INTO apparel_type (Apparel_type, Campus_name) VALUES (@Apparel_type, @Campus_name)"
                            cmdInsert.Parameters.AddWithValue("@Apparel_type", Apparel_type)
                            cmdInsert.Parameters.AddWithValue("@Campus_name", Campus_name)
                            cmdInsert.ExecuteNonQuery()

                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Added Successfully", Encoding.UTF8)
                            Return response
                        End Using
                    Catch ex As Exception
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                        response.Content = New StringContent("Application Error: There is an error in performing this action, " & ex.ToString(), Encoding.UTF8)
                        Return response
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                    response.Content = New StringContent("Server Error: Unable to connect to the database server", Encoding.UTF8)
                    Return response
                End If
            End Using
        End Function


        'MODIFY APPAREL TYPE

        <Route("Modify/Apparel", Name:="Post_modify_apparel")>
        Public Function Post_modify_apparel(ByVal Apparel_ID As String, ByVal Apparel_type As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Check if Apparel_type is empty or contains only spaces
            If String.IsNullOrWhiteSpace(Apparel_type) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Apparel Type is required.", Encoding.UTF8)
                Return response
            End If

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        ' Check if the Apparel_type already exists for a different Apparel_ID
                        Dim count As Integer
                        Using checkCmd As New MySqlCommand("SELECT COUNT(*) FROM apparel_type WHERE Apparel_ID <> @Apparel_ID AND Apparel_type = @Apparel_type AND Campus_name = @Campus_name", sqlConn)
                            checkCmd.Parameters.AddWithValue("@Apparel_ID", Apparel_ID)
                            checkCmd.Parameters.AddWithValue("@Apparel_type", Apparel_type)
                            checkCmd.Parameters.AddWithValue("@Campus_name", Campus_name)

                            sqlConn.Open()
                            count = Convert.ToInt32(checkCmd.ExecuteScalar())
                        End Using

                        If count > 0 Then
                            ' The Apparel_type already exists for the given Campus_name, return an error response
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Error: Apparel Type already exists for the given campus.", Encoding.UTF8)
                            Return response
                        End If

                        ' Proceed with the update if no duplicate is found
                        Dim updateQuery As String = "UPDATE apparel_type SET Apparel_type = @Apparel_type, Campus_name = @Campus_name WHERE Apparel_ID = @Apparel_ID"
                        Using updateCmd As New MySqlCommand(updateQuery, sqlConn)
                            updateCmd.Parameters.AddWithValue("@Apparel_ID", Apparel_ID)
                            updateCmd.Parameters.AddWithValue("@Apparel_type", Apparel_type)
                            updateCmd.Parameters.AddWithValue("@Campus_name", Campus_name)

                            updateCmd.ExecuteNonQuery()

                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully Updated", Encoding.UTF8)
                            Return response
                        End Using
                    Catch ex As Exception
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                        response.Content = New StringContent("Application Error: There is an error in performing this action, " & ex.ToString(), Encoding.UTF8)
                        Return response
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                    response.Content = New StringContent("Server Error: Unable to connect to the database server", Encoding.UTF8)
                    Return response
                End If
            End Using
        End Function


        'SHOW ALL SIZE

        <Route("Size/list", Name:="Get_size_list")>
        Public Function Get_size_list(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Size_list) = New List(Of Size_list)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM size_list
                                                            WHERE Campus_name = '" & Campus_name & "'", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Size_list
                                        With dataObj
                                            .Size_ID = dtReader("Size_ID").ToString()
                                            .Size = dtReader("Size").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                        End With
                                        stats.Add(dataObj)
                                    End While
                                    Return Ok(stats)
                                Else
                                    Return NotFound()
                                End If
                                dtReader.Close()
                            End Using
                            msqlcom.Dispose()
                        End Using
                        sqlConn.Close()
                    Catch ex As Exception
                        Return InternalServerError(ex)
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'ADD SIZES

        <Route("add/Size", Name:="Post_add_sizes")>
        Public Function Post_add_sizes(ByVal Size As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Check if Size is empty or contains only spaces
            If String.IsNullOrWhiteSpace(Size) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Invalid Size", Encoding.UTF8)
                Return response
            End If

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()

                        ' Check if the size and campus_name already exist
                        Using cmdCheck As New MySqlCommand()
                            cmdCheck.Connection = sqlConn
                            cmdCheck.CommandText = "SELECT COUNT(*) FROM size_list WHERE Size = @Size AND Campus_name = @Campus_name"
                            cmdCheck.Parameters.AddWithValue("@Size", Size)
                            cmdCheck.Parameters.AddWithValue("@Campus_name", Campus_name)
                            Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                            If count > 0 Then
                                ' The size and campus_name already exist, return an error response
                                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                                response.Content = New StringContent("Error: Size already exists", Encoding.UTF8)
                            Else
                                ' The size and campus_name do not exist, proceed with the insertion
                                Using cmdInsert As New MySqlCommand()
                                    cmdInsert.Connection = sqlConn
                                    cmdInsert.CommandText = "INSERT INTO size_list (Size, Campus_name) VALUES (@Size, @Campus_name)"
                                    cmdInsert.Parameters.AddWithValue("@Size", Size)
                                    cmdInsert.Parameters.AddWithValue("@Campus_name", Campus_name)
                                    cmdInsert.ExecuteNonQuery()

                                    response = Request.CreateResponse(HttpStatusCode.OK)
                                    response.Content = New StringContent("Added Successfully", Encoding.UTF8)
                                End Using
                            End If
                        End Using
                    Catch ex As Exception
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                        response.Content = New StringContent("Application Error: There is an error in performing this action, " & ex.ToString(), Encoding.UTF8)
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                    response.Content = New StringContent("Server Error: Unable to connect to the database server", Encoding.UTF8)
                End If
            End Using

            Return response
        End Function


        <Route("Modify/Sizes", Name:="Post_update_sizes")>
        Public Function Post_update_sizes(ByVal Size_ID As String, ByVal Size As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Check if Size is empty or contains only spaces
            If String.IsNullOrWhiteSpace(Size) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Size is not valid", Encoding.UTF8)
                Return response
            End If

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if the size and campus_name already exist for another Size_ID
                    Dim checkQuery As String = "SELECT COUNT(*) FROM size_list WHERE Size = @Size AND Campus_name = @Campus_name AND Size_ID <> @Size_ID"
                    Using cmdCheck As New MySqlCommand(checkQuery, sqlConn)
                        cmdCheck.Parameters.AddWithValue("@Size_ID", Size_ID)
                        cmdCheck.Parameters.AddWithValue("@Size", Size)
                        cmdCheck.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                        If count > 0 Then
                            ' Size already exists, return error message
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Error: Size already exists", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' Update the size if it doesn't exist
                    Dim updateQuery As String = "UPDATE size_list SET Size = @Size, Campus_name = @Campus_name WHERE Size_ID = @Size_ID"
                    Using cmdUpdate As New MySqlCommand(updateQuery, sqlConn)
                        cmdUpdate.Parameters.AddWithValue("@Size_ID", Size_ID)
                        cmdUpdate.Parameters.AddWithValue("@Size", Size)
                        cmdUpdate.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim rowsAffected As Integer = cmdUpdate.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully updated", Encoding.UTF8, "text/plain")
                        Else
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                            response.Content = New StringContent("Failed to update", Encoding.UTF8)
                        End If
                    End Using
                End Using
            Catch dbEx As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & dbEx.Message, Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            Return response
        End Function



        'SHOW ALL APPAREL RECORDS

        <Route("uniformlogs/logs", Name:="Get_apparel_log")>
        Public Function Get_apparel_log(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Apparel_logs) = New List(Of Apparel_logs)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM apparel_logs
                                                            WHERE Campus_name = '" & Campus_name & "'", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Apparel_logs
                                        With dataObj
                                            .Apparel_type = dtReader("Apparel_type").ToString()
                                            .First_name = dtReader("First_name").ToString()
                                            .Last_name = dtReader("Last_name").ToString()
                                            .Grade_level = dtReader("Grade_level").ToString()
                                            .Size = dtReader("Size").ToString()
                                            .Transaction = dtReader("Transaction").ToString()
                                            .Quantity = dtReader("Quantity").ToString()
                                            .Transaction_date = dtReader("Transaction_date").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                            .Reciept_number = dtReader("Reciept_number").ToString()
                                        End With
                                        stats.Add(dataObj)
                                    End While
                                    Return Ok(stats)
                                Else
                                    Return NotFound()
                                End If
                                dtReader.Close()
                            End Using
                            msqlcom.Dispose()
                        End Using
                        sqlConn.Close()
                    Catch ex As Exception
                        Return InternalServerError(ex)
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'SHOW ALL CLAIM APPAREL

        <Route("Claim/Apparel", Name:="Get_claim_list")>
        Public Function Get_claim_list(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Claim_stub) = New List(Of Claim_stub)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM claim_stub
                                                            WHERE Campus_name = '" & Campus_name & "'", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Claim_stub
                                        With dataObj
                                            .Claim_ID = dtReader("Claim_ID").ToString()
                                            .Size = dtReader("Size").ToString()
                                            .Quantity = dtReader("Quantity").ToString()
                                            .Grade_level = dtReader("Grade_level").ToString()
                                            .Apparel_name = dtReader("Apparel_name").ToString()
                                            .First_name = dtReader("First_name").ToString()
                                            .Last_name = dtReader("Last_name").ToString()
                                            .Date_recieve = dtReader("Date_recieve").ToString()
                                            .Reciept_number = dtReader("Reciept_number").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                        End With
                                        stats.Add(dataObj)
                                    End While
                                    Return Ok(stats)
                                Else
                                    Return NotFound()
                                End If
                                dtReader.Close()
                            End Using
                            msqlcom.Dispose()
                        End Using
                        sqlConn.Close()
                    Catch ex As Exception
                        Return InternalServerError(ex)
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function




        'CLAIM PROCESS 

        <Route("add/claimitems", Name:="Post_add_claimitem")>
        Public Function Post_add_claimitem(ByVal First_name As String, ByVal Last_name As String, ByVal Apparel_name As String, ByVal Reciept_number As String, ByVal Date_receive As String, ByVal Campus_name As String, ByVal Grade_level As String, ByVal Quantity As Integer, ByVal Size As String) As HttpResponseMessage
            Dim response As HttpResponseMessage = Nothing

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()


                    Dim stockCheckCmd As New MySqlCommand("SELECT Quantity FROM apparel_stock WHERE Grade_level = @Grade_level AND Apparel_type = @Apparel_name AND Size = @Size", sqlConn)
                    stockCheckCmd.Parameters.AddWithValue("@Grade_level", Grade_level)
                    stockCheckCmd.Parameters.AddWithValue("@Apparel_name", Apparel_name)
                    stockCheckCmd.Parameters.AddWithValue("@Size", Size)
                    Dim stockQuantity As Integer = Convert.ToInt32(stockCheckCmd.ExecuteScalar())


                    If stockQuantity >= Quantity Then

                        Dim insertClaimCmd As New MySqlCommand("INSERT INTO claim_stub(Size, Quantity, Grade_level, Apparel_name, First_name, Last_name, Date_recieve, Reciept_number, Campus_name) VALUES (@Size, @Quantity, @Grade_level, @Apparel_name, @First_name, @Last_name, @Date_receive, @Reciept_number, @Campus_name)", sqlConn)
                        insertClaimCmd.Parameters.AddWithValue("@Size", Size)
                        insertClaimCmd.Parameters.AddWithValue("@Quantity", Quantity)
                        insertClaimCmd.Parameters.AddWithValue("@Grade_level", Grade_level)
                        insertClaimCmd.Parameters.AddWithValue("@Apparel_name", Apparel_name)
                        insertClaimCmd.Parameters.AddWithValue("@First_name", First_name)
                        insertClaimCmd.Parameters.AddWithValue("@Last_name", Last_name)
                        insertClaimCmd.Parameters.AddWithValue("@Date_receive", Date_receive)
                        insertClaimCmd.Parameters.AddWithValue("@Reciept_number", Reciept_number)
                        insertClaimCmd.Parameters.AddWithValue("@Campus_name", Campus_name)
                        insertClaimCmd.ExecuteNonQuery()

                        ' Insert into apparel_logs
                        Dim insertLogCmd As New MySqlCommand("INSERT INTO apparel_logs(Apparel_type, Last_name, First_name, Grade_level, Transaction, Size, Quantity, Transaction_date, Campus_name, Reciept_number) VALUES (@Apparel_name, @Last_name, @First_name, @Grade_level, 'CLAIM', @Size, @Quantity, @Date_receive, @Campus_name, @Reciept_number)", sqlConn)
                        insertLogCmd.Parameters.AddWithValue("@Apparel_name", Apparel_name)
                        insertLogCmd.Parameters.AddWithValue("@Last_name", Last_name)
                        insertLogCmd.Parameters.AddWithValue("@First_name", First_name)
                        insertLogCmd.Parameters.AddWithValue("@Grade_level", Grade_level)
                        insertLogCmd.Parameters.AddWithValue("@Size", Size)
                        insertLogCmd.Parameters.AddWithValue("@Quantity", Quantity)
                        insertLogCmd.Parameters.AddWithValue("@Date_receive", Date_receive)
                        insertLogCmd.Parameters.AddWithValue("@Campus_name", Campus_name)
                        insertLogCmd.Parameters.AddWithValue("@Reciept_number", Reciept_number)
                        insertLogCmd.ExecuteNonQuery()

                        ' Update stock quantity
                        Dim updateStockCmd As New MySqlCommand("UPDATE apparel_stock SET Quantity = Quantity - @Quantity WHERE Grade_level = @Grade_level AND Apparel_type = @Apparel_name AND Size = @Size", sqlConn)
                        updateStockCmd.Parameters.AddWithValue("@Quantity", Quantity)
                        updateStockCmd.Parameters.AddWithValue("@Grade_level", Grade_level)
                        updateStockCmd.Parameters.AddWithValue("@Apparel_name", Apparel_name)
                        updateStockCmd.Parameters.AddWithValue("@Size", Size)
                        updateStockCmd.ExecuteNonQuery()

                        ' Return success response
                        response = New HttpResponseMessage(HttpStatusCode.OK)
                        response.Content = New StringContent("Successfully Added", Encoding.UTF8)
                        Return response
                    Else
                        ' If insufficient stock, return error response
                        response = New HttpResponseMessage(HttpStatusCode.BadRequest)
                        response.Content = New StringContent("Insufficient Stock", Encoding.UTF8)
                        Return response
                    End If
                End Using
            Catch ex As MySqlException
                ' Handle database error
                response = New HttpResponseMessage(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & ex.Message, Encoding.UTF8)
            Catch ex As Exception
                ' Handle application error
                response = New HttpResponseMessage(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            ' Return response
            Return response
        End Function





    End Class
End Namespace