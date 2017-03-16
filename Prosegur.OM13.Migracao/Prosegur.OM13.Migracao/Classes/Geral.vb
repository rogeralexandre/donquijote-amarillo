Imports System.Text.RegularExpressions

Public Class Geral

    Public Shared Function RemoverAcentuacao(ByVal pTexto As String) As String

        Dim comAcentos As String = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç."
        Dim semAcentos As String = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc "

        For i As Integer = 0 To comAcentos.Length - 1
            pTexto = pTexto.Replace(comAcentos(i).ToString(), semAcentos(i).ToString())
        Next

        Return pTexto.Trim

    End Function

    Public Shared Function RetiraCaracterEspecial(ByVal pTexto As String) As String

        Dim pattern As String = "(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s]"
        Dim replacement As String = String.Empty
        Dim rgx As New Regex(pattern)
        Return rgx.Replace(pTexto, replacement)

    End Function

End Class
