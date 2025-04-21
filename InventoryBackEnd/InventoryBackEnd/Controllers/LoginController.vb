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


    End Class
End Namespace