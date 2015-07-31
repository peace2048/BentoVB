Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class User
    <DatabaseGenerated(DatabaseGeneratedOption.None), Key>
    Public Property Id As Integer
    Public Property Name As String
End Class
