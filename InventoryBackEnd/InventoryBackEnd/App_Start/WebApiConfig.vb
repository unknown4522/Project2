Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Http

Public Module WebApiConfig
    Public Sub Register(ByVal config As HttpConfiguration)
        ' Web API configuration and services

        ' Enable attribute routing
        config.MapHttpAttributeRoutes()

        ' Default API route (keeps /api/controller/id style)
        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )

        ' Custom route for controller/action (optional, if you want to use /controller/action/id style)
        config.Routes.MapHttpRoute(
            name:="ControllerActionApi",
            routeTemplate:="{controller}/{action}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )
    End Sub
End Module
