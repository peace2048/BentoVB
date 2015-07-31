Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Http

Public Module WebApiConfig
    Public Sub Register(ByVal config As HttpConfiguration)
        ' Web API の設定およびサービス


        ' Web API ルート
        config.MapHttpAttributeRoutes()

        config.Routes.MapHttpRoute(
            name:="OrderApi",
            routeTemplate:="api/User/{userId}/Order/{year}/{month}/{day}",
            defaults:=New With {
                .controller = "Order",
                .year = RouteParameter.Optional,
                .month = RouteParameter.Optional,
                .day = RouteParameter.Optional
            }
        )

        config.Routes.MapHttpRoute(
            name:="CalendarApi",
            routeTemplate:="api/Calendar/{year}/{month}/{day}",
            defaults:=New With {
                .controller = "Calendar",
                .year = RouteParameter.Optional,
                .month = RouteParameter.Optional,
                .day = RouteParameter.Optional
            }
        )

        config.Routes.MapHttpRoute(
            name:="SummaryApi",
            routeTemplate:="api/Summary/{action}/{year}/{month}/{day}",
            defaults:=New With {
                .controller = "Summary",
                .day = RouteParameter.Optional
            }
        )

        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )

        config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local
    End Sub
End Module
