Imports System.Net
Imports System.Web.Http

Namespace Controllers
    Public Class CalendarController
        Inherits ApiController

        ' GET: api/Calendar/{year}
        Public Function GetValue(ByVal year As Integer) As IEnumerable(Of Calendar)
            Using db As New BentoDbContext()
                Return db.Calendar.Where(Function(a) a.Target.Year = year).AsEnumerable().Where(Function(a) a.Target.IsWeekEnds() OrElse a.Flag = 2).ToList()
            End Using
        End Function

        ' POST: api/Calendar
        Public Sub PostValue(<FromBody()> ByVal value As Calendar)
            Using db As New BentoDbContext()
                Dim target = db.Calendar.Where(Function(a) a.Target = value.Target).FirstOrDefault()
                If target Is Nothing Then
                    db.Calendar.Add(value)
                Else
                    target.Flag = value.Flag
                End If
                db.SaveChanges()
            End Using
        End Sub

        ' DELETE: api/Calendar/{year}
        Public Sub DeleteValue(ByVal year As Integer, month As Integer, day As Integer)
            Dim d = New Date(year, month, day)
            Using db As New BentoDbContext()
                Dim target = db.Calendar.Where(Function(a) a.Target = d).FirstOrDefault()
                If target IsNot Nothing Then
                    If target.Flag <> 1 Then
                        db.Calendar.Remove(target)
                        db.SaveChanges()
                    End If
                End If
            End Using
        End Sub
    End Class
End Namespace