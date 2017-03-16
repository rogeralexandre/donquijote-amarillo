Imports System.Data


Public Class funcoes_va
    Public Shared Function RetornarSQL(cmd As IDbCommand) As String
        Dim quotedParameterTypes As DbType() = New DbType() {DbType.AnsiString,
                                                             DbType.[Date],
                                                             DbType.DateTime,
                                                             DbType.Guid,
                                                             DbType.[String],
                                                             DbType.AnsiStringFixedLength,
                                                             DbType.StringFixedLength}
        Dim query As String = cmd.CommandText

        Dim arrParams = New IDataParameter(cmd.Parameters.Count - 1) {}
        cmd.Parameters.CopyTo(arrParams, 0)

        For Each p As IDataParameter In arrParams.OrderByDescending(Function(pa) pa.ParameterName.Length)
            Dim value As String = p.Value.ToString()
            If quotedParameterTypes.Contains(p.DbType) Then
                value = (Convert.ToString("'") & value) + "'"
            End If
            query = query.Replace(p.ParameterName, value)
        Next

        Return query

    End Function
End Class
