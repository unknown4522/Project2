Imports System.Net
Imports System.Net.Http
Imports System.Security.Claims
Imports System.Web.Http
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Controllers
    <RoutePrefix("API/CreateAccount")>
    Public Class AccountController
        Inherits ApiController
        Dim identity As New ClaimsIdentity(User.Identity)
        Private response As HttpResponseMessage
        Dim ds As DataSet = New DataSet


        <Route("Create/account", Name:="Post_create_acc")>
        Public Function Post_create_acc(ByVal Firstname As String, ByVal Lastname As String, ByVal Campus_name As String, ByVal Username As String, ByVal Password As String, ByVal Role As String) As HttpResponseMessage
            Dim response As HttpResponseMessage

            Try
                Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                    sqlConn.Open()

                    ' Check if the Firstname, Lastname, or Username already exist
                    Using cmdCheckExistence As New MySqlCommand()
                        cmdCheckExistence.Connection = sqlConn
                        cmdCheckExistence.CommandText = "SELECT COUNT(*) FROM employee_account WHERE Firstname = @Firstname AND Lastname = @Lastname OR Username = @Username"
                        cmdCheckExistence.Parameters.AddWithValue("@Firstname", Firstname)
                        cmdCheckExistence.Parameters.AddWithValue("@Lastname", Lastname)
                        cmdCheckExistence.Parameters.AddWithValue("@Username", Username)
                        Dim existingRecordsCount As Integer = Convert.ToInt32(cmdCheckExistence.ExecuteScalar())

                        If existingRecordsCount > 0 Then
                            ' Account already exists, return an error response
                            response = Request.CreateResponse(HttpStatusCode.BadRequest)
                            response.Content = New StringContent("Account already exists", Encoding.UTF8)
                            Return response
                        End If
                    End Using

                    ' If not exists, proceed with the insertion
                    Using cmdInsert As New MySqlCommand()
                        cmdInsert.Connection = sqlConn
                        cmdInsert.CommandText = "INSERT INTO employee_account (Firstname, Lastname, Campus_name, Username, Password, Role) VALUES (@Firstname, @Lastname, @Campus_name, @Username, @Password, @Role)"
                        cmdInsert.Parameters.AddWithValue("@Firstname", Firstname)
                        cmdInsert.Parameters.AddWithValue("@Lastname", Lastname)
                        cmdInsert.Parameters.AddWithValue("@Campus_name", Campus_name)
                        cmdInsert.Parameters.AddWithValue("@Username", Username)
                        cmdInsert.Parameters.AddWithValue("@Password", Password)
                        cmdInsert.Parameters.AddWithValue("@Role", Role)

                        Dim rowsAffected = cmdInsert.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            response = Request.CreateResponse(HttpStatusCode.OK)
                            response.Content = New StringContent("Successfully Created", Encoding.UTF8)
                        Else
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError)
                            response.Content = New StringContent("Failed to Create Account", Encoding.UTF8)
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

    End Class
End Namespace