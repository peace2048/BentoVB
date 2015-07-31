Imports System.Data.Entity

Public Class BentoDbContext
    Inherits DbContext

    Public Property Users As DbSet(Of User)
    Public Property Calendar As DbSet(Of Calendar)
    Public Property Orders As DbSet(Of Order)

End Class
