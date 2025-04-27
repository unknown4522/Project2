Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Security.Claims
Imports System.Web.Http
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json


Namespace Controllers
    <RoutePrefix("API/LoginAccount")>
    Public Class LoginController
        Inherits ApiController
        Dim identity As New ClaimsIdentity(User.Identity)
        Private response As HttpResponseMessage
        Dim ds As DataSet = New DataSet

        'LOG IN
        <Route("Login/All", Name:="Post_login_acc")>
        Public Function Post_login_acc(ByVal Username As String, ByVal Password As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    Using cmdCheckCredentials As New MySqlCommand()
                        cmdCheckCredentials.Connection = sqlConn
                        cmdCheckCredentials.Parameters.AddWithValue("@Username", Username)
                        cmdCheckCredentials.Parameters.AddWithValue("@Password", Password)
                        cmdCheckCredentials.CommandText = "SELECT ID, Firstname, Lastname, Campus_name, Role FROM employee_account WHERE Username = @Username AND Password = @Password"

                        Using reader As MySqlDataReader = cmdCheckCredentials.ExecuteReader()
                            If reader.HasRows Then
                                reader.Read()
                                Dim Firstname As String = reader.GetString(reader.GetOrdinal("Firstname"))
                                Dim Lastname As String = reader.GetString(reader.GetOrdinal("Lastname"))
                                Dim Campus_name As String = reader.GetString(reader.GetOrdinal("Campus_name"))
                                Dim Role As String = reader.GetString(reader.GetOrdinal("Role"))
                                response = Request.CreateResponse(HttpStatusCode.OK)
                                response.Content = New StringContent($"Successfully Logged In|{Firstname}|{Lastname}|{Campus_name}|{Role}", Encoding.UTF8)
                            Else
                                response = Request.CreateResponse(HttpStatusCode.Unauthorized)
                                response.Content = New StringContent("Invalid username or password", Encoding.UTF8)
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As MySqlException
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Server Error: Unable to process the request", Encoding.UTF8)
            Catch ex As Exception
                response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                response.Content = New StringContent("Server Error: Unable to process the request", Encoding.UTF8)
            End Try

            Return response
        End Function

        <Route("Upload/ProfilePicture"), HttpPost>
        Public Function UploadProfilePicture() As HttpResponseMessage
            Try
                Dim httpRequest = HttpContext.Current.Request

                If httpRequest.Files.Count > 0 Then
                    Dim postedFile = httpRequest.Files(0)
                    Dim fileName = Path.GetFileName(postedFile.FileName)

                    ' Ensure directory exists
                    Dim uploadsFolder = HttpContext.Current.Server.MapPath("~/Uploads/ProfilePictures/")

                    ' Create the folder if it does not exist
                    If Not Directory.Exists(uploadsFolder) Then
                        Directory.CreateDirectory(uploadsFolder)
                    End If

                    Dim filePath = Path.Combine(uploadsFolder, fileName)

                    ' Save file to server folder
                    postedFile.SaveAs(filePath)

                    ' Now update database
                    Dim userId = httpRequest.Form("ID") ' ID must be sent from client

                    Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                        sqlConn.Open()
                        Using cmdUpdate As New MySqlCommand("UPDATE employee_account SET ProfilePicturePath = @ProfilePicturePath WHERE ID = @ID", sqlConn)
                            cmdUpdate.Parameters.AddWithValue("@ProfilePicturePath", "/Uploads/ProfilePictures/" & fileName)
                            cmdUpdate.Parameters.AddWithValue("@ID", userId)
                            cmdUpdate.ExecuteNonQuery()
                        End Using
                    End Using

                    Return Request.CreateResponse(HttpStatusCode.OK, "Profile picture uploaded successfully!")
                Else
                    Return Request.CreateResponse(HttpStatusCode.BadRequest, "No file received.")
                End If
            Catch ex As Exception
                Return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error uploading profile picture: " & ex.Message)
            End Try
        End Function

    End Class
End Namespace