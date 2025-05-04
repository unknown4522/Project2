Imports System.Net
Imports System.Net.Http
Imports System.Security.Claims
Imports System.Web.Http
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Controllers

    <RoutePrefix("API/Materials")>
    Public Class MaterialsController
        Inherits ApiController
        Dim identity As New ClaimsIdentity(User.Identity)
        Private response As HttpResponseMessage
        Dim ds As DataSet = New DataSet


        <Route("Supply/List", Name:="Get_Material_lists")>
        Public Function Get_Material_lists(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Materials_list) = New List(Of Materials_list)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT * FROM materials_list WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)
                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows = True Then
                                    While dtReader.Read()
                                        Dim dataObj As New Materials_list
                                        With dataObj
                                            .Material_ID = dtReader("Material_ID").ToString()
                                            .Material_name = dtReader("Material_name").ToString()
                                            .Quantity = dtReader("Quantity").ToString()
                                            .Date_Purchased = dtReader("Date_Purchased").ToString()
                                            .Received_date = dtReader("Received_date").ToString()
                                            .Request_by = dtReader("Request_by").ToString()
                                            .Approve_by = dtReader("Approve_by").ToString()
                                            .Status = dtReader("Status").ToString()
                                            .Campus_name = dtReader("Campus_name").ToString()
                                            .Supplier = dtReader("Supplier").ToString()
                                            .Unit = dtReader("Unit").ToString()
                                            .Received_By = dtReader("Received_By").ToString()
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




    End Class
End Namespace