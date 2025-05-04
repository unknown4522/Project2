Imports System.Net
Imports System.Net.Http
Imports System.Security.Claims
Imports System.Web.Http
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Namespace Controllers
    <RoutePrefix("API/Inventory")>
    Public Class InventoryController
        Inherits ApiController
        Dim identity As New ClaimsIdentity(User.Identity)
        Private response As HttpResponseMessage
        Dim ds As DataSet = New DataSet




        'SHOW ALL CAMPUS LIST

        <Route("Campuses", Name:="Get_campus_list")>
        Public Function Get_campus_list() As IHttpActionResult
            Dim stats As List(Of Campuses) = New List(Of Campuses)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM campuses", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Campuses
                                        With dataObj
                                            .Campus_ID = dtReader("Campus_ID").ToString()
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


        'ADD Campus

        <Route("Add/Campus", Name:="Post_add_campus")>
        Public Function Post_add_campus(ByVal campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Trim and validate the input
            Dim trimmedCampusName As String = campus_name.Trim()

            If String.IsNullOrWhiteSpace(trimmedCampusName) OrElse trimmedCampusName.Length <= 1 OrElse IsNumeric(trimmedCampusName) Then
                ' Invalid input, return a bad request response
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Invalid campus name.", Encoding.UTF8)
                Return response
            End If

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if the campus name already exists
                    Using cmdCheck As New MySqlCommand()
                        cmdCheck.Connection = sqlConn
                        cmdCheck.CommandText = "SELECT COUNT(*) FROM campuses WHERE Campus_name = @Campus_name"
                        cmdCheck.Parameters.AddWithValue("@Campus_name", trimmedCampusName)
                        Dim campusCount As Integer = CInt(cmdCheck.ExecuteScalar())

                        If campusCount > 0 Then
                            ' Campus name already exists, return an error response
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Campus name already exists.", Encoding.UTF8)
                        Else
                            ' Campus name doesn't exist, proceed with the insertion
                            Using cmdInsert As New MySqlCommand()
                                cmdInsert.Connection = sqlConn
                                cmdInsert.CommandText = "INSERT INTO campuses (Campus_name) VALUES (@Campus_name)"
                                cmdInsert.Parameters.AddWithValue("@Campus_name", trimmedCampusName)
                                Dim rowsAffected = cmdInsert.ExecuteNonQuery()

                                If rowsAffected > 0 Then
                                    response = Request.CreateResponse(HttpStatusCode.OK)
                                    response.Content = New StringContent("Successfully added campus", Encoding.UTF8)
                                Else
                                    response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                                    response.Content = New StringContent("Failed to add campus", Encoding.UTF8)
                                End If
                            End Using
                        End If
                    End Using
                End Using
            Catch ex As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & ex.Message, Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            Return response
        End Function


        'MODIFY CAMPUS

        <Route("Modify/Campus", Name:="Post_update_campus")>
        Public Function Post_update_campus(ByVal Campus_id As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    If sqlConn.State = ConnectionState.Closed Then
                        sqlConn.Open()
                    End If

                    ' Check if the campus name already exists (excluding the current campus being updated)
                    Using checkCmd As New MySqlCommand
                        checkCmd.Connection = sqlConn
                        checkCmd.CommandText = "SELECT COUNT(*) FROM campuses WHERE Campus_name = @Campus_name AND Campus_ID <> @Campus_ID"
                        checkCmd.Parameters.Add("@Campus_name", MySqlDbType.Text).Value = Campus_name
                        checkCmd.Parameters.Add("@Campus_ID", MySqlDbType.Text).Value = Campus_id

                        Dim existingCampusCount As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                        If existingCampusCount > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Campus name is already in use", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' Proceed with the update if the campus name is not already in use
                    Using cmd As New MySqlCommand
                        cmd.Connection = sqlConn
                        cmd.CommandText = "UPDATE campuses SET Campus_name = @Campus_name WHERE Campus_ID = @Campus_ID"
                        cmd.Parameters.Add("@Campus_ID", MySqlDbType.Text).Value = Campus_id
                        cmd.Parameters.Add("@Campus_name", MySqlDbType.Text).Value = Campus_name

                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully Updated", Encoding.UTF8)
                        Else
                            response = Request.CreateResponse(HttpStatusCode.NotFound)
                            response.Content = New StringContent("Campus not found for updating", Encoding.UTF8)
                        End If
                    End Using
                End Using
            Catch ex As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & ex.Message, Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            Return response
        End Function




        'SHOW ALL BRANDS    

        <Route("Brand/List", Name:="Get_brand_list")>
        Public Function Get_brand_list(ByVal campus_name As String) As IHttpActionResult
            Dim stats As List(Of Brand_lIST) = New List(Of Brand_lIST)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM brand_list
                                                            WHERE Campus_name = '" & campus_name & "'", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Brand_lIST
                                        With dataObj
                                            .Brand_ID = dtReader("Brand_ID").ToString()
                                            .Brand_name = dtReader("Brand_name").ToString()
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


        'ADD BRAND 

        <Route("Add/Brand", Name:="Post_add_brand")>
        Public Function Post_add_brand(ByVal Brand_name As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Try

                If String.IsNullOrWhiteSpace(Brand_name) Then
                    response = Request.CreateResponse(HttpStatusCode.BadRequest)
                    response.Content = New StringContent("Invalid brand name. Please provide a valid brand name.", Encoding.UTF8)
                    Return response
                End If

                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()


                    Dim checkBrandQuery As String = "SELECT COUNT(*) FROM brand_list WHERE Brand_name = @Brand_name AND Campus_name = @Campus_name;"
                    Using cmdCheckBrand As New MySqlCommand(checkBrandQuery, sqlConn)
                        cmdCheckBrand.Parameters.AddWithValue("@Brand_name", Brand_name)
                        cmdCheckBrand.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim brandCount As Integer = Convert.ToInt32(cmdCheckBrand.ExecuteScalar())

                        If brandCount > 0 Then

                            response = Request.CreateResponse(HttpStatusCode.Conflict)
                            response.Content = New StringContent("Brand already exists.", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' If brand does not exist, proceed with the insertion
                    Dim insertQuery As String = "INSERT INTO brand_list (Brand_name, Campus_name) VALUES (@Brand_name, @Campus_name);"
                    Using cmdInsert As New MySqlCommand(insertQuery, sqlConn)
                        cmdInsert.Parameters.AddWithValue("@Brand_name", Brand_name)
                        cmdInsert.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim rowsAffected As Integer = cmdInsert.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully ADDED BRAND", Encoding.UTF8)
                        Else
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                            response.Content = New StringContent("Failed to ADD BRAND", Encoding.UTF8)
                        End If
                    End Using
                End Using
            Catch ex As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & ex.Message, Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            Return response
        End Function

        'MODIFY BRAND

        <Route("Modify/brand", Name:="Post_update_brand")>
        Public Function Post_update_brand(ByVal Brand_ID As String, ByVal Brand_name As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if the Brand_name already exists
                    Dim checkQuery As String = "SELECT COUNT(*) FROM brand_list WHERE Brand_name = @Brand_name AND Brand_ID <> @Brand_ID"
                    Using cmdCheck As New MySqlCommand(checkQuery, sqlConn)
                        cmdCheck.Parameters.AddWithValue("@Brand_name", Brand_name)
                        cmdCheck.Parameters.AddWithValue("@Brand_ID", Brand_ID)

                        Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                        If count > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Brand name is already used", Encoding.UTF8, "text/plain")
                            Return response
                        End If
                    End Using

                    ' Proceed with the update if Brand_name is unique
                    Dim sqlQuery As String = "UPDATE brand_list SET Brand_name = @Brand_name, Campus_name = @Campus_name WHERE Brand_ID = @Brand_ID"
                    Using cmdUpdate As New MySqlCommand(sqlQuery, sqlConn)
                        cmdUpdate.Parameters.AddWithValue("@Brand_ID", Brand_ID)
                        cmdUpdate.Parameters.AddWithValue("@Brand_name", Brand_name)
                        cmdUpdate.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim rowsAffected As Integer = cmdUpdate.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully UPDATED BRAND NAME", Encoding.UTF8, "text/plain")
                        Else
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                            response.Content = New StringContent("Failed to UPDATE BRAND NAME", Encoding.UTF8)
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



        'SHOW ALL ROOMS

        <Route("Rooms/List", Name:="Get_room_list")>
        Public Function Get_room_list(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Rooms) = New List(Of Rooms)
            Dim roomCount As Integer = 0

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        ' Count the number of rooms
                        Using countCommand As New MySqlCommand("SELECT COUNT(*) FROM rooms WHERE Campus_name = @Campus_name", sqlConn)
                            countCommand.Parameters.AddWithValue("@Campus_name", Campus_name)
                            roomCount = Convert.ToInt32(countCommand.ExecuteScalar())
                        End Using

                        ' Retrieve the rooms
                        Using msqlcom As New MySqlCommand("SELECT * FROM rooms WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Rooms
                                        With dataObj
                                            .ID = dtReader("ID").ToString()
                                            .Room_name = dtReader("Room_name").ToString()
                                            .Floor_level = dtReader("Floor_level").ToString()
                                            .Room_description = dtReader("Room_description").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                            .Counter = roomCount
                                        End With
                                        stats.Add(dataObj)
                                    End While

                                    Return Ok(stats)
                                Else
                                    Return NotFound()
                                End If
                            End Using
                        End Using
                    Catch ex As Exception
                        Return InternalServerError(ex)
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function


        'ADD ROOMS

        <Route("Add/Newroom", Name:="Post_add_room")>
        Public Function Post_add_room(ByVal Room_name As String, ByVal Floor_level As String, ByVal Room_description As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            If String.IsNullOrEmpty(Room_name) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Room name is required", Encoding.UTF8)
                Return response
            ElseIf String.IsNullOrEmpty(Campus_name) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Campus_name is required", Encoding.UTF8)
                Return response
            End If

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        ' Checking Room Existence
                        Dim checkExistenceQuery As String = "SELECT COUNT(*) FROM rooms WHERE Room_name = @Room_name AND Campus_name = @Campus_name;"

                        Using cmdCheckExistence As New MySqlCommand(checkExistenceQuery, sqlConn)
                            cmdCheckExistence.Parameters.AddWithValue("@Room_name", Room_name)
                            cmdCheckExistence.Parameters.AddWithValue("@Campus_name", Campus_name)

                            sqlConn.Open()

                            Dim roomCount As Integer = Convert.ToInt32(cmdCheckExistence.ExecuteScalar())

                            If roomCount > 0 Then
                                ' Room number already exists, return an error message
                                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                                response.Content = New StringContent("Error: Room Name already exists", Encoding.UTF8)
                                Return response
                            End If
                        End Using

                        ' Adding a New Room
                        Dim addRoomQuery As String = "INSERT INTO rooms (Room_name, Floor_level, Room_description, Campus_name) VALUES (@Room_name, @Floor_level, @Room_description, @Campus_name);"

                        Using cmdAddRoom As New MySqlCommand(addRoomQuery, sqlConn)
                            cmdAddRoom.Parameters.AddWithValue("@Room_name", Room_name)
                            cmdAddRoom.Parameters.AddWithValue("@Floor_level", Floor_level)
                            cmdAddRoom.Parameters.AddWithValue("@Room_description", Room_description)
                            cmdAddRoom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            cmdAddRoom.ExecuteNonQuery()

                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully Created", Encoding.UTF8)
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


        'MODIFY ROOM

        <Route("Modify/room", Name:="Post_update_rooms")>
        Public Function Post_update_rooms(ByVal ID As String, ByVal Room_name As String, ByVal Floor_level As String, ByVal Room_description As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Check if Room_name is empty or contains only spaces
            If String.IsNullOrWhiteSpace(Room_name) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Error: Room name is required.", Encoding.UTF8)
                Return response
            End If

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                Try
                    sqlConn.Open()

                    ' Check if the Room_name is already in use
                    Dim checkSql As String = "SELECT COUNT(*) FROM rooms WHERE Room_name = @Room_name AND ID != @ID"
                    Using checkCmd As New MySqlCommand(checkSql, sqlConn)
                        checkCmd.Parameters.AddWithValue("@Room_name", Room_name)
                        checkCmd.Parameters.AddWithValue("@ID", ID)
                        Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                        If count > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Error: Room name is already in use.", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' Update the room details
                    Using updateCmd As New MySqlCommand("UPDATE rooms SET Room_name = @Room_name, Floor_level = @Floor_level, Room_description = @Room_description WHERE ID = @ID", sqlConn)
                        updateCmd.Parameters.AddWithValue("@ID", ID)
                        updateCmd.Parameters.AddWithValue("@Room_name", Room_name)
                        updateCmd.Parameters.AddWithValue("@Floor_level", Floor_level)
                        updateCmd.Parameters.AddWithValue("@Room_description", Room_description)

                        Dim rowsAffected As Integer = updateCmd.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully Updated", Encoding.UTF8)
                        Else
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Error: The room with the specified ID does not exist.", Encoding.UTF8)
                        End If
                    End Using

                Catch ex As Exception
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                    response.Content = New StringContent("Application Error: There is an error in performing this action, " & ex.ToString(), Encoding.UTF8)
                End Try
            End Using

            Return response
        End Function



        ' SHOW ALL EMPLOYEE LIST

        <Route("Employee/List", Name:="Get_employee_list")>
        Public Function Get_employee_list(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Employee_Account) = New List(Of Employee_Account)
            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    Dim sqlQuery As String = "SELECT * FROM employee_account WHERE Campus_name = @Campus_name;"
                    Using cmd As New MySqlCommand(sqlQuery, sqlConn)
                        cmd.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Using dtReader As MySqlDataReader = cmd.ExecuteReader()
                            If dtReader.HasRows Then
                                While dtReader.Read()
                                    Dim dataObj As New Employee_Account
                                    With dataObj
                                        .ID = dtReader("ID").ToString()
                                        .Firstname = dtReader("Firstname").ToString()
                                        .Lastname = dtReader("Lastname").ToString()
                                        .Campus_name = dtReader("Campus_name").ToString()
                                        .Username = dtReader("Username").ToString()
                                        .Password = dtReader("Password").ToString()
                                        .Role = dtReader("Role").ToString()
                                    End With
                                    stats.Add(dataObj)
                                End While
                                Return Ok(stats)
                            Else
                                Return NotFound()
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Return InternalServerError(ex)
            End Try
        End Function

        'ADD EMPLOYEE

        <Route("Add/EmployeeName", Name:="Post_add_employeename")>
        Public Function Post_add_employeename(ByVal Firstname As String, ByVal Lastname As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            ' Trim the Firstname and Lastname to check for spaces only
            If String.IsNullOrWhiteSpace(Firstname) OrElse String.IsNullOrWhiteSpace(Lastname) Then
                response = Request.CreateResponse(HttpStatusCode.BadRequest)
                response.Content = New StringContent("Name is invalid", Encoding.UTF8)
                Return response
            End If

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if the employee_name and campus_name already exist
                    Using cmdCheckExistence As New MySqlCommand()
                        cmdCheckExistence.Connection = sqlConn
                        cmdCheckExistence.CommandText = "SELECT COUNT(*) FROM employee_account WHERE Firstname = @Firstname AND Lastname = @Lastname AND Campus_name = @Campus_name"
                        cmdCheckExistence.Parameters.AddWithValue("@Firstname", Firstname)
                        cmdCheckExistence.Parameters.AddWithValue("@Lastname", Lastname)
                        cmdCheckExistence.Parameters.AddWithValue("@Campus_name", Campus_name)
                        Dim existingRecordsCount As Integer = Convert.ToInt32(cmdCheckExistence.ExecuteScalar())

                        If existingRecordsCount > 0 Then
                            ' Employee_name and campus_name already exist, return an error response
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Employee with the same name already exists", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' If not exists, proceed with the insertion
                    Using cmdInsert As New MySqlCommand()
                        cmdInsert.Connection = sqlConn
                        cmdInsert.CommandText = "INSERT INTO employee_account (Firstname, Lastname, Campus_name) VALUES (@Firstname, @Lastname, @Campus_name)"
                        cmdInsert.Parameters.AddWithValue("@Firstname", Firstname)
                        cmdInsert.Parameters.AddWithValue("@Lastname", Lastname)
                        cmdInsert.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim rowsAffected = cmdInsert.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully added employee", Encoding.UTF8)
                        Else
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                            response.Content = New StringContent("Failed to add employee", Encoding.UTF8)
                        End If
                    End Using
                End Using

            Catch ex As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & ex.Message, Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            Return response
        End Function


        'Modify EMPLOYEE

        <Route("Modify/Employee", Name:="Post_update_employee")>
        Public Function Post_update_employee(ByVal ID As String, ByVal Firstname As String, ByVal Lastname As String, ByVal Campus_name As String, ByVal Username As String, ByVal Password As String, ByVal Role As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Try
                ' Check if any of the fields are empty
                If String.IsNullOrWhiteSpace(Firstname) OrElse String.IsNullOrWhiteSpace(Lastname) OrElse String.IsNullOrWhiteSpace(Campus_name) OrElse String.IsNullOrWhiteSpace(Username) OrElse String.IsNullOrWhiteSpace(Password) OrElse String.IsNullOrWhiteSpace(Role) Then
                    response = Request.CreateResponse(HttpStatusCode.BadRequest)
                    response.Content = New StringContent("All fields are required", Encoding.UTF8)
                    Return response
                End If

                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if the username already exists
                    Dim checkUsernameQuery As String = "SELECT COUNT(*) FROM employee_account WHERE Username = @Username AND ID <> @ID"
                    Using cmdCheckUsername As New MySqlCommand(checkUsernameQuery, sqlConn)
                        cmdCheckUsername.Parameters.AddWithValue("@ID", ID)
                        cmdCheckUsername.Parameters.AddWithValue("@Username", Username)

                        Dim usernameCount As Integer = Convert.ToInt32(cmdCheckUsername.ExecuteScalar())

                        If usernameCount > 0 Then
                            ' Username already exists, return error message
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Username is already in use", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' Check if the combination of firstname and lastname already exists
                    Dim checkNameQuery As String = "SELECT COUNT(*) FROM employee_account WHERE Firstname = @Firstname AND Lastname = @Lastname AND ID <> @ID"
                    Using cmdCheckName As New MySqlCommand(checkNameQuery, sqlConn)
                        cmdCheckName.Parameters.AddWithValue("@ID", ID)
                        cmdCheckName.Parameters.AddWithValue("@Firstname", Firstname)
                        cmdCheckName.Parameters.AddWithValue("@Lastname", Lastname)

                        Dim existingCount As Integer = Convert.ToInt32(cmdCheckName.ExecuteScalar())

                        If existingCount > 0 Then
                            ' Employee with the same name already exists, return error message
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Employee with the same name already exists", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' If no duplicate and all fields are not empty, proceed with the update
                    Dim sqlQuery As String = "UPDATE employee_account SET Firstname = @Firstname, Lastname = @Lastname, Campus_name = @Campus_name, Username = @Username, Password = @Password, Role = @Role WHERE ID = @ID"
                    Using cmdUpdate As New MySqlCommand(sqlQuery, sqlConn)
                        cmdUpdate.Parameters.AddWithValue("@ID", ID)
                        cmdUpdate.Parameters.AddWithValue("@Firstname", Firstname)
                        cmdUpdate.Parameters.AddWithValue("@Lastname", Lastname)
                        cmdUpdate.Parameters.AddWithValue("@Campus_name", Campus_name)
                        cmdUpdate.Parameters.AddWithValue("@Username", Username)
                        cmdUpdate.Parameters.AddWithValue("@Password", Password)
                        cmdUpdate.Parameters.AddWithValue("@Role", Role)

                        Dim rowsAffected As Integer = cmdUpdate.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully updated employee", Encoding.UTF8, "text/plain")
                        Else
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                            response.Content = New StringContent("Failed to update employee", Encoding.UTF8)
                        End If
                    End Using
                End Using
            Catch ex As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & ex.Message, Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            Return response
        End Function





        'SHOW ALL IN ITEM LOGS

        <Route("Item/Logs", Name:="Get_history_list")>
        Public Function Get_history_list(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Item_logs) = New List(Of Item_logs)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM item_logs
                                                            WHERE Campus_name = '" & Campus_name & "'", sqlConn)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Item_logs
                                        With dataObj
                                            .Item_name = dtReader("Item_name").ToString()
                                            .Serial_number = dtReader("Serial_number").ToString()
                                            .Date_Transfer = dtReader("Date_transfer").ToString()
                                            .Last_location = dtReader("Last_location").ToString()
                                            .Current_location = dtReader("Current_location").ToString()
                                            .Transfer_by = dtReader("Transfer_by").ToString()
                                            .Requested_by = dtReader("Requested_by").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                            .Process = dtReader("Process").ToString()
                                            .Model = dtReader("Model").ToString()
                                            .Brand = dtReader("Brand").ToString()
                                            .Item_type = dtReader("Item_type").ToString()
                                            .item_status = dtReader("item_status").ToString()
                                            .Department = dtReader("Department").ToString()

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



        'SHOW ALL ITEMS 
        <Route("Item/All", Name:="Get_item_list")>
        Public Function Get_item_list(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As New List(Of Item_list)()

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                Try
                    sqlConn.Open()

                    Using msqlcom As New MySqlCommand("SELECT * FROM item_list WHERE Campus_name = @Campus_name", sqlConn)
                        msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                            If dtReader.HasRows Then
                                While dtReader.Read()
                                    Dim dataObj As New Item_list With {
                                .ID = dtReader("ID").ToString(),
                                .Item_name = dtReader("Item_name").ToString(),
                                .Model = dtReader("Model").ToString(),
                                .Brand = dtReader("Brand").ToString(),
                                .Serial_number = dtReader("Serial_number").ToString(),
                                .Item_type = dtReader("Item_type").ToString(),
                                .Room_name = dtReader("Room_name").ToString(),
                                .Item_status = dtReader("Item_status").ToString(),
                                .Department = dtReader("Department").ToString(),
                                .Date_now = dtReader("Date_now").ToString(),
                                .Campus_name = dtReader("Campus_name").ToString()
                            }
                                    stats.Add(dataObj)
                                End While

                                Return Ok(stats)
                            Else
                                Return NotFound()
                            End If
                        End Using
                    End Using
                Catch ex As Exception
                    Return InternalServerError(ex)
                End Try
            End Using
        End Function

        'ADD ITEMS

        <Route("Add/Newitems", Name:="Post_New_items")>
        Public Function Post_New_items(ByVal Item_name As String, ByVal Model As String, ByVal Brand As String, ByVal Serial_number As String, ByVal Item_type As String, ByVal Date_now As String, ByVal Room_name As String, ByVal Item_status As String, ByVal Department As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Try
                ' Check if Room_name, Model, and Serial_number are empty or contain only spaces
                If String.IsNullOrWhiteSpace(Item_name) Then
                    response = Request.CreateResponse(HttpStatusCode.BadRequest)
                    response.Content = New StringContent("ITEM NAME is required.", Encoding.UTF8)
                    Return response
                ElseIf String.IsNullOrWhiteSpace(Model) Then
                    response = Request.CreateResponse(HttpStatusCode.BadRequest)
                    response.Content = New StringContent("Model is required.", Encoding.UTF8)
                    Return response
                ElseIf String.IsNullOrWhiteSpace(Serial_number) Then
                    response = Request.CreateResponse(HttpStatusCode.BadRequest)
                    response.Content = New StringContent(" Serial Number is required.", Encoding.UTF8)
                    Return response
                End If

                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if Serial_number and Campus_name already exist
                    Dim checkExistingQuery As String = "SELECT COUNT(*) FROM item_list WHERE Serial_number = @Serial_number AND Campus_name = @Campus_name;"
                    Using cmdCheckExisting As New MySqlCommand(checkExistingQuery, sqlConn)
                        cmdCheckExisting.Parameters.AddWithValue("@Serial_number", Serial_number)
                        cmdCheckExisting.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim existingCount As Integer = CInt(cmdCheckExisting.ExecuteScalar())

                        ' If Serial_number and Campus_name already exist, return an error
                        If existingCount > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Error: Serial number and Campus name combination already exists", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' Serial_number and Campus_name are unique, proceed with the insertion
                    Dim insertQuery As String = "INSERT INTO item_list (Item_name, Model, Brand, Serial_number, Item_type, Date_now, Room_name, Item_status, Department, Campus_name) VALUES (@Item_name, @Model, @Brand, @Serial_number, @Item_type, @Date_now, @Room_name, @Item_status, @Department, @Campus_name);"
                    Using cmdInsert As New MySqlCommand(insertQuery, sqlConn)
                        ' Parameters for insertion
                        cmdInsert.Parameters.AddWithValue("@Item_name", Item_name)
                        cmdInsert.Parameters.AddWithValue("@Model", Model)
                        cmdInsert.Parameters.AddWithValue("@Brand", Brand)
                        cmdInsert.Parameters.AddWithValue("@Serial_number", Serial_number)
                        cmdInsert.Parameters.AddWithValue("@Item_type", Item_type)
                        cmdInsert.Parameters.AddWithValue("@Date_now", Date_now)
                        cmdInsert.Parameters.AddWithValue("@Room_name", Room_name)
                        cmdInsert.Parameters.AddWithValue("@Item_status", Item_status)
                        cmdInsert.Parameters.AddWithValue("@Department", Department)
                        cmdInsert.Parameters.AddWithValue("@Campus_name", Campus_name)

                        Dim rowsAffected As Integer = cmdInsert.ExecuteNonQuery()

                        ' Check if insertion was successful
                        If rowsAffected > 0 Then
                            ' Insert into item_logs table
                            Dim insertLogQuery As String = "INSERT INTO item_logs (item_name, Serial_number, Date_transfer, Process, Campus_name, Model, Brand) VALUES (@Item_name, @Serial_number, @Date_transfer, @Process, @Campus_name, @Model, @Brand);"
                            Using cmdInsertLog As New MySqlCommand(insertLogQuery, sqlConn)
                                ' Parameters for log insertion
                                cmdInsertLog.Parameters.AddWithValue("@Item_name", Item_name)
                                cmdInsertLog.Parameters.AddWithValue("@Serial_number", Serial_number)
                                cmdInsertLog.Parameters.AddWithValue("@Date_transfer", Date_now) ' Assuming Date_now is already formatted properly
                                cmdInsertLog.Parameters.AddWithValue("@Process", "ADD IT ASSET")
                                cmdInsertLog.Parameters.AddWithValue("@Campus_name", Campus_name)
                                cmdInsertLog.Parameters.AddWithValue("@Model", Model)
                                cmdInsertLog.Parameters.AddWithValue("@Brand", Brand)

                                ' Execute log insertion
                                cmdInsertLog.ExecuteNonQuery()
                            End Using

                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully added item", Encoding.UTF8)
                        Else
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                            response.Content = New StringContent("Failed to add item", Encoding.UTF8)
                        End If
                    End Using
                End Using
            Catch ex As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Database Error: " & ex.Message, Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: " & ex.Message, Encoding.UTF8)
            End Try

            Return response
        End Function



        'MODIFY ITEMS 

        <Route("Update/Items", Name:="Update_update_item")>
        Public Async Function Update_update_item(ByVal ID As Integer, ByVal Item_name As String, ByVal Model As String, ByVal Brand As String, ByVal Serial_number As String, ByVal Item_type As String, ByVal Room_name As String, ByVal Item_status As String, ByVal Department As String, ByVal Campus_name As String) As Threading.Tasks.Task(Of IHttpActionResult)
            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    Await sqlConn.OpenAsync()

                    ' Check if the item with the specified ID exists
                    Dim checkSql As String = "SELECT COUNT(*) FROM item_list WHERE ID = @ID"
                    Using cmdCheck As New MySqlCommand(checkSql, sqlConn)
                        cmdCheck.Parameters.AddWithValue("@ID", ID)
                        Dim count As Integer = Convert.ToInt32(Await cmdCheck.ExecuteScalarAsync())

                        If count > 0 Then
                            ' The item exists, so update it
                            Dim updateSql As String = "UPDATE item_list SET Item_name = @Item_name, Model = @Model, Brand = @Brand, Serial_number = @Serial_number, Item_type = @Item_type, Room_name = @Room_name, Item_status = @Item_status, Department = @Department, Campus_name = @Campus_name WHERE ID = @ID"
                            Using cmdUpdate As New MySqlCommand(updateSql, sqlConn)
                                cmdUpdate.Parameters.AddWithValue("@Item_name", Item_name)
                                cmdUpdate.Parameters.AddWithValue("@Model", Model)
                                cmdUpdate.Parameters.AddWithValue("@Brand", Brand)
                                cmdUpdate.Parameters.AddWithValue("@Serial_number", Serial_number)
                                cmdUpdate.Parameters.AddWithValue("@Item_type", Item_type)
                                cmdUpdate.Parameters.AddWithValue("@Room_name", Room_name)
                                cmdUpdate.Parameters.AddWithValue("@Item_status", Item_status)
                                cmdUpdate.Parameters.AddWithValue("@Department", Department)
                                cmdUpdate.Parameters.AddWithValue("@Campus_name", Campus_name)
                                cmdUpdate.Parameters.AddWithValue("@ID", ID)

                                Dim rowsAffected As Integer = Await cmdUpdate.ExecuteNonQueryAsync()

                                If rowsAffected > 0 Then
                                    ' Insert into item_logs table
                                    Dim insertLogSql As String = "INSERT INTO item_logs (Item_name, Model, Brand, Item_type, Item_status, Department, Campus_name, Serial_number, Date_transfer, Process) VALUES (@Item_name, @Model, @Brand, @Item_type, @Item_status, @Department, @Campus_name, @Serial_number, NOW(), 'UPDATE IT ASSET')"
                                    Using cmdInsertLog As New MySqlCommand(insertLogSql, sqlConn)
                                        cmdInsertLog.Parameters.AddWithValue("@Item_name", Item_name)
                                        cmdInsertLog.Parameters.AddWithValue("@Model", Model)
                                        cmdInsertLog.Parameters.AddWithValue("@Brand", Brand)
                                        cmdInsertLog.Parameters.AddWithValue("@Item_type", Item_type)
                                        cmdInsertLog.Parameters.AddWithValue("@Item_status", Item_status)
                                        cmdInsertLog.Parameters.AddWithValue("@Department", Department)
                                        cmdInsertLog.Parameters.AddWithValue("@Campus_name", Campus_name)
                                        cmdInsertLog.Parameters.AddWithValue("@Serial_number", Serial_number)

                                        Await cmdInsertLog.ExecuteNonQueryAsync()
                                    End Using

                                    Return Ok("Successfully updated item")
                                Else
                                    Return InternalServerError(New Exception("Failed to update item"))
                                End If
                            End Using
                        Else
                            Return NotFound()
                        End If
                    End Using
                End Using
            Catch ex As MySqlException
                Return InternalServerError(New Exception("Database Error: " & ex.Message))
            Catch ex As Exception
                Return InternalServerError(New Exception("Application Error: " & ex.Message))
            End Try
        End Function




        'SHOW ALL ITEMS INSIDE ROOM

        <Route("Inside/Room", Name:="Get_inside_room")>
        Public Function Get_inside_room(ByVal Room_name As String, ByVal Campus_name As String) As IHttpActionResult
            Dim stats As New List(Of Item_list)()


            Dim query As String = "SELECT * FROM item_list WHERE Room_name = @Room_name AND Campus_name = @Campus_name"

            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand(query, sqlConn)
                            msqlcom.Parameters.AddWithValue("@Room_name", Room_name)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Item_list()
                                        With dataObj
                                            .Item_name = dtReader("Item_name").ToString()
                                            .Model = dtReader("Model").ToString()
                                            .Brand = dtReader("Brand").ToString()
                                            .Serial_number = dtReader("Serial_number").ToString()
                                            .Item_type = dtReader("Item_type").ToString()
                                            .Room_name = dtReader("Room_name").ToString()
                                            .Item_status = dtReader("Item_status").ToString()
                                            .Department = dtReader("Department").ToString()
                                            .Date_now = dtReader("Date_now").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                        End With
                                        stats.Add(dataObj)
                                    End While
                                    Return Ok(stats)
                                Else
                                    Return NotFound()
                                End If
                            End Using
                        End Using
                    Catch ex As Exception
                        Return InternalServerError(ex)
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function


        'TRANSFER ITEM INSIDE ROOM

        <Route("Transfer/Asset", Name:="Post_transfer_Asset")>
        Public Function Post_transfer_Asset(ByVal Serial_number As String, ByVal Room_name As String, ByVal Transfer_by As String, ByVal Requested_by As String, ByVal Campus_name As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Try
                ' Input Validation
                If String.IsNullOrEmpty(Serial_number) OrElse String.IsNullOrEmpty(Room_name) Then
                    response = Request.CreateResponse(HttpStatusCode.BadRequest)
                    response.Content = New StringContent("Invalid input parameters", Encoding.UTF8)
                    Return response
                End If

                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if the room_name is the same as the current room_name
                    Using cmdCheckRoom As New MySqlCommand()
                        cmdCheckRoom.Connection = sqlConn
                        cmdCheckRoom.CommandText = "SELECT Room_name FROM item_list WHERE Serial_number = @Serial_number"
                        cmdCheckRoom.Parameters.AddWithValue("@Serial_number", Serial_number)

                        Dim currentRoom As String = cmdCheckRoom.ExecuteScalar()?.ToString()

                        If currentRoom = Room_name Then
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Item is already in that room", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' Get the current date (today)
                    Dim currentDate As DateTime = DateTime.Now

                    ' Insert into item_logs
                    Dim insertQuery As String = "INSERT INTO item_logs (Item_name, Serial_number, Last_location, Current_location, Date_transfer, Transfer_by, Requested_by, Campus_name, Process) " &
                                        "SELECT Item_name, Serial_number, Room_name, @Room_name, @Date_transfer, @Transfer_by, @Requested_by, @Campus_name, 'Transfer Asset' " &
                                        "FROM item_list WHERE Serial_number = @Serial_number"
                    Using cmdInsertLog As New MySqlCommand(insertQuery, sqlConn)
                        cmdInsertLog.Parameters.AddWithValue("@Serial_number", Serial_number)
                        cmdInsertLog.Parameters.AddWithValue("@Room_name", Room_name)
                        cmdInsertLog.Parameters.AddWithValue("@Date_transfer", currentDate)
                        cmdInsertLog.Parameters.AddWithValue("@Transfer_by", Transfer_by)
                        cmdInsertLog.Parameters.AddWithValue("@Requested_by", Requested_by)
                        cmdInsertLog.Parameters.AddWithValue("@Campus_name", Campus_name)
                        cmdInsertLog.ExecuteNonQuery()
                    End Using

                    ' Update item_list table
                    Dim updateQuery As String = "UPDATE item_list SET Room_name = @Room_name WHERE Serial_number = @Serial_number"
                    Using cmdUpdateItem As New MySqlCommand(updateQuery, sqlConn)
                        cmdUpdateItem.Parameters.AddWithValue("@Serial_number", Serial_number)
                        cmdUpdateItem.Parameters.AddWithValue("@Room_name", Room_name)
                        cmdUpdateItem.ExecuteNonQuery()
                    End Using

                    response = Request.CreateResponse(HttpStatusCode.OK)
                    response.Content = New StringContent("Successfully TRANSFER", Encoding.UTF8)
                End Using

                Return response
            Catch ex As Exception
                ' Log the exception
                ' Logging code here...

                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Application Error: There is an error in performing this action, " & ex.ToString(), Encoding.UTF8)
                Return response
            End Try
        End Function



    End Class
End Namespace