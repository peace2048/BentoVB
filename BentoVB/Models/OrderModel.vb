Public Class OrderModel
    Public Property Id As Integer
    Public Property OrderDate As Date
    Public Property IsOrdered As Boolean
    Public Property Status As Integer
End Class

Module OrderModelExtension
    <System.Runtime.CompilerServices.Extension>
    Public Function GetOrder(context As BentoDbContext, userId As Integer, orderDate As Date) As OrderModel
        Dim calendar = context.Calendar.Where(Function(a) a.Target = orderDate).FirstOrDefault()
        Dim order = context.Orders.Where(Function(a) a.UserId = userId AndAlso a.OrderDate = orderDate).FirstOrDefault()

        Dim result = New OrderModel()
        result.OrderDate = orderDate
        If order IsNot Nothing Then
            result.Id = order.Id
            result.IsOrdered = True
        End If
        If calendar IsNot Nothing Then
            result.Status = calendar.Flag
        Else
            Select Case orderDate.DayOfWeek
                Case DayOfWeek.Saturday, DayOfWeek.Sunday
                    result.Status = 2
                Case Else
                    result.Status = 0
            End Select
        End If
        Return result
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function GetOrdersOfMonth(context As BentoDbContext, userId As Integer, year As Integer, month As Integer) As IEnumerable(Of OrderModel)
        Dim startDate = New Date(year, month, 21)
        Dim endDate = startDate.AddMonths(1)

        Dim calendars = context.Calendar.Where(Function(a) a.Target >= startDate AndAlso a.Target < endDate).ToList()
        Dim orders = context.Orders.Where(Function(a) a.UserId = userId AndAlso a.OrderDate >= startDate AndAlso a.OrderDate < endDate).ToList()
        Dim days = Enumerable.Range(0, 31).Select(Function(n) startDate.AddDays(n)).Where(Function(a) a < endDate)

        Return days.GroupJoin(calendars,
                       Function(day) day,
                       Function(cal) cal.Target,
                       Function(day, g)
                           Dim cal = g.FirstOrDefault()
                           Dim result = New OrderModel()
                           result.OrderDate = day
                           If cal IsNot Nothing Then
                               result.Status = cal.Flag
                           Else
                               Select Case day.DayOfWeek
                                   Case DayOfWeek.Saturday, DayOfWeek.Sunday
                                       result.Status = 2
                                   Case Else
                                       result.Status = If(day < Date.Today, 1, 0)
                               End Select
                           End If
                           Return result
                       End Function).
        GroupJoin(orders.AsEnumerable(),
                  Function(r) r.OrderDate,
                  Function(order) order.OrderDate,
                  Function(r, g)
                      Dim order = g.FirstOrDefault()
                      If order IsNot Nothing Then
                          r.Id = order.Id
                          r.IsOrdered = True
                      End If
                      Return r
                  End Function)
    End Function
End Module
