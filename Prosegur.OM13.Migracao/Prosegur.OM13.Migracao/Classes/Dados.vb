Imports Prosegur.DbHelper.AcessoDados
Imports System.Data

Public Class Dados

#Region "CONEXÕES"

    Public Shared ReadOnly Property CONEXAO_NOVI() As String
        Get
            Return System.Configuration.ConfigurationManager.AppSettings("ConexaoNOVI")
        End Get
    End Property

    Public Shared ReadOnly Property CONEXAO_MARTE() As String
        Get
            Return System.Configuration.ConfigurationManager.AppSettings("ConexaoMarte")
        End Get
    End Property

    Public Shared ReadOnly Property CONEXAO_PROFAT() As String
        Get
            Return System.Configuration.ConfigurationManager.AppSettings("ConexaoPROFAT")
        End Get
    End Property

#End Region

#Region "CLIENTE"

    ''' <summary>
    ''' Retorna dados de cliente
    ''' </summary>
    ''' <returns>Tabela com dados de cliente MARTE</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaClientesDEPara() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_DADOSCLIENTE
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna cod da via da tabela XELEM
    ''' </summary>
    ''' <returns>String com o codigo da via</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaCodVia(ByVal pCodComercial As String) As String
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_NOVI)
            cmd.CommandText = My.Resources.SELECT_NOVI_BUSCACODVIA
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_COMERCIAL", DbType.String, pCodComercial))
            Dim dtCodVia As DataTable = DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_NOVI, cmd)

            If Not dtCodVia Is Nothing AndAlso dtCodVia.Rows.Count > 0 Then
                Return dtCodVia.Rows(0).Item(0).ToString
            Else
                Return String.Empty
            End If
        End Using
    End Function

    ''' <summary>
    ''' Retorna dados de cliente com NIF duplicado
    ''' </summary>
    ''' <returns>Tabela com dados de cliente MARTE</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaClientesDuplicados() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_VERIFICADUPLICIDADE
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna dados de cliente com NIF nulo
    ''' </summary>
    ''' <returns>Tabela com dados de cliente MARTE</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaClientesComNIFNulo() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_NOVI_VERIFICAFISCALNULO
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Insere dados de tipo de logradouro no NOVI
    ''' </summary>
    ''' <returns>quantidade de dados inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaCodComercialClientePROFAT(ByVal pCodComercial As String, ByVal pCodProfat As String, ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.ATUALIZA_PROFAT_ATUALIZACLIENTE

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "COD_COMERCIAL", DbType.String, pCodComercial))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "COD_PROFAT", DbType.String, pCodProfat))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Insere dados de tipo de logradouro no NOVI
    ''' </summary>
    ''' <returns>quantidade de dados inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaCodComercialClienteNOVI(ByVal pCodNOVI As String, ByVal pCodNIF As String, _
                                                           ByVal pNomeFantasia As String, ByVal pDesRazaoSoc As String, _
                                                           ByVal pCodComercial As String, ByVal pCodTipoLog As String, ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.ATUALIZA_NOVI_ATUALIZACLIENTE

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_IDENTIFICACION_FISCAL", DbType.String, pCodNIF))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "DES_NOMBRE_FANTASIA", DbType.String, pNomeFantasia))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "DES_RAZON_SOCIAL", DbType.String, pDesRazaoSoc))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "OCLTE_COD_VIA", DbType.String, RetornaCodVia(pCodTipoLog)))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_COMERCIAL", DbType.String, pCodComercial))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_NOVI", DbType.String, pCodNOVI))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Function AtualizaSegmentoSubsegmentoClientecomBasePROFAT(ByVal pCodPROFAT As String,
                                                               ByVal pCodRamo As String,
                                                               ByVal pCodSubRamo As String,
                                                               ByVal pNomeArquivoLog As String,
                                                               ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao

                Dim sSql As String = My.Resources.UPDATE_SEGMENTO_SUBSEGMENTO_MARTE

                If (String.IsNullOrEmpty(pCodRamo)) Then
                    pCodRamo = "0"
                End If
                If (String.IsNullOrEmpty(pCodSubRamo)) Then
                    pCodSubRamo = "0"
                End If

                sSql = sSql.Replace("COD_RAMO", pCodRamo)
                sSql = sSql.Replace("COD_SUBRAMO", pCodSubRamo)
                sSql = sSql.Replace("COD_PROFAT", pCodPROFAT)

                cmd.CommandText = sSql
                Log.GravarLog("executando comando: " & sSql, pNomeArquivoLog)

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
            End Using
        Catch ex As Exception
            'Throw ex
            Log.GravarLog("Ocorreu erro na execução do comando: " & ex.Message, pNomeArquivoLog)
            Log.GravarLog(ex.ToString(), pNomeArquivoLog)
        End Try

    End Function

    Public Shared Function RetornaRamoSubramoPROFAT() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_RAMOS_SUBRAMOS_CLIENTE
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

#End Region

#Region "SUBCLIENTE"

    ''' <summary>
    ''' Retorna Tabela com dados de subcliente MARTE
    ''' </summary>
    ''' <returns>Tabela com dados de subcliente</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSubClientesDEPara() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_DADOSSUBCLIENTE
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna dados de responsavel do faturamento
    ''' </summary>
    ''' <returns>Tabela com dados de responsavel</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaResFat(ByVal pCodCliente As String, ByVal pCodSubCliente As String) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_DADOSRESPFAT

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "CLIENTE", DbType.String, pCodCliente))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "SUBCLIENTE", DbType.String, pCodSubCliente))

            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna quantidade de dados que tem no PROFAT
    ''' </summary>
    ''' <returns>tabela com NIF nulos</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSubClientesNIFNulo() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_VERIFICANIFNULO
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna quantidade de dados que tem no PROFAT
    ''' </summary>
    ''' <returns>tabela com NIF duplicados</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSubClienteDuplicado() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_VERIFICANIFSUPLICADO
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna quantidade de dados que tem no PROFAT
    ''' </summary>
    ''' <returns>tabela com NIF diferentes</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSubClienteRazaoNIF() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_VERIFICARAZAONIF
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Insere dados de tipo de logradouro no NOVI
    ''' </summary>
    ''' <returns>quantidade de dados inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaCodComercialSubCliPROFAT(ByVal pCodComercial As String, ByVal pCodProfat As String, ByRef objtransacao As IDbTransaction) As Integer

        Dim pNomeArquivoScript As String = "SCRIPT_PROFAT"
        Dim sSQL As String

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.ATUALIZA_PROFAT_SUBCLIENTE

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "COD_COMERCIAL", DbType.String, pCodComercial))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "COD_PROFAT", DbType.String, pCodProfat))

                sSQL = My.Resources.ATUALIZA_PROFAT_SUBCLIENTE
                sSQL = sSQL.Replace("@COD_COMERCIAL", "'" & pCodComercial & "'")
                sSQL = sSQL.Replace("@COD_PROFAT", "'" & pCodProfat & "'")
                Log.GravarLog(sSQL, pNomeArquivoScript)

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Insere dados de tipo de logradouro no NOVI
    ''' </summary>
    ''' <returns>quantidade de dados inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaCodComercialSubCliNOVI(ByVal pNomeSubCli As String, ByVal pNomeCurto As String, ByVal pCodTipoLog As String, _
                                                          ByVal pCodNOVICliente As String, ByVal pCodNOVISubCliente As String, _
                                                          ByVal pCodComercial As String, ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.ATUALIZA_NOVI_SUBCLIENTE

                'cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "OSBCL_NOC_SUBCLI", DbType.String, pNomeCurto))
                'cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "OSBCL_NOL_SUBCLI", DbType.String, pNomeSubCli))
                'cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "OSBCL_COD_VIA", DbType.String, RetornaCodVia(pCodTipoLog)))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_COMERCIAL", DbType.String, pCodComercial))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_CLIE", DbType.String, pCodNOVICliente))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_SUBCLIE", DbType.String, pCodNOVISubCliente))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

#End Region

#Region "TIPO LOGRADOURO"

    ''' <summary>
    ''' Retorna quantidade de dados que tem no PROFAT
    ''' </summary>
    ''' <returns>Inteiro</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaQuantidadeDadosPROFAT() As Integer
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_PROFAT_COUNTTIPOLOG
            Return DbHelper.AcessoDados.ExecutarScalar(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna quantidade de dados que tem no MARTE
    ''' </summary>
    ''' <returns>Inteiro</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaQuantidadeDadosMARTE() As Integer
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_COUNTTIPOLOG
            Return DbHelper.AcessoDados.ExecutarScalar(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Busca dados de tipo de logradouro do PROFAT
    ''' </summary>
    ''' <returns>tabela com todos os dados de tipo de logradouro</returns>
    ''' <remarks></remarks>
    Public Shared Function TipoLogradouroPROFAT(Optional ByVal pCodTipoLog As String = "", Optional ByVal pDesTipoLog As String = "") As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)

            If String.IsNullOrEmpty(pCodTipoLog) Then
                cmd.CommandText = String.Format(My.Resources.SELECT_PROFAT_BUSCARTIPOLOG, String.Empty)
            Else
                cmd.CommandText = String.Format(My.Resources.SELECT_PROFAT_BUSCARTIPOLOG, " WHERE (UPPER(DESABRTIPLOG) = '" & Geral.RemoverAcentuacao(pCodTipoLog).ToString.ToUpper & "' OR UPPER(DESABRTIPLOG) = '" & Geral.RemoverAcentuacao(pCodTipoLog).ToString.ToUpper & ".') OR (DESTIPLOG = '" & Geral.RemoverAcentuacao(pDesTipoLog).ToString.ToUpper & "' OR DESTIPLOG = '" & pDesTipoLog.ToString.ToUpper & "')")
            End If
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Busca dados de tipo de logradouro do MARTE
    ''' </summary>
    ''' <returns>tabela com todos os dados de tipo de logradouro</returns>
    ''' <remarks></remarks>
    Public Shared Function TipoLogradouroMARTE() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = String.Format(My.Resources.SELECT_MARTE_BUSCARTIPOLOG, String.Empty)
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Apaga todos os dados de Tipo de logradouro do NOVI para refazer a inserção a partir dos dados do MARTE ou PROFAT
    ''' </summary>
    ''' <returns>quantidade de dados deletados</returns>
    ''' <remarks></remarks>
    Public Shared Function DeletaTipoLogradouroNOVI(ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao

            cmd.CommandText = My.Resources.DELETE_NOVI_DELETATIPOLOG
            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    Public Shared Function AtualizaTipoLogradouroPROFAT(ByVal pCodTipoLog As String, ByVal pCodComercial As String, ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.UPDATE_PROFAT_TIPOLOG

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "CODCOMERCIAL", DbType.String, pCodComercial))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "COD_PROFAT", DbType.String, pCodTipoLog))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Function AtualizaTipoLogradouroMARTE(ByVal pCodTipoLog As String, ByVal pCodComercial As String, ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.UPDATE_MARTE_TIPOLOG

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_PROFAT", DbType.String, pCodTipoLog))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_MARTE", DbType.String, pCodComercial & "_"))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Function AtualizaTipoLogradouroMARTE_TEMP(ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.UPDATE_MARTE_TIPOLOGTEMP

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Insere dados de tipo de logradouro no NOVI
    ''' </summary>
    ''' <returns>quantidade de dados inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaTipoLogradouroNOVI(ByVal pCodTipoLog As String, ByVal pCodComercial As String, ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao

                cmd.CommandText = My.Resources.UPDATE_NOVI_TIPOLOG

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_ELEMENTO", DbType.String, pCodTipoLog))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "CODCOMERCIAL", DbType.String, pCodComercial))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Apaga todos os dados de Tipo de logradouro da tabela de DePara para refazer a inserção a partir dos dados do MARTE, NOVI e  PROFAT
    ''' </summary>
    ''' <returns>quantidade de dados deletados</returns>
    ''' <remarks></remarks>
    Public Shared Function DeletaTipoLogradouroDEPARA(ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.DELETE_DEPARA_DELETATIPOLOGRADOURO
            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Insere dados de tipo de logradouro no DEPARA
    ''' </summary>
    ''' <returns>quantidade de dados inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereTipoLogradouroDEPARA(ByVal pCodMARTE As String, ByVal pCodNOVI As String, ByVal pCodPROFAT As String, ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.INSERT_DEPARA_NORMALIZATIPOLOGRADOURO

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "OID_TABELA", DbType.String, Guid.NewGuid.ToString))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_MARTE", DbType.String, pCodMARTE))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_PROFAT", DbType.String, pCodPROFAT))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_NOVI", DbType.String, pCodNOVI))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Insere dados de tipo de logradouro no MARTE
    ''' </summary>
    ''' <returns>quantidade de dados inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereTipoLogradouroMARTE(ByVal pDescLogradouro As String, ByVal pCodLogradouro As String) As Integer
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)

            Dim CodMarte As String = RetornaUltimoCodMARTE()
            cmd.CommandText = My.Resources.INSERT_MARTE_INSERETIPOLOG

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "OID_TIPO_CALLE", DbType.String, Guid.NewGuid.ToString))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_TIPO_CALLE", DbType.String, CodMarte))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "DES_TIPO_CALLE", DbType.String, pCodLogradouro))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "DES_CORTA_TIPO_CALLE", DbType.String, pDescLogradouro))

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
            Return CodMarte
        End Using
    End Function

    ''' <summary>
    ''' Insere dados de tipo de logradouro no PROFAT
    ''' </summary>
    ''' <returns>quantidade de dados inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereTipoLogradouroPROFAT(ByVal pDescLogradouro As String, ByVal pCodLogradouro As String) As Integer
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)

            Dim CodProfat As String = RetornaUltimoCodPROFAT()
            cmd.CommandText = My.Resources.INSERT_PROFAT_INSERETIPOLOG

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "CODTIPLOG", DbType.Int32, CodProfat))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "DESTIPLOG", DbType.String, pDescLogradouro))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "DESABRTIPLOG", DbType.String, Geral.RemoverAcentuacao(pCodLogradouro).ToUpper))

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
            Return CodProfat
        End Using
    End Function

    ''' <summary>
    ''' Retorna ultimo codigo valido da tabela de logradouro do PROTAT 
    ''' </summary>
    ''' <returns>Ultimo codigo valido</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaUltimoCodPROFAT() As Integer
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_PROFAT_ULTIMOCODIGOLOGRADOURO
            Return DbHelper.AcessoDados.ExecutarScalar(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna ultimo codigo valido da tabela de logradouro do MARTE 
    ''' </summary>
    ''' <returns>Ultimo codigo valido</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaUltimoCodMARTE() As String
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_ULTIMOCODIGOLOGRADOURO
            Return DbHelper.AcessoDados.ExecutarScalar(CONEXAO_MARTE, cmd).ToString
        End Using
    End Function

    ''' <summary>
    ''' Verifica se há duplicidade de dados no MARTE
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub VerificaDuplicidadeLogradouroMARTE()
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            Dim dt As New DataTable
            cmd.CommandText = My.Resources.SELECT_MARTE_VERIFICADUPLICIDADETIPOLOGRADOURO
            DeletaLogradouroMARTE(DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd))
        End Using
    End Sub

    ''' <summary>
    ''' Deleta dados duplicados do MARTE
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub DeletaLogradouroMARTE(ByVal pDados As DataTable)
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            For Each dr As DataRow In pDados.Rows
                cmd.CommandText = My.Resources.DELETE_MARTE_DELETALOGRADOURODUPLICADO
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "DES_CORTA_TIPO_CALLE", DbType.String, dr("DES_CORTA_TIPO_CALLE")))
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
            Next
        End Using
    End Sub

    ''' <summary>
    ''' Verifica se há duplicidade de dados no PROFAT
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub VerificaDuplicidadeLogradouroPROFAT()
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            Dim dt As New DataTable
            cmd.CommandText = My.Resources.SELECT_PROFAT_VERIFICADUPLICIDADETIPOLOGRADOURO
            DeletaLogradouroPROFAT(DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd))
        End Using
    End Sub

    ''' <summary>
    ''' Deleta dados duplicados do PROFAT
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub DeletaLogradouroPROFAT(ByVal pDados As DataTable)
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            For Each dr As DataRow In pDados.Rows
                cmd.CommandText = My.Resources.DELETE_PROFAT_DELETALOGRADOURODUPLICADO
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "DESABRTIPLOG", DbType.Int32, dr("DESABRTIPLOG")))
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
            Next
        End Using
    End Sub

#End Region

#Region "DELEGAÇÃO"

    ''' <summary>
    ''' Retorna as delegações do PROFAT
    ''' </summary>
    ''' <returns>Tabela com os dados de delegação</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaDelegacaoPROFAT() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_PROFAT_BUSCARDELEGACAO
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Atualiza dados de delegação no MARTE
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros atualizados</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaDelegacaoMARTE(ByVal pUFProfat As String, ByVal pDESProfat As String, ByVal pCodProfat As String, ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.UPDATE_MARTE_ATUALIZACODDELEGACAO

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "UF_PROFAT", DbType.String, pUFProfat))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "DES_PROFAT", DbType.String, pDESProfat))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PROFAT", DbType.String, pCodProfat.PadLeft(2, "0")))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
            End Using
        Catch ex As Exception
            AtualizaDelegacaoMARTE_Ajuste(pUFProfat, pDESProfat, pCodProfat, objtransacao)
            Return 1
        End Try

    End Function

    Public Shared Sub AtualizaDelegacaoMARTE_Ajuste(ByVal pUFProfat As String, ByVal pDESProfat As String, ByVal pCodProfat As String, ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.UPDATE_MARTE_ATUALIZACODDELEGACAO_AJUSTE

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PROVISORIO", DbType.String, "_" & pCodProfat.PadLeft(2, "0")))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PROFAT", DbType.String, pCodProfat.PadLeft(2, "0")))

                If DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd) > 0 Then
                    AtualizaDelegacaoMARTE(pUFProfat, pDESProfat, pCodProfat, objtransacao)
                End If

            End Using
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    ''' <summary>
    ''' Atualiza dados de delegação no NOVI
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros atualizados</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaDelegacaoNOVI(ByVal pUFProfat As String, ByVal pCodProfat As String, ByVal pDescDelegacaoProfat As String, ByRef objtransacao As IDbTransaction) As Integer

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.UPDATE_NOVI_ATUALIZACODDELEGACAO

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_PROFAT", DbType.String, pCodProfat.PadLeft(2, "0")))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "UF_PROFAT", DbType.String, pUFProfat))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "DES_PROFAT", DbType.String, pDescDelegacaoProfat.ToUpper))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "DES_PROFAT2", DbType.String, Geral.RemoverAcentuacao(pDescDelegacaoProfat).ToUpper))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            AtualizaDelegacaoNOVI_Ajuste(pUFProfat, pCodProfat, pDescDelegacaoProfat, objtransacao)
            Return 1
        End Try

    End Function


    Public Shared Sub AtualizaDelegacaoNOVI_Ajuste(ByVal pUFProfat As String, ByVal pCodProfat As String, ByVal pDescDelegacaoProfat As String, ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.UPDATE_NOVI_ATUALIZACODDELEGACAO_AJUSTE

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_PROVISORIO", DbType.String, "_" & pCodProfat.PadLeft(2, "0")))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_PROFAT", DbType.String, pCodProfat.PadLeft(2, "0")))

                If DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd) > 0 Then
                    AtualizaDelegacaoNOVI(pUFProfat, pCodProfat, pDescDelegacaoProfat, objtransacao)
                End If

            End Using
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    ''' <summary>
    ''' Atualiza dados de delegação no NOVI - tabela DDLVIG.ODERE
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros atualizados</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaDelegacaoNOVI_Ref(ByVal pUFProfat As String, ByVal pCodProfat As String, ByVal pDescDelegacaoProfat As String, ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.UPDATE_NOVI_ATUALIZACODDELEGACAO_REFERENCIA

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_PROFAT", DbType.String, pCodProfat.PadLeft(2, "0")))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "UF_PROFAT", DbType.String, pUFProfat))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "DES_PROFAT", DbType.String, pDescDelegacaoProfat.ToUpper))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "DES_PROFAT2", DbType.String, Geral.RemoverAcentuacao(pDescDelegacaoProfat).ToUpper))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Insere dados de delegação no MARTE
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereDelegacaoMARTE(ByVal pCodProfat As String, ByVal pUFProfat As String, ByVal pDescDelegacaoProfat As String, ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.INSERT_MARTE_INSEREDELEGACAO

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "OID_DELEGACION", DbType.String, Guid.NewGuid.ToString))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_DELEGACION", DbType.String, pCodProfat.PadLeft(2, "0")))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "DES_DELEGACION", DbType.String, pUFProfat))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "DES_CORTA_DELEGACION", DbType.String, pDescDelegacaoProfat))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Insere dados de delegação no NOVI
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereDelegacaoNOVI(ByVal pCodProfat As String, ByVal pUFProfat As String, ByVal pDescDelegacaoProfat As String, ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.INSERT_NOVI_INSEREDELEGACAO

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "OREGI_COD_REG", DbType.String, pCodProfat.PadLeft(2, "0")))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "OREGI_NOC_REG", DbType.String, pUFProfat.Trim))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "OREGI_NOL_REG", DbType.String, pDescDelegacaoProfat.Trim))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Insere dados de delegação na tabela de DePara
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros inseridos</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereDelegacaoDePara(ByVal pCodPROFAT As String, ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao

            cmd.CommandText = My.Resources.INSERT_DEPARA_NORMALIZADELEGACAO

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "OID_TABELA", DbType.String, Guid.NewGuid.ToString))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PROFAT", DbType.String, pCodPROFAT))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_MARTE", DbType.String, pCodPROFAT.PadLeft(2, "0")))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_NOVI", DbType.String, pCodPROFAT.PadLeft(2, "0")))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Apaga dados de delegação no NOVI
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros deletados</returns>
    ''' <remarks></remarks>
    Public Shared Function DeletaDelegacaoNOVI(ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.DELETE_NOVI_DELETADELEGACAO
            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Apaga dados de delegação na tabela de DePara
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros deletados</returns>
    ''' <remarks></remarks>
    Public Shared Function DeletaDelegacaoDePara(ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.DELETE_DEPARA_DELETADELEGACAO
            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Apaga dados de delegação no MARTE
    ''' </summary>
    ''' <returns>inteiro com quantidade de registros deletados</returns>
    ''' <remarks></remarks>
    Public Shared Function DeletaDelegacaoMARTE(ByRef objtransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.DELETE_MARTE_DELETADELEGACAO
            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Function

#End Region

#Region "FILIAL"

    ''' <summary>
    ''' Retorna as Filiais do MARTE
    ''' </summary>
    ''' <returns>Tabela com os dados de filial</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaFiliaisMARTE() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_BUSCARFILIAIS
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna as dados Filiais no DePARA do MARTE/PROFAT
    ''' </summary>
    ''' <returns>Tabela com os dados de filial</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaCODFilialPROFAT(ByVal pCodFilialMarte As String) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_PROFAT_BUSCACODFILIAL

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_COMERCIAL", DbType.String, pCodFilialMarte))

            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna as Filiais do PROFAT
    ''' </summary>
    ''' <returns>Tabela com os dados de filial</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaFilialPROFAT(ByVal pCodFilial As String) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_PROFAT_BUSCAFILIAL

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "CODFIL", DbType.Int32, Convert.ToInt32(pCodFilial)))

            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna as Filiais do PROFAT
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub AtualizaFilialNOVI(ByVal pCodComercial As String, ByVal pCodNOVI As String, ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao

                cmd.CommandText = My.Resources.UPDATE_NOVI_ATUALIZACODCOMERCIAL

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_COMERCIAL", DbType.String, pCodComercial))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_NOVI", DbType.String, pCodNOVI))

                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    ''' <summary>
    ''' Insere na tabela de DePara Faturamento/Marte
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub InsereDeParaFaturamento(ByVal pCodComercial As String, ByVal pCodEmpresaComercial As String, ByVal pCodFilialFat As String, ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                Dim sSql As String = My.Resources.INSERT_MARTE_INSEREDEPARAFILIALFAT

                'APESAR DO NOME AQUI É NA VERDADE O COD_PARAM_TAB QUE ESTÁ TRADUZINDO PARA EMPRESA B13 OU B20.
                If pCodEmpresaComercial = "4" Then
                    pCodEmpresaComercial = "B13"
                Else
                    pCodEmpresaComercial = "B20"
                End If

                sSql = sSql.Replace("PAR1", pCodComercial.PadLeft(2, "0"))
                sSql = sSql.Replace("PAR2", pCodEmpresaComercial)
                sSql = sSql.Replace("PAR3", pCodFilialFat)

                cmd.CommandText = sSql
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    ''' <summary>
    ''' Insere na tabela de espelho Faturamento/Marte
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub InsereTabelaEspelhoFaturamento(ByVal pCodFilial As String, ByVal pDesFilial As String, ByVal pCNPJ As String, ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                Dim sSql As String = My.Resources.INSERT_MARTE_INSEREESPELHOFILIALFAT

                sSql = sSql.Replace("PAR1", pCodFilial)
                sSql = sSql.Replace("PAR2", pDesFilial)
                sSql = sSql.Replace("PAR3", If(String.IsNullOrEmpty(pCNPJ), " ", pCNPJ))

                cmd.CommandText = sSql
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Insere dados na tabela ODERE, join entre Filial e delegação
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub InsereOdere(ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.INSERT_NOVI_ODERE
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Deleta dados da tabela ODERE, join entre Filial e delegação
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub DeleteOdere(ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.DELETE_NOVI_ODERE
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

#Region "TIPO DE POSTO"

    ''' <summary>
    ''' Retorna os Tipos de postos do MARTE
    ''' </summary>
    ''' <returns>Tabela com os dados de Tipos de postos</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaTipoPostoMARTE() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_DADOSTIPOPOSTO
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Atualiza o cod comercial do tipo de posto no NOVI
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub AtualizaCodComercialTipoPostoNOVI(ByVal pCodComercial As String, ByVal pCodNOVI As String, ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao

                cmd.CommandText = My.Resources.UPDATE_NOVI_ATUALIZACODCOMERCIAL1

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_COMERCIAL", DbType.String, pCodComercial))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_NOVI", DbType.String, pCodNOVI))

                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    ''' <summary>
    ''' Atualiza ou Insere dados de tipo de posto no PROFAT
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub MergeTipoPostoPROFAT(ByVal pCodNOVI As String, ByVal pDescTipoPosto As String, ByVal pCodComercial As String, ByRef objtransacao As IDbTransaction)

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao

                Dim sSql As String = My.Resources.INSERT_PROFAT_MERGETIPOPOSTO

                sSql = sSql.Replace("PAR1", pCodNOVI)
                sSql = sSql.Replace("PAR2", pDescTipoPosto.Replace("'", ""))
                sSql = sSql.Replace("PAR3", pCodComercial)

                cmd.CommandText = sSql
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
            End Using
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

#End Region

#Region "SEGMENTO/SUBSEGMENTO"

    ''' <summary>
    ''' Retorna os Segmentos do MARTE
    ''' </summary>
    ''' <returns>Tabela com os dados de Segmentos</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSegmentosMARTE(ByVal pCodSegmento As String) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            If Not String.IsNullOrEmpty(pCodSegmento) Then
                cmd.CommandText = String.Format(My.Resources.SELECT_DEPARA_SEGMENTO, " AND A.COD_PROFAT = '" & pCodSegmento & "'")
            Else
                cmd.CommandText = String.Format(My.Resources.SELECT_DEPARA_SEGMENTO, pCodSegmento)
            End If
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna os Segmentos do PROFAT
    ''' </summary>
    ''' <returns>Tabela com os dados de Segmentos</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSegmentosPROFAT() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_PROFAT_DADOSSEG_SUB
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna os SubSegmentos do MARTE
    ''' </summary>
    ''' <returns>Tabela com os dados de SubSegmentos</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSubSegmentosMARTE(ByVal pCodSubSegmento As String, ByVal pOIDSegmento As String) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            Dim dt As New DataTable

            If Not String.IsNullOrEmpty(pCodSubSegmento) Then
                cmd.CommandText = String.Format(My.Resources.SELECT_DEPARA_SUBSEGMENTO, " AND A.COD_PROFAT = '" & pCodSubSegmento & "' AND S.OID_SEGMENTO = '" & pOIDSegmento & "'")
            Else
                cmd.CommandText = String.Format(My.Resources.SELECT_DEPARA_SUBSEGMENTO, pCodSubSegmento)
            End If

            dt = DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Return dt
            Else
                cmd.CommandText = String.Format(My.Resources.SELECT_MARTE_SEGSUBSEG, " AND SUB.COD_SUBSEGMENTO = '" & pCodSubSegmento & "' AND S.OID_SEGMENTO = '" & pOIDSegmento & "'")
                Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
            End If

        End Using
    End Function

    ''' <summary>
    ''' Retorna os SubSegmentos do PROFAT
    ''' </summary>
    ''' <returns>Tabela com os dados de SubSegmentos</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSegmentosPROFAT(ByVal pcodProfat As Integer) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_PROFAT_SEGMENTO
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "CODRAMATV", DbType.String, pcodProfat))
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna os SubSegmentos do PROFAT
    ''' </summary>
    ''' <returns>Tabela com os dados de SubSegmentos</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaSubSegmentosPROFAT(ByVal pcodProfat As Integer) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_PROFAT_SUBSEGMENTO
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "CODSUBRAMATV", DbType.String, pcodProfat))
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Atualiza ou Insere dados de tipo de posto no PROFAT
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function MergeSegmentoMARTE(ByVal pCodSegmento As String, ByVal pDescSegmento As String, _
                                         ByVal pCodSegmentoPROFAT As String, ByVal pOIDSegmento As String, ByRef objtransacao As IDbTransaction) As String

        Dim sGUID As String = Guid.NewGuid.ToString

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao

                Dim sSql As String = My.Resources.MERGE_MARTE_SEGMENTO


                sSql = sSql.Replace("PAR1", pCodSegmento)
                sSql = sSql.Replace("PAR2", pDescSegmento)
                sSql = sSql.Replace("PAR3", pCodSegmentoPROFAT)
                sSql = sSql.Replace("PAR4", pOIDSegmento)
                sSql = sSql.Replace("PAR5", sGUID)

                cmd.CommandText = sSql
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)

            End Using
        Catch ex As Exception
            Throw ex
        End Try

        If String.IsNullOrEmpty(pOIDSegmento) Then
            Return sGUID
        Else
            Return String.Empty
        End If

    End Function

    ''' <summary>
    ''' Atualiza ou Insere dados de tipo de posto no PROFAT
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function MergeSubSegmentoMARTE(ByVal pCodSubSegmento As String, ByVal pDescSubSegmento As String, _
                                                ByVal pCodSubSegmentoPROFAT As String, ByVal pOIDSegmento As String, _
                                                ByVal pOIDSubSegmento As String, ByRef objtransacao As IDbTransaction) As String

        Dim sGUID As String = Guid.NewGuid.ToString
        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao

                Dim sSql As String = My.Resources.MERGE_MARTE_SUBSEGMENTO


                sSql = sSql.Replace("PAR1", pCodSubSegmento)
                sSql = sSql.Replace("PAR2", pDescSubSegmento)
                sSql = sSql.Replace("PAR3", pCodSubSegmentoPROFAT)
                sSql = sSql.Replace("PAR4", pOIDSegmento)
                sSql = sSql.Replace("PAR5", pOIDSubSegmento)
                sSql = sSql.Replace("PAR6", sGUID)

                cmd.CommandText = sSql
                DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)

            End Using
        Catch ex As Exception
            Throw ex
        End Try

        If String.IsNullOrEmpty(pOIDSubSegmento) Then
            Return sGUID
        Else
            Return String.Empty
        End If

    End Function

    ''' <summary>
    ''' Insere dados de dePara 
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub InsereDeParaSegmentoSubSeg(ByVal pcodMarte As String, ByVal pcodProfat As String, ByVal pcodParam As String, ByRef objtransacao As IDbTransaction)

        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.INSERT_DEPARA_SEGMENTO_SUB

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_MARTE", DbType.String, pcodMarte.Trim()))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PROFAT", DbType.String, pcodProfat))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PARAM_TAB", DbType.String, pcodParam))

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Sub

    ''' <summary>
    ''' Deleta dados de dePara 
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub DeletaDeParaSegmentoSubSeg(ByVal pcodParam As String, ByRef objtransacao As IDbTransaction)

        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.DELETE_DEPARA_SEGMENTO_SUB

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PARAM_TAB", DbType.String, pcodParam))

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Sub

    Public Shared Sub InsereSegmentoPadrao(ByRef objTransacao As IDbTransaction)

        Using cmd As IDbCommand = objTransacao.Connection.CreateCommand()
            cmd.Transaction = objTransacao
            cmd.CommandText = My.Resources.INSERT_SEGMENTO_MARTE

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using

    End Sub

    Public Shared Sub InsereSubSegmentoPadrao(ByRef objTransacao As IDbTransaction)

        Using cmd As IDbCommand = objTransacao.Connection.CreateCommand()
            cmd.Transaction = objTransacao
            cmd.CommandText = My.Resources.INSERT_SUBSEGMENTO_MARTE

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using

    End Sub

    Public Shared Sub AtualizarSegmentoSubsegmentoCliente(ByRef objTransacao As IDbTransaction)

        Using cmd As IDbCommand = objTransacao.Connection.CreateCommand()
            cmd.Transaction = objTransacao
            cmd.CommandText = My.Resources.UPDATE_CLIENTE_MARTE

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using

    End Sub

    Public Shared Sub ApagarSegmento(ByRef objTransacao As IDbTransaction)
        Using cmd As IDbCommand = objTransacao.Connection.CreateCommand()
            cmd.Transaction = objTransacao
            cmd.CommandText = My.Resources.DELETE_MARTE_SEGMENTO

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Sub

    Public Shared Sub ApagarSubSegmento(ByRef objTransacao As IDbTransaction)
        Using cmd As IDbCommand = objTransacao.Connection.CreateCommand()
            cmd.Transaction = objTransacao
            cmd.CommandText = My.Resources.DELETE_MARTE_SUBSEGMENTO

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Sub
#End Region

#Region "CIDADE"

    ''' <summary>
    ''' Retorna os Cidades do MARTE
    ''' </summary>
    ''' <returns>Tabela com os dados de Cidades</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaCidadesMARTE() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_DADOSCIDADE
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna os Cidades do MARTE sem DEPARA
    ''' </summary>
    ''' <returns>Tabela com os dados de Cidades</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaCidadesSemDeparaMARTE() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.SELECT_MARTE_CIDADESEMDEPARA
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna os Cidades do PROFAT
    ''' </summary>
    ''' <returns>Tabela com os dados de Cidades</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaCidadesPROFAT(ByVal pParam As String) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = String.Format(My.Resources.SELECT_PROFAT_DADOSCIDADE, pParam)
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Atualiza o codigo do IBGE na tabela do NOVI de cidades
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaCodIBGENOVI(ByVal pcodMarte As String, ByVal pcodIBGE As String, ByRef objtransacao As IDbTransaction) As Int32
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.UPDATE_NOVI_ATUALIZACIDADE

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_CIUDAD", DbType.String, pcodMarte))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_IBGE", DbType.String, pcodIBGE))

            'Log.GravarLog(cmd.CommandText, pNomeArquivoLog)

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Atualiza o codigo do IBGE na tabela do MARTE de cidades
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaCodIBGEMARTE(ByVal pcodMarte As String, ByVal pcodIBGE As String, ByRef objtransacao As IDbTransaction) As Int32
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao

            cmd.CommandText = My.Resources.UPDATE_MARTE_ATUALIZACIDADE

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_CIUDAD", DbType.String, pcodMarte))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_IBGE", DbType.String, pcodIBGE))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Deleta cidades sem DEPARA do NOVI
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function DeletaCidadesSemDeparaNOVI(ByRef objtransacao As IDbTransaction) As Int32
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_NOVI)
            cmd.CommandText = My.Resources.DELETE_NOVI_CIDADESEMDEPARA
            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Deleta cidades sem DEPARA
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function DeletaCidadesSemDepara(ByRef objtransacao As IDbTransaction) As Int32
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = My.Resources.DELETE_DEPARA_CIDADESEMDEPARA
            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Atualiza o codigo do IBGE na tabela do MARTE de cidades
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereCidadeDEPARA(ByVal pCodMarte As String, ByVal pCodProfat As String, ByVal pCodNOVI As String, ByRef objtransacao As IDbTransaction) As Int32
        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao
                cmd.CommandText = My.Resources.INSERT_DEPARA_CIDADE

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "ID", DbType.String, Guid.NewGuid.ToString))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_MARTE", DbType.String, pCodMarte))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PROFAT", DbType.String, pCodProfat))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_NOVI", DbType.String, pCodNOVI))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
            End Using
        Catch ex As Exception
            'Caso acontece algum erro apenas abafa
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Insere dados de cidade no NOVI
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereCidadeNOVI(ByVal pcodMarte As String, ByVal pDesCidade As String, ByVal pcodIBGE As String, ByRef objtransacao As IDbTransaction) As Int32

        Try
            Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
                cmd.Transaction = objtransacao

                cmd.CommandText = My.Resources.INSERT_NOVI_CIDADE

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_NOVI", DbType.String, pcodMarte))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "DES_MUN", DbType.String, pDesCidade))
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_COMERCIAL", DbType.String, pcodIBGE))

                Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
            End Using
        Catch ex As Exception
            'Caso acontece algum erro apenas abafa
            Return 0
        End Try
    End Function

    Public Shared Function DesabilitarTriggerCidadeMARTE(ByRef objtransacao As IDbTransaction) As Int32
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao

            cmd.CommandText = My.Resources.ALTER_TABLE_MARTE_DESABILITA_TRIGGER

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Function

    Public Shared Function HabilitarTriggerCidadeMARTE(ByRef objtransacao As IDbTransaction) As Int32
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao

            cmd.CommandText = My.Resources.ALTER_TABLE_MARTE_HABILITA_TRIGGER

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Function


#End Region

#Region "DIVISÃO"

    ''' <summary>
    ''' Retorna os Cidades do MARTE
    ''' </summary>
    ''' <returns>Tabela com os dados de divisão</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaDivisaoMARTE() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)

            cmd.CommandText = My.Resources.SELECT_MARTE_DADOSDIVISAO
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna os Cidades do PROFAT
    ''' </summary>
    ''' <returns>Tabela com os dados de Cidades</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaDivisaoPROFAT(ByVal pCod As Integer) As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_PROFAT)
            cmd.CommandText = My.Resources.SELECT_PROFAT_DADOSDIVISAO

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "CODTIPSER", DbType.String, pCod))

            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Retorna os ultimo cod NOVI
    ''' </summary>
    ''' <returns>proximo codigo de divisão</returns>
    ''' <remarks></remarks>
    Public Shared Function RetornaCodDivisaoNovi() As String
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_NOVI)
            cmd.CommandText = "SELECT NVL(MAX(TO_NUMBER(ODIVI_COD_DIVI))+1,'01') COD FROM ADA_ODIVI"
            Return DbHelper.AcessoDados.ExecutarScalar(CONEXAO_NOVI, cmd).ToString
        End Using
    End Function

    ''' <summary>
    ''' Atualiza o codigo do IBGE na tabela do NOVI de cidades
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function InsereDivisaoNOVI(ByVal pcodMarte As String, ByVal pDesDivisao As String, ByRef objtransacao As IDbTransaction) As Int32
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.INSERT_NOVI_DIVISAO

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "ODIVI_COD_DIVI", DbType.String, RetornaCodDivisaoNovi()))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "ODIVI_NOC_DIVI", DbType.String, pDesDivisao.ToString.ToUpper))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "ODIVI_NOL_DIVI", DbType.String, pDesDivisao.ToString.ToUpper))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_COMERCIAL", DbType.String, pcodMarte))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Atualiza o codigo do IBGE na tabela do NOVI de cidades
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaCodComercialDivisaoNOVI(ByVal pcodMarte As String, ByVal pcodDivisao As String, ByRef objtransacao As IDbTransaction) As Int32
        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.ATUALIZA_NOVI_DIVISAOCODCOMERCIAL

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_COMERCIAL", DbType.String, pcodMarte))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_NOVI, "COD_DIV", DbType.String, pcodDivisao))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_NOVI, cmd)
        End Using
    End Function

    ''' <summary>
    ''' Atualiza o codigo do IBGE na tabela do NOVI de cidades
    ''' </summary>
    ''' <returns>Quantidade de registros afetados</returns>
    ''' <remarks></remarks>
    Public Shared Function AtualizaCodComercialDivisaoPROFAT(ByVal pcodMarte As String, ByVal pcodItemCod As String, ByRef objtransacao As IDbTransaction) As Int32

        Using cmd As IDbCommand = objtransacao.Connection.CreateCommand()
            cmd.Transaction = objtransacao
            cmd.CommandText = My.Resources.ATUALIZA_PROFAT_DIVISAOCODCOMERCIAL

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "CODSEGCOM", DbType.String, pcodMarte))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_PROFAT, "CODITECOB", DbType.Int32, pcodItemCod))

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
        End Using

    End Function

#End Region

#Region "ESCALA"
    ''' <summary>
    ''' Busca dados de ESCALA do MARTE
    ''' </summary>
    ''' <returns>tabela com todos os dados de ESCALA</returns>
    ''' <remarks></remarks>
    Public Shared Function BuscarEscalaMARTE() As DataTable
        Using cmd As IDbCommand = DbHelper.AcessoDados.CriarComando(CONEXAO_MARTE)
            cmd.CommandText = String.Format(My.Resources.SELECT_ESCALA_MARTE, String.Empty)
            Return DbHelper.AcessoDados.ExecutarDataTable(CONEXAO_MARTE, cmd)
        End Using
    End Function

    Public Shared Function MergeEscalaProfat(pCodMARTE As String,
                                             pDesEsc As String,
                                             pCodEsc As String,
                                             ByRef objTransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objTransacao.Connection.CreateCommand
            cmd.Transaction = objTransacao

            Dim sSQL As String = My.Resources.MERGE_ESCALA_PROFAT

            sSQL = sSQL.Replace("PAR_CODMARTE", pCodMARTE)
            sSQL = sSQL.Replace("PAR_DESESC", pDesEsc)
            sSQL = sSQL.Replace("PAR_CODESC", pCodEsc)

            cmd.CommandText = sSQL

            Return DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_PROFAT, cmd)
        End Using
    End Function

    Public Shared Function InsereEscalaDePara(pOID_MARTE As String, pCodProfat As String, objTransacao As IDbTransaction) As Integer
        Using cmd As IDbCommand = objTransacao.Connection.CreateCommand()
            cmd.Transaction = objTransacao
            cmd.CommandText = My.Resources.INSERT_ESCALA_DEPARA

            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "OID_MARTE", DbType.String, pOID_MARTE))
            cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(CONEXAO_MARTE, "COD_PROFAT", DbType.String, pCodProfat))

            DbHelper.AcessoDados.ExecutarNonQuery(CONEXAO_MARTE, cmd)
        End Using
    End Function

#End Region


End Class
