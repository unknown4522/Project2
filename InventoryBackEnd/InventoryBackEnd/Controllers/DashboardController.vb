Imports System.Net
Imports System.Net.Http
Imports System.Security.Claims
Imports System.Web.Http
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Controllers
    <RoutePrefix("API/Dashboard")>
    Public Class DashboardController
        Inherits ApiController
        Dim identity As New ClaimsIdentity(User.Identity)
        Private response As HttpResponseMessage
        Dim ds As DataSet = New DataSet


        'ROOMS COUNT

        <Route("Rooms/Counter", Name:="Get_rooms_Counter")>
        Public Function Get_rooms_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM rooms WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'IT ASSET COUNT

        <Route("Item/Counter", Name:="Get_Item_Counter")>
        Public Function Get_Item_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM item_list WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        ' Employee Count
        <Route("Employee/Counter", Name:="Get_Employee_Counter")>
        Public Function Get_Employee_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM employee_account WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        ' Asset Log Count

        <Route("Logs/Counter", Name:="Get_Logs_Counter")>
        Public Function Get_Logs_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM item_logs WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        ' BRAND COUNTER

        <Route("Brand/Counter", Name:="Get_Brand_Counter")>
        Public Function Get_Brand_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM brand WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'COUNTER APPREL Available

        <Route("Apparel/Counter", Name:="Get_Apprel_Counter")>
        Public Function Get_Apprel_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM apparel_stock WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'APPAREL LOGS COUNTER

        <Route("Apparellogs/Counter", Name:="Get_Apprellogs_Counter")>
        Public Function Get_Apprellogs_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM apparel_logs WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'APPAREL TYPE COUNTER

        <Route("Appareltype/Counter", Name:="Get_Appareltype_Counter")>
        Public Function Get_Appareltype_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM apparel_type WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'APPAREL SIZE COUNTER

        <Route("Apparelsize/Counter", Name:="Get_Apparelsize_Counter")>
        Public Function Get_Apparelsize_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM size_list WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'APPAREL CLAIM COUNTER

        <Route("Apparelclaim/Counter", Name:="Get_Apparelclaim_Counter")>
        Public Function Get_Apparelclaim_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM claim_stub WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

        'APPARE GRADE COUNTER

        <Route("Apparelgrade/Counter", Name:="Get_Apparelgrade_Counter")>
        Public Function Get_Apparelgrade_Counter(ByVal Campus_name As String) As IHttpActionResult
            Dim stats As List(Of Fieldcounter) = New List(Of Fieldcounter)
            Using sqlConn As New MySqlConnection(ConfigurationManager.ConnectionStrings("constr").ConnectionString)
                If sqlConn.State = ConnectionState.Closed Then
                    Try
                        sqlConn.Open()
                        Using msqlcom As New MySqlCommand("SELECT COUNT(*) as count_value FROM grade WHERE Campus_name = @Campus_name", sqlConn)
                            msqlcom.Parameters.AddWithValue("@Campus_name", Campus_name)

                            Using dtReader As MySqlDataReader = msqlcom.ExecuteReader()
                                If dtReader.HasRows Then
                                    While dtReader.Read()
                                        Dim dataObj As New Fieldcounter
                                        With dataObj
                                            .Counter = dtReader("count_value").ToString()
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
                    Finally
                        sqlConn.Close()
                    End Try
                Else
                    Return InternalServerError()
                End If
            End Using
        End Function

    End Class
End Namespace