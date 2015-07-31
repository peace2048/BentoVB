Public Class Calendar
    <System.ComponentModel.DataAnnotations.Key>
    Public Property Target As Date
    Public Property Flag As Integer
End Class

Module CalendarExtensions
    <System.Runtime.CompilerServices.Extension>
    Public Function IsWeekEnds(target As Date) As Boolean
        Return target.DayOfWeek = DayOfWeek.Saturday OrElse target.DayOfWeek = DayOfWeek.Sunday
    End Function
End Module