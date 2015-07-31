Imports System.Net
Imports System.Web.Http

Namespace Controllers
    Public Class UserController
        Inherits ApiController

        ' GET: api/User
        Public Function GetValues() As IEnumerable(Of User)
            Using db As New BentoDbContext()
                Return db.Users.ToList()
            End Using
        End Function

        ' GET: api/User/5
        Public Function GetValue(ByVal id As Integer) As User
            Using db As New BentoDbContext()
                Return db.Users.Where(Function(a) a.Id = id).FirstOrDefault()
            End Using
        End Function

        ' POST: api/User
        Public Sub PostValue(<FromBody()> ByVal value As User)
            Using db As New BentoDbContext()
                db.Users.Add(value)
                db.SaveChanges()
            End Using
        End Sub
    End Class
End Namespace