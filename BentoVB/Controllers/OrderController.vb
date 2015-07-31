Imports System.Net
Imports System.Web.Http

Namespace Controllers
    Public Class OrderController
        Inherits ApiController

        ' GET: api/User/{userId}/Order/{year}/{month}
        Public Function GetUsersOrderByMonth(ByVal userId As Integer, ByVal year As Integer, month As Integer) As IEnumerable(Of OrderModel)
            Using db As New BentoDbContext()
                Return db.GetOrdersOfMonth(userId, year, month)
            End Using
        End Function

        ' GET: api/User/{userId}/Order/{year}/{month}/{day}
        Public Function GetUsersOrder(ByVal userId As Integer, ByVal year As Integer, ByVal month As Integer, day As Integer) As OrderModel
            Using db As New BentoDbContext()
                Return db.GetOrder(userId, New Date(year, month, day))
            End Using
        End Function

        ' POST: api/User/{userId}/Order
        Public Sub PostUsersOrder(ByVal userId As Integer, <FromBody> order As OrderModel)
            Using db As New BentoDbContext()
                Dim o = db.Orders.Where(Function(a) a.UserId = userId AndAlso a.OrderDate = order.OrderDate).FirstOrDefault()
                If order.IsOrdered Then
                    If o Is Nothing Then
                        db.Orders.Add(New Order With {.UserId = userId, .OrderDate = order.OrderDate})
                    End If
                Else
                    If o IsNot Nothing Then
                        db.Orders.Remove(o)
                    End If
                End If
                db.SaveChanges()
            End Using
        End Sub
    End Class
End Namespace