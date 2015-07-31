Imports System.Net
Imports System.Web.Http

Public Class SummaryController
    Inherits ApiController

    <ActionName("Today")>
    Public Function GetDaysSummaryByDate(year As Integer, month As Integer, day As Integer) As DaysSummaryModel
        Using db As New BentoDbContext()
            Dim today = New Date(year, month, day)
            Dim calendar = db.Calendar.Where(Function(a) a.Target = today).FirstOrDefault()
            Dim order = db.Orders.Where(Function(a) a.OrderDate = today).Count()
            Dim result = New DaysSummaryModel()
            result.Count = order
            result.Target = today
            If calendar IsNot Nothing Then
                result.Flag = calendar.Flag
            Else
                result.Flag = If(today.IsWeekEnds(), 2, 0)
            End If
            Return result
        End Using
    End Function

    <ActionName("Date")>
    Public Function GetDaysSummaryOfMonth(year As Integer, month As Integer) As IEnumerable(Of DaysSummaryModel)
        Using db As New BentoDbContext()
            Dim startDate = New Date(year, month, 21)
            Dim endDate = startDate.AddMonths(1)

            Dim cc = From c In db.Calendar
                     Where startDate <= c.Target AndAlso c.Target < endDate

            Dim oo = From o In db.Orders
                     Where startDate <= o.OrderDate AndAlso o.OrderDate < endDate
                     Group By o.OrderDate Into g = Group
                     Select OrderDate, g.Count()

            Dim qq = From n In Enumerable.Range(0, 31)
                     Select target = startDate.AddDays(n)
                     Where target < endDate
                     Group Join c In cc On target Equals c.Target Into g1 = Group
                     Group Join o In oo On target Equals o.OrderDate Into g2 = Group
                     Select New DaysSummaryModel With {
                         .Target = target,
                         .Count = g2.Count,
                         .Flag = If(g1.Any(), g1.First().Flag, If(target.IsWeekEnds(), 2, 0))
                     }

            Return qq.ToList()
        End Using
    End Function

    <ActionName("User")>
    Public Function GetUsersSummaryOfMonth(year As Integer, month As Integer) As IEnumerable(Of UsersSummaryModel)
        Using db As New BentoDbContext()
            Dim startDate = New Date(year, month, 21)
            Dim endDate = startDate.AddMonths(1)

            Return (From o In db.Orders
                    Where startDate <= o.OrderDate AndAlso o.OrderDate < endDate
                    Group o By o.UserId Into g = Group
                    Join u In db.Users On UserId Equals u.Id
                    Select New UsersSummaryModel With {.User = u, .Count = g.Count()}).
            ToList()
        End Using
    End Function

End Class
