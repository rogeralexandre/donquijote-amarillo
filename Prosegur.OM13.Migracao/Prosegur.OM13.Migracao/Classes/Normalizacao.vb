Imports Prosegur.DbHelper
Imports System.Data.SqlClient

Public Class Normalizacao

#Region "CLIENTE"

    Public Shared Function NormalizaCliente(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_NOVI As IDbTransaction
        Dim objtransacao_PROFAT As IDbTransaction
        Dim UsaWS As Boolean = CType(System.Configuration.ConfigurationManager.AppSettings("HabilitarWSPROFAT"), Boolean)

        Try

            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE CLIENTE ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE CLIENTE")

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            Dim conn_PROFAT As New SqlConnection(AcessoDados.RecuperarStringConexao(Dados.CONEXAO_PROFAT))
            conn_PROFAT.Open()

            objtransacao_NOVI = conn_NOVI.BeginTransaction
            objtransacao_PROFAT = conn_PROFAT.BeginTransaction

            Dim dtClientes As New DataTable
            Dim ws As New WSPROFAT.ClienteMarteProfat
            Dim DadosCliente As New WSPROFAT.DadosWsCliente
            Dim wsRet As New WSPROFAT.Retorno
            Dim cont As Integer = 0

            AlteraStatusProcessamento("PROCESSANDO... ")
            dtClientes = Dados.RetornaClientesDEPara()
            For Each dr As DataRow In dtClientes.Rows

                If Not String.IsNullOrEmpty(dr.Item("COD_PROFAT").ToString) Then
                    If UsaWS Then

                        Log.GravarLog("FAZENDO A CHAMADA DO WS PARA ATUALIZAR O CLIENTE PROFAT = " & dr.Item("COD_PROFAT").ToString, pNomeArquivoLog)

                        Dim tipoInscricao As String = ""
                        If dr.Item("OID_TIPO_ID_FISCAL").ToString.ToUpper.Trim = "CNPJ" Then
                            tipoInscricao = "2"
                        Else
                            tipoInscricao = "1"
                        End If

                        DadosCliente.CodigoClientePROFAT = dr.Item("COD_PROFAT").ToString
                        DadosCliente.CEP = dr.Item("COD_CEP").ToString
                        DadosCliente.TipoLogradouro = dr.Item("DES_TIPO_CALLE").ToString
                        DadosCliente.Endereco = dr.Item("DES_CALLE").ToString
                        DadosCliente.NumeroEndereco = dr.Item("NUM_NRO").ToString
                        DadosCliente.Complemento = dr.Item("DES_COMPL_DIRECCION").ToString
                        DadosCliente.Bairro = dr.Item("DES_BARRIO").ToString
                        DadosCliente.Cidade = dr.Item("DES_CIUDAD").ToString
                        DadosCliente.UF = dr.Item("ESTADO").ToString
                        DadosCliente.IndicacaoCpfCnpj = tipoInscricao
                        DadosCliente.CnpjCpf = dr.Item("COD_IDENTIFICACION_FISCAL").ToString
                        DadosCliente.NomeAbreviado = dr.Item("DES_NOMBRE_FANTASIA").ToString
                        DadosCliente.RazaoSocial = dr.Item("DES_RAZON_SOCIAL").ToString
                        DadosCliente.SituacaoCliente = dr.Item("BOL_ACTIVO").ToString

                        wsRet = ws.AtualizarDadosClienteVA(DadosCliente)

                        If wsRet.CodigoRetorno <> 1 Then
                            Log.GravarLog("-----------------------------------------------------------", pNomeArquivoLog)
                            Log.GravarLog("ERRO AO TENTAR ATUALIZAR O CLIENTE: " & dr.Item("COD_CLIENTE").ToString, pNomeArquivoLog)
                            For Each msgRet As WSPROFAT.Mensagens In wsRet.Lista_Retorno
                                Log.GravarLog("Cod Retorno WSPROFAT = " & msgRet.Codigo & vbNewLine & _
                                              "Mensagem retorno = " & msgRet.Mensagem & vbNewLine & _
                                              "Detalhe = " & msgRet.Detalhe, pNomeArquivoLog)
                            Next
                            Log.GravarLog("-----------------------------------------------------------", pNomeArquivoLog)
                        Else
                            Log.GravarLog("CLIENTE ATUALIZADO COM SUCESSO", pNomeArquivoLog)
                        End If

                    End If

                    Log.GravarLog("ATUALIZOU NA TABELA 'FAT_TCADCLI' CODCLICOM = " & dr.Item("COD_CLIENTE").ToString, pNomeArquivoLog)
                    Dados.AtualizaCodComercialClientePROFAT(dr.Item("COD_CLIENTE").ToString, dr.Item("COD_PROFAT").ToString, objtransacao_PROFAT)
                End If

                Log.GravarLog("ATUALIZOU NA TABELA 'DDLVIG.OCLTE' COD_COMERCIAL = " & dr.Item("COD_CLIENTE").ToString, pNomeArquivoLog)
                Dados.AtualizaCodComercialClienteNOVI(dr.Item("COD_NOVI").ToString, dr.Item("COD_IDENTIFICACION_FISCAL").ToString, _
                                                      dr.Item("DES_NOMBRE_FANTASIA").ToString, dr.Item("DES_RAZON_SOCIAL").ToString, _
                                                      dr.Item("COD_NOVI").ToString, dr.Item("DES_TIPO_CALLE").ToString, objtransacao_NOVI)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dtClientes.Rows.Count)
            Next

            objtransacao_PROFAT.Commit()
            objtransacao_NOVI.Commit()

            Dim msgInfo As String = String.Empty
            Dim dtAux As New DataTable
            dtAux = Dados.RetornaClientesDuplicados()

            If dtAux.Rows.Count > 0 Then
                msgInfo = "CLIENTES COM NIF DUPLICADOS NO MARTE: " & vbNewLine
                Log.GravarLog("CLIENTES COM NIF DUPLICADOS NO MARTE:" & vbNewLine, pNomeArquivoLog)
                For Each drDupli As DataRow In dtAux.Rows
                    msgInfo &= "COD CLIENTE:" & drDupli.Item("COD_CLIENTE").ToString & " NIF:" & drDupli.Item("COD_IDENTIFICACION_FISCAL").ToString & vbNewLine
                    Log.GravarLog("COD CLIENTE:" & drDupli.Item("COD_CLIENTE").ToString & " NIF:" & drDupli.Item("COD_IDENTIFICACION_FISCAL").ToString & vbNewLine, pNomeArquivoLog)
                Next
            End If

            dtAux = Dados.RetornaClientesComNIFNulo
            If dtAux.Rows.Count > 0 Then
                msgInfo &= "------------------------------" & vbNewLine
                msgInfo &= "CLIENTES COM NIF NULO NO MARTE: " & vbNewLine
                Log.GravarLog("CLIENTES COM NIF NULO NO MARTE:" & vbNewLine, pNomeArquivoLog)
                For Each drNulo As DataRow In dtAux.Rows
                    msgInfo &= "COD CLIENTE:" & drNulo.Item("COD_CLIENTE").ToString & " RAZÃO SOCIAL:" & drNulo.Item("DES_RAZON_SOCIAL").ToString & vbNewLine
                    Log.GravarLog("COD CLIENTE:" & drNulo.Item("COD_CLIENTE").ToString & " RAZÃO SOCIAL:" & drNulo.Item("DES_RAZON_SOCIAL").ToString & vbNewLine, pNomeArquivoLog)
                Next
            End If

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE CLIENTE ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento(msgInfo & vbNewLine & "FIM DA NORMALIZAÇÃO DE CLIENTE")

        Catch ex As Exception
            objtransacao_PROFAT.Rollback()
            objtransacao_NOVI.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True

    End Function

#End Region

#Region "SUBCLIENTE"

    Public Shared Function NormalizaSubCliente(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_NOVI As IDbTransaction
        Dim objtransacao_PROFAT As IDbTransaction
        Dim UsaWS As Boolean = CType(System.Configuration.ConfigurationManager.AppSettings("HabilitarWSPROFAT"), Boolean)

        Try

            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE SUB CLIENTE ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE SUB CLIENTE")

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            Dim conn_PROFAT As New SqlConnection(AcessoDados.RecuperarStringConexao(Dados.CONEXAO_PROFAT))
            conn_PROFAT.Open()

            objtransacao_NOVI = conn_NOVI.BeginTransaction
            objtransacao_PROFAT = conn_PROFAT.BeginTransaction

            Dim dtSubClientes As New DataTable
            Dim dtResp As New DataTable
            Dim ws As New WSPROFAT.ClienteMarteProfat
            Dim DadosSubCliente As New WSPROFAT.DadosWsSubCliente
            Dim wsRet As New WSPROFAT.RetornoSubCliente
            Dim cont As Integer = 0

            AlteraStatusProcessamento("PROCESSANDO... ")
            dtSubClientes = Dados.RetornaSubClientesDEPara()
            For Each dr As DataRow In dtSubClientes.Rows

                If Not String.IsNullOrEmpty(dr.Item("COD_PROFAT").ToString) Then
                    If UsaWS Then

                        Dim tipoInscricao As String = ""
                        If dr.Item("OID_TIPO_ID_FISCAL").ToString.ToUpper.Trim = "CNPJ" Then
                            tipoInscricao = "2"
                        Else
                            tipoInscricao = "1"
                        End If

                        Log.GravarLog("FAZENDO A CHAMADA DO WS PARA ATUALIZAR O SUB CLIENTE PROFAT = " & dr.Item("COD_PROFAT").ToString, pNomeArquivoLog)
                        DadosSubCliente.CodigoPontoAtendimento = dr.Item("COD_PROFAT").ToString
                        DadosSubCliente.CEP = dr.Item("COD_CEP").ToString
                        DadosSubCliente.TipoLogradouro = dr.Item("DES_TIPO_CALLE").ToString
                        DadosSubCliente.Endereco = dr.Item("DES_CALLE").ToString
                        DadosSubCliente.NumeroEndereco = dr.Item("NUM_NRO").ToString
                        DadosSubCliente.Complemento = dr.Item("DES_COMPL_DIRECCION").ToString
                        DadosSubCliente.Bairro = dr.Item("DES_BARRIO").ToString
                        DadosSubCliente.Cidade = dr.Item("DES_CIUDAD").ToString
                        DadosSubCliente.UF = dr.Item("ESTADO").ToString
                        DadosSubCliente.IndicacaoCpfCnpj = tipoInscricao
                        DadosSubCliente.CnpjCpf = dr.Item("COD_IDENTIFICACION_FISCAL").ToString
                        DadosSubCliente.RazaoSocial = dr.Item("DES_SUBCLIENTE").ToString
                        DadosSubCliente.SituacaoPonto = dr.Item("BOL_ACTIVO").ToString
                        DadosSubCliente.Departamento = dr.Item("DES_FILIAL").ToString
                        DadosSubCliente.InsEst = dr.Item("COD_INSCRIPCION_ESTATAL").ToString
                        DadosSubCliente.InsMun = dr.Item("COD_INSCRIPCION_MUNICIPAL").ToString
                        DadosSubCliente.TipoPonto = Convert.ToInt32(dr.Item("COD_TIPO_SUBCLIENTE").ToString)

                        dtResp = Dados.RetornaResFat(dr.Item("OID_CLIENTE").ToString, dr.Item("COD_SUB_MARTE").ToString)
                        If Not dtResp Is Nothing Then
                            DadosSubCliente.Destinatario = dtResp.Rows(0).Item("DESTINATARIOMALADIRETA").ToString
                            DadosSubCliente.NomeResponsavelEmail = dtResp.Rows(0).Item("NOMERESPONSAVELNF").ToString
                            DadosSubCliente.Email = dtResp.Rows(0).Item("EMAILRESPONSAVELNF").ToString
                        End If

                        wsRet = ws.AtualizarDadosSubClienteVA(DadosSubCliente)

                        If wsRet.CodigoRetorno <> 1 Then
                            Log.GravarLog("-----------------------------------------------------------", pNomeArquivoLog)
                            Log.GravarLog("ERRO AO TENTAR ATUALIZAR O SUB CLIENTE: " & dr.Item("COD_SUB_MARTE").ToString, pNomeArquivoLog)
                            For Each msgRet As WSPROFAT.Mensagens In wsRet.Lista_Retorno
                                Log.GravarLog("Cod Retorno WSPROFAT = " & msgRet.Codigo & vbNewLine & _
                                              "Mensagem retorno = " & msgRet.Mensagem & vbNewLine & _
                                              "Detalhe = " & msgRet.Detalhe, pNomeArquivoLog)
                            Next
                            Log.GravarLog("-----------------------------------------------------------", pNomeArquivoLog)
                        Else
                            Log.GravarLog("SUB CLIENTE ATUALIZADO COM SUCESSO", pNomeArquivoLog)
                        End If
                    End If

                    Log.GravarLog("ATUALIZOU NA TABELA 'FAT_TCADPONATE' CODPONATE = " & dr.Item("COD_PROFAT").ToString & " CODPONATECOM = " & dr.Item("COD_SUB_MARTE").ToString, pNomeArquivoLog)
                    Dados.AtualizaCodComercialSubCliPROFAT(dr.Item("COD_SUB_MARTE").ToString, dr.Item("COD_PROFAT").ToString, objtransacao_PROFAT)
                End If

                Log.GravarLog("ATUALIZOU NA TABELA 'DDLVIG.OSBCL' CLIENTE E SUB NOVI: " & dr.Item("COD_CLI_NOVI").ToString & "/" & dr.Item("COD_SUB_NOVI").ToString & " COD_SUBCLI_COMERCIAL = " & dr.Item("COD_SUB_MARTE").ToString, pNomeArquivoLog)
                Dados.AtualizaCodComercialSubCliNOVI(dr.Item("DES_SUBCLIENTE").ToString, dr.Item("DES_CORTA_SUBCLIENTE").ToString, dr.Item("DES_TIPO_CALLE").ToString, _
                                                     dr.Item("COD_CLI_NOVI").ToString, dr.Item("COD_SUB_NOVI").ToString, dr.Item("COD_SUB_MARTE").ToString, objtransacao_NOVI)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dtSubClientes.Rows.Count)
            Next

            objtransacao_PROFAT.Commit()
            objtransacao_NOVI.Commit()

            Dim msgInfo As String = String.Empty
            Dim dtAux As New DataTable
            dtAux = Dados.RetornaSubClienteDuplicado()
            If dtAux.Rows.Count > 0 Then
                msgInfo = "SUB CLIENTES COM NIF DUPLICADOS NO MARTE: " & vbNewLine
                Log.GravarLog("SUB CLIENTES COM NIF DUPLICADOS NO MARTE:" & vbNewLine, pNomeArquivoLog)
                For Each drDupli As DataRow In dtAux.Rows
                    msgInfo &= "NIF:" & drDupli.Item("COD_IDENTIFICACION_FISCAL").ToString & " QUANTIDADE: " & drDupli.Item("QTD").ToString & vbNewLine
                    Log.GravarLog("NIF:" & drDupli.Item("COD_IDENTIFICACION_FISCAL").ToString & " QUANTIDADE: " & drDupli.Item("QTD").ToString & vbNewLine, pNomeArquivoLog)
                Next
            End If

            dtAux = Dados.RetornaSubClientesNIFNulo
            If dtAux.Rows.Count > 0 Then
                msgInfo &= "------------------------------" & vbNewLine
                msgInfo &= "SUB CLIENTES COM NIF NULO NO MARTE: " & vbNewLine
                Log.GravarLog("SUB CLIENTES COM NIF NULO NO MARTE:" & vbNewLine, pNomeArquivoLog)
                For Each drNulo As DataRow In dtAux.Rows
                    msgInfo &= "COD CLIENTE:" & drNulo.Item("COD_CLIENTE").ToString & "COD SUBCLIENTE:" & drNulo.Item("COD_SUBCLIENTE").ToString & vbNewLine
                    Log.GravarLog("COD CLIENTE:" & drNulo.Item("COD_CLIENTE").ToString & "COD SUBCLIENTE:" & drNulo.Item("COD_SUBCLIENTE").ToString & vbNewLine, pNomeArquivoLog)
                Next
            End If

            dtAux = Dados.RetornaSubClienteRazaoNIF
            If dtAux.Rows.Count > 0 Then
                msgInfo &= "------------------------------" & vbNewLine
                msgInfo &= "SUB CLIENTES COM RAZÃO DO NIF DIFERENTE DO CLIENTE: " & vbNewLine
                Log.GravarLog("SUB CLIENTES COM RAZÃO DO NIF DIFERENTE DO CLIENTE:" & vbNewLine, pNomeArquivoLog)
                For Each drNif As DataRow In dtAux.Rows
                    msgInfo &= "COD CLIENTE:" & drNif.Item("COD_CLIENTE").ToString & " NIF CLIENTE:" & drNif.Item("NIF_CLIENTE").ToString & _
                                "COD SUB CLIENTE:" & drNif.Item("COD_SUBCLIENTE").ToString & " NIF SUB CLIENTE:" & drNif.Item("NIF_SUBCLIENTE").ToString & vbNewLine
                    Log.GravarLog("COD CLIENTE:" & drNif.Item("COD_CLIENTE").ToString & " NIF CLIENTE:" & drNif.Item("NIF_CLIENTE").ToString & _
                                  "COD SUB CLIENTE:" & drNif.Item("COD_SUBCLIENTE").ToString & " NIF SUB CLIENTE:" & drNif.Item("NIF_SUBCLIENTE").ToString & vbNewLine, pNomeArquivoLog)
                Next
            End If

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE SUB CLIENTE ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento(msgInfo & vbNewLine & "FIM DA NORMALIZAÇÃO DE SUB CLIENTE")

        Catch ex As Exception
            objtransacao_PROFAT.Rollback()
            objtransacao_NOVI.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True

    End Function

#End Region

#Region "TIPO LOGRADOURO"

    Public Shared Function NormalizaTipoLogradouro(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_NOVI As IDbTransaction

        Try
            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE TIPO DE LOGRADOURO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE TIPO DE LOGRADOURO")

            Dim DadosMaestro As New DataTable
            Dim cont As Integer = 0

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            objtransacao_NOVI = conn_NOVI.BeginTransaction

            AlteraStatusProcessamento("PROCESSANDO...")
            Log.GravarLog("DADOS MAESTROS PROVENIENTES DO MARTE ----------------------------------------", pNomeArquivoLog)

            DadosMaestro = Dados.TipoLogradouroMARTE()
            Log.GravarLog("BUSCOU DADOS DO MARTE PARA A MEMORIA", pNomeArquivoLog)

            'Limpa a tabela de tipo de logradouro do NOVI e DePara
            'Dados.DeletaTipoLogradouroNOVI(objtransacao_NOVI)
            'Log.GravarLog("LIMPOU A TABELA DE LOGRADOURO DO NOVI", pNomeArquivoLog)

            'ROGER: Desliguei este BLOCO porque não tem porque mudar o DE_PARA e os códigos dos logradouros do MARTE.
            'Dados.DeletaTipoLogradouroDEPARA(objtransacao_NOVI)
            'Log.GravarLog("LIMPOU A TABELA DE LOGRADOURO DE DEPARA", pNomeArquivoLog)
            'Dados.AtualizaTipoLogradouroMARTE_TEMP(objtransacao_NOVI)

            For Each dr As DataRow In DadosMaestro.Rows

                'ROGER: Não vamos trocar os códigos do MARTE para serem iguais ao do PROFAT.
                'Log.GravarLog("ATUALIZOU O CODIGO DO PROFAT NO MARTE --- CODMARTE:" & dr("COD_TIPO_CALLE") & "PASSOU A SER CODPROFAT:" & dr.Item("COD_PROFAT"), pNomeArquivoLog)
                'Dados.AtualizaTipoLogradouroMARTE(dr.Item("COD_PROFAT"), dr.Item("COD_TIPO_CALLE"), objtransacao_NOVI)

                Log.GravarLog("INSERIU OS DADOS DE TIPO DE LOGRADOURO NO NOVI, COD_COMERCIAL recebeu o DES_TIPO_CALLE do MARTE. COD_NOVI:" & dr("COD_NOVI") & " COD_COMERCIAL=" & dr("DES_TIPO_CALLE"), pNomeArquivoLog)
                Dados.AtualizaTipoLogradouroNOVI(dr("COD_NOVI"), dr("DES_TIPO_CALLE"), objtransacao_NOVI)

                ' FALTA NORMALIZAR A DESCRIÇÃO DO TIPO CALLE ENTRE MARTE E PROFAT (COM OU SEM ACENTUAÇÃO E CEDILHA?)
                Log.GravarLog("NORMALIZANDO A DESCRIÇÃO DO TIPO DE LOGRADOURO NO PROFAT. CODPROFAT=" & dr("COD_PROFAT") & " DESCRIÇÃO=" & dr("DES_TIPO_CALLE"), pNomeArquivoLog)
                ' Aguardando o Demetrius para validar e fazer a normalização.
                ' TODO: normalizar a descrição do PROFAT.

                'ROGER: Se eu não destrui o DE-PARA não tem porque reconstruí-lo.
                'Dados.InsereTipoLogradouroDEPARA(dr("COD_PROFAT"), dr("COD_NOVI"), dr.Item("COD_PROFAT"), objtransacao_NOVI)
                'Log.GravarLog("INSERIU OS DADOS DE TIPO DE LOGRADOURO NA TABELA DE DEPARA --", pNomeArquivoLog)
                'Log.GravarLog("COD MARTE --" & dr("COD_PROFAT"), pNomeArquivoLog)
                'Log.GravarLog("COD PROFAT --" & dr.Item("COD_PROFAT"), pNomeArquivoLog)
                'Log.GravarLog("COD NOVI --" & dr("COD_NOVI"), pNomeArquivoLog)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & DadosMaestro.Rows.Count)
            Next

            objtransacao_NOVI.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE TIPO DE LOGRADOURO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE TIPO DE LOGRADOURO")

        Catch ex As Exception

            objtransacao_NOVI.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True

    End Function

#End Region

#Region "REGIONAL"

    Public Shared Function NormalizaDelegacao(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_NOVI As IDbTransaction
        Dim objtransacao_MARTE As IDbTransaction

        Try
            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE DELEGAÇÃO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE DELEGAÇÃO")

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)

            objtransacao_NOVI = conn_NOVI.BeginTransaction
            objtransacao_MARTE = conn_MARTE.BeginTransaction

            Dim cont As Integer = 0
            Dados.DeletaDelegacaoDePara(objtransacao_MARTE)
            Log.GravarLog("APAGOU DADOS TABELA DE DEPARA", pNomeArquivoLog)
            Dados.DeletaDelegacaoNOVI(objtransacao_NOVI)
            Log.GravarLog("APAGOU DADOS TABELA DE DEPARA", pNomeArquivoLog)

            Log.GravarLog("BUSCOU DADOS DE DELEGAÇÃO DO PROFAT", pNomeArquivoLog)
            Dim dt As DataTable = Dados.RetornaDelegacaoPROFAT()
            AlteraStatusProcessamento("PROCESSANDO...")

            For Each dr As DataRow In dt.Rows

                Log.GravarLog("ATUALIZANDO DADOS DE DELEGAÇÃO NO MARTE - '" & dr.Item("DESABRREG") & "' -- NOVO CODIGO: " & dr.Item("CODREG"), pNomeArquivoLog)
                If Dados.AtualizaDelegacaoMARTE(dr.Item("DESABRREG"), dr.Item("DESREG"), dr.Item("CODREG"), objtransacao_MARTE) = 0 Then
                    Log.GravarLog("INSERINDO DADOS DE DELEGAÇÃO NO MARTE - '" & dr.Item("DESABRREG") & "' -- NOVO CODIGO: " & dr.Item("CODREG"), pNomeArquivoLog)
                    Dados.InsereDelegacaoMARTE(dr.Item("CODREG"), dr.Item("DESABRREG"), dr.Item("DESREG"), objtransacao_MARTE)
                End If

                Log.GravarLog("INSERINDO DADOS DE DELEGAÇÃO NO NOVI - '" & dr.Item("DESABRREG") & "' -- NOVO CODIGO: " & dr.Item("CODREG"), pNomeArquivoLog)
                Dados.InsereDelegacaoNOVI(dr.Item("CODREG").ToString, dr.Item("DESABRREG").ToString, dr.Item("DESREG").ToString, objtransacao_NOVI)

                Log.GravarLog("INSERINDO DADOS DE DELEGAÇÃO NA TABELA DE DEPARA - '" & dr.Item("DESABRREG") & "' -- NOVO CODIGO: " & dr.Item("CODREG"), pNomeArquivoLog)
                Dados.InsereDelegacaoDePara(dr.Item("CODREG").ToString, objtransacao_MARTE)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dt.Rows.Count)
            Next
            Dados.DeletaDelegacaoMARTE(objtransacao_MARTE)

            'objtransacao_NOVI.Rollback()
            'objtransacao_MARTE.Rollback()
            objtransacao_NOVI.Commit()
            objtransacao_MARTE.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE DELEGAÇÃO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE DELEGAÇÃO..." & vbNewLine & " É NECESSÁRIO FAZER A NORMALIZAÇÃO DE CIDADES, SE AINDA NÃO FOI FEITA.")

        Catch ex As Exception

            objtransacao_NOVI.Rollback()
            objtransacao_MARTE.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True
    End Function

#End Region

#Region "CIDADES"
    Public Shared Function NormalizaCidades(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_NOVI As IDbTransaction
        Dim objtransacao_MARTE As IDbTransaction

        Try
            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE CIDADES ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE CIDADES")

            Dim cont As Integer = 0
            Dim dt As New DataTable
            Dim dtProfat As New DataTable

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)

            objtransacao_NOVI = conn_NOVI.BeginTransaction
            objtransacao_MARTE = conn_MARTE.BeginTransaction

            'DESATIVAR AS TRIGGERS NO MARTE.
            Log.GravarLog("DESABILITANDO TRIGGER NA TABELA MARTE.COPR_TCIUDAD", pNomeArquivoLog)
            Dados.DesabilitarTriggerCidadeMARTE(objtransacao_MARTE)

            Log.GravarLog("BUSCOU DADOS DE CIDADES DO MARTE/PROFAT", pNomeArquivoLog)
            dt = Dados.RetornaCidadesMARTE()
            AlteraStatusProcessamento("PROCESSANDO...")
            For Each dr As DataRow In dt.Rows

                If Not String.IsNullOrEmpty(dr.Item("COD_PROFAT").ToString) Then

                    dtProfat = Dados.RetornaCidadesPROFAT(" CODMUN = " & dr.Item("COD_PROFAT").ToString)
                    If dtProfat.Rows.Count > 0 AndAlso Not String.IsNullOrEmpty(dtProfat.Rows(0).Item("CODIBGE").ToString) Then
                        Log.GravarLog("ATUALIZADO CODIGO IBGE " & dtProfat.Rows(0).Item("CODIBGE").ToString & " NA CIDADE " & dr.Item("COD_MARTE").ToString() & " DO MARTE.", pNomeArquivoLog)
                        Dados.AtualizaCodIBGEMARTE(dr.Item("COD_MARTE"), dtProfat.Rows(0).Item("CODIBGE"), objtransacao_MARTE)

                        Log.GravarLog("ATUALIZADO CODIGO IBGE " & dtProfat.Rows(0).Item("CODIBGE").ToString & " NA CIDADE " & dr.Item("COD_NOVI").ToString() & " DO NOVI.", pNomeArquivoLog)
                        Dados.AtualizaCodIBGENOVI(dr.Item("COD_NOVI"), dtProfat.Rows(0).Item("CODIBGE"), objtransacao_NOVI)
                    Else
                        '' ROGER: Logar as cidades que estão com CodIBGE vazio no PROFAT
                        Log.GravarLog("CIDADE COD_PROFAT " & dr.Item("COD_PROFAT").ToString() & " ESTÁ COM O CODIBGE VAZIO, PORTANTO NÃO FOI NORMALIZADO.", pNomeArquivoLog)
                    End If
                Else
                    Log.GravarLog("CIDADE MARTE " & dr.Item("COD_MARTE").ToString() & ", NOVI " & dr.Item("COD_NOVI").ToString() & " NÃO TEM COD_PROFAT NO DE-PARA.", pNomeArquivoLog)
                End If

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dt.Rows.Count)
            Next

            dt = Dados.RetornaCidadesSemDeparaMARTE()
            AlteraStatusProcessamento("INSERINDO DADOS NA TABELA DE DEPARA PARA NOVAS CIDADES...")
            For Each dr As DataRow In dt.Rows

                dtProfat = Dados.RetornaCidadesPROFAT(" NOMMUN = '" & dr.Item("DES_CIUDAD") & "'")
                If dtProfat.Rows.Count > 0 AndAlso Not String.IsNullOrEmpty(dtProfat.Rows(0).Item("CODIBGE").ToString) Then
                    Log.GravarLog("INSERE CIDADE NO NOVI - " & dr.Item("COD_CIUDAD") & "-" & dr.Item("DES_CIUDAD") & "-" & dr.Item("OID_ESTADO").ToString.Trim, pNomeArquivoLog)
                    Dados.InsereCidadeNOVI(dr.Item("COD_CIUDAD").ToString, dr.Item("DES_CIUDAD").ToString & "-" & dr.Item("OID_ESTADO").ToString.Trim,
                                           dtProfat.Rows(0).Item("CODIBGE").ToString, objtransacao_NOVI)

                    Log.GravarLog("INSERE DEPARA PARA ITENS QUE NÃO TINHAM DEPARA - " & dr.Item("COD_CIUDAD") & "-" & dr.Item("DES_CIUDAD") & "-" & dr.Item("OID_ESTADO").ToString.Trim, pNomeArquivoLog)
                    Dados.InsereCidadeDEPARA(dr.Item("COD_CIUDAD"), dtProfat.Rows(0).Item("CODMUN"), dr.Item("COD_CIUDAD"), objtransacao_MARTE)
                Else
                    Log.GravarLog("A CIDADE " & dr.Item("COD_CIUDAD") & "-" & dr.Item("DES_CIUDAD") & "-" & dr.Item("OID_ESTADO").ToString.Trim & "NÃO FOI ENCONTRADA PELA DESCRIÇÃO OU NÃO TEM CODIBGE PREENCHIDO NO PROFAT.", pNomeArquivoLog)
                End If
            Next
            Log.GravarLog("LIMPOU LIXO DAS TABELAS DO NOVI E DO DEPARA", pNomeArquivoLog)
            Dados.DeletaCidadesSemDeparaNOVI(objtransacao_NOVI)
            Dados.DeletaCidadesSemDepara(objtransacao_MARTE)

            Log.GravarLog("HABILITANDO TRIGGER NA TABELA MARTE.COPR_TCIUDAD", pNomeArquivoLog)
            Dados.HabilitarTriggerCidadeMARTE(objtransacao_MARTE)

            Log.GravarLog("EXECUTANDO O COMMIT NO BANCO DO NOVI", pNomeArquivoLog)
            objtransacao_NOVI.Commit()
            Log.GravarLog("EXECUTANDO O COMMIT NO BANCO DO MARTE", pNomeArquivoLog)
            objtransacao_MARTE.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE CIDADES ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE CIDADES.")

        Catch ex As Exception

            'LOGANDO O ERRO QUE OCORREU DENTRO DO TRY, ANTES QUE OCORRA UM ERRO AQUI EMBAIXO E SUJE O EXCEPTION
            Log.GravarLog("ERRO NA EXECUÇÃO: " & ex.ToString, pNomeArquivoLog)

            If objtransacao_MARTE.Connection IsNot Nothing Then
                Log.GravarLog("HABILITANDO TRIGGER NA TABELA MARTE.COPR_TCIUDAD (DENTRO DO TRATAMENTO DE EXCEÇÕES)", pNomeArquivoLog)
                Dados.HabilitarTriggerCidadeMARTE(objtransacao_MARTE)
                Log.GravarLog("EXECUTANDO O ROLLBACK NO BANCO DO MARTE (DENTRO DO TRATAMENTO DE EXCEÇÕES PORQUE OCORREU UM ERRO)", pNomeArquivoLog)
                objtransacao_MARTE.Rollback()
            End If
            If objtransacao_NOVI.Connection IsNot Nothing Then
                Log.GravarLog("EXECUTANDO O ROLLBACK NO BANCO DO NOVI (DENTRO DO TRATAMENTO DE EXCEÇÕES PORQUE OCORREU UM ERRO)", pNomeArquivoLog)
                objtransacao_NOVI.Rollback()
            End If

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True
    End Function
#End Region

#Region "FILIAL"

    Public Shared Function NormalizaFilial(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_NOVI As IDbTransaction
        Dim objtransacao_MARTE As IDbTransaction

        Try
            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE FILIAL ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE FILIAL")

            Dim cont As Integer = 0
            Dim dtCodProfat As New DataTable
            Dim dtaux As New DataTable
            Dim dt As DataTable = Dados.RetornaFiliaisMARTE()
            Log.GravarLog("RECUPEROU DADOS DE FILIAL DO MARTE", pNomeArquivoLog)

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)

            objtransacao_NOVI = conn_NOVI.BeginTransaction
            objtransacao_MARTE = conn_MARTE.BeginTransaction

            AlteraStatusProcessamento("PROCESSANDO...")
            For Each dr As DataRow In dt.Rows
                Dados.AtualizaFilialNOVI(dr.Item("COMERCIAL"), dr.Item("NOVI"), objtransacao_NOVI)
                Log.GravarLog("ATUALIZOU A TABELA ODELE NO NOVI COM O CAMPO COD_FILIAL_COMERCIAL = " & dr.Item("COMERCIAL"), pNomeArquivoLog)

                dtCodProfat = Dados.RetornaCODFilialPROFAT(dr.Item("COMERCIAL"))
                Log.GravarLog("BUSCOU DADOS DO PROFAT NA TABELA DE DEPARA COM O CODIGO = " & dr.Item("COMERCIAL"), pNomeArquivoLog)
                For Each drProfat As DataRow In dtCodProfat.Rows
                    If Not String.IsNullOrEmpty(drProfat.Item("COD_PROFAT").ToString) Then
                        dtaux = Dados.RetornaFilialPROFAT(drProfat.Item("COD_PROFAT").ToString)

                        Log.GravarLog("INSERIU DADOS NA TABELA ESPELHO DE FATURAMENTO COPR_TCAD_FILIAL_FAT CODFIL = " & dtaux.Rows(0).Item("CODFIL") & " NOMFIL = " & dtaux.Rows(0).Item("NOMFIL") & " CNP = " & dtaux.Rows(0).Item("CNP"), pNomeArquivoLog)
                        Dados.InsereTabelaEspelhoFaturamento(dtaux.Rows(0).Item("CODFIL").ToString, dtaux.Rows(0).Item("NOMFIL").ToString, dtaux.Rows(0).Item("CNP").ToString, objtransacao_MARTE)

                        Log.GravarLog("INSERIU DADOS NA TABELA DEPARA DE FATURAMENTO COPR_TFILIAL_COM_X_FAT COMERCIAL = " & dr.Item("COMERCIAL") & " CODEMP = " & dtaux.Rows(0).Item("CODEMP") & " CODFIL = " & dtaux.Rows(0).Item("CODFIL"), pNomeArquivoLog)
                        Dados.InsereDeParaFaturamento(dr.Item("COMERCIAL"), drProfat.Item("COD_PARAM_TAB").ToString, dtaux.Rows(0).Item("CODFIL"), objtransacao_MARTE)
                    Else
                        Log.GravarLog("NÃO FOI ENCONTRADO VALOR DO CAMPO COD_PROFAT DO DE-PARA PARA A FILIAL COMERCIAL " & dr.Item("COMERCIAL") & " PARA O COD_PARAM_TAB=" & dr.Item("COD_PARAM_TAB"), pNomeArquivoLog)
                    End If
                Next

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dt.Rows.Count)
            Next

            '**** 
            'ESTE BLOCO FOI DESLIGADO PORQUE NÃO VAMOS FAZER NORMALIZAÇÃO DE REGIONAL
            '****
            'AlteraStatusProcessamento("DELETANDO DADOS DA TABELA ODERE")
            'Log.GravarLog("DELETANDO DADOS DA TABELA ODERE", pNomeArquivoLog)
            'Dados.DeleteOdere(objtransacao_NOVI)

            'AlteraStatusProcessamento("INSERINDO DADOS NA TABELA ODERE A PARTIR DO DEPARA")
            'Log.GravarLog("INSERINDO DADOS NA TABELA ODERE A PARTIR DO DEPARA", pNomeArquivoLog)
            'Dados.InsereOdere(objtransacao_NOVI)
            '********

            objtransacao_NOVI.Commit()
            objtransacao_MARTE.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE FILIAL ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE FILIAL...")

        Catch ex As Exception

            objtransacao_NOVI.Rollback()
            objtransacao_MARTE.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True

    End Function

#End Region

#Region "TIPO POSTO"

    Public Shared Function NormalizaTipoPosto(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_NOVI As IDbTransaction
        Dim objtransacao_PROFAT As IDbTransaction

        Try

            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE TIPO DE POSTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE TIPO DE POSTO")

            Dim cont As Integer = 0
            Dim dtTipoPosto As DataTable = Dados.RetornaTipoPostoMARTE()
            Log.GravarLog("RECUPEROU DADOS DO MARTE PARA FAZER A NORMALIZAÇÃO", pNomeArquivoLog)

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            Dim conn_PROFAT As New SqlConnection(AcessoDados.RecuperarStringConexao(Dados.CONEXAO_PROFAT))

            conn_PROFAT.Open()

            objtransacao_PROFAT = conn_PROFAT.BeginTransaction
            objtransacao_NOVI = conn_NOVI.BeginTransaction

            AlteraStatusProcessamento("PROCESSANDO...")
            For Each dr As DataRow In dtTipoPosto.Rows
                Dados.AtualizaCodComercialTipoPostoNOVI(dr.Item("COD_COMERCIAL"), dr.Item("COD_NOVI"), objtransacao_NOVI)
                Log.GravarLog("ATUALIZOU A TABELA XELEM COD_TABLA = 73 E COD_ELEMENTO = '" & dr.Item("COD_NOVI") & "', COM INFORMAÇÕES DO TIPO DE POSTO COMERCIAL NO CAMPO COD_COMERCIAL = " & dr.Item("COD_COMERCIAL"), pNomeArquivoLog)

                Dados.MergeTipoPostoPROFAT(dr.Item("COD_NOVI"), dr.Item("DES_TIPO_PUESTO"), dr.Item("COD_COMERCIAL"), objtransacao_PROFAT)
                Log.GravarLog("VERIFICANDO SE O TIPO DE POSTO JÁ EXISTE NO PROFAT COD = '" & dr.Item("COD_COMERCIAL") & "', SE SIM ATUALIZOU O CAMPO CODFUNPATCOM COM TAL CODIGO, SENÃO CRIOU O ITEM " & dr.Item("COD_NOVI"), pNomeArquivoLog)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dtTipoPosto.Rows.Count)
            Next

            objtransacao_PROFAT.Commit()
            objtransacao_NOVI.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE TIPO DE POSTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE TIPO DE POSTO...")

        Catch ex As Exception

            objtransacao_PROFAT.Rollback()
            objtransacao_NOVI.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True
    End Function

#End Region

#Region "SEGMENTO/SUB SEGMENTO"

    Public Shared Function NormalizaSegmentoSubSeg(ByVal pNomeArquivoLog As String) As Boolean

        'FUNÇÃO DESCONTINUADA, USAMOS A OUTRA QUE TEM O MESMO NOME MAS COM FINAL 2.
        Dim objtransacao_MARTE As IDbTransaction

        Try

            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE SEGMENTO E SUB SEGMENTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE SEGMENTO E SUB SEGMENTO")

            Dim cont As Integer = 0
            Dim dtSeg As DataTable = Dados.RetornaSegmentosPROFAT()
            Dim dtSegmentoMarte As DataTable
            Dim dtSubSegmentoMarte As DataTable
            Dim sCodSeg As String = String.Empty
            Dim sCodSubSeg As String = String.Empty
            Dim sOidSeg As String = String.Empty
            Dim sOidSubSeg As String = String.Empty
            Dim sOidSegNOVO As String = String.Empty
            Dim sOidSubSegNOVO As String = String.Empty
            Dim sCodAnterior As String = String.Empty
            Log.GravarLog("RECUPEROU DADOS DO PROFAT PARA FAZER A NORMALIZAÇÃO", pNomeArquivoLog)

            Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)
            objtransacao_MARTE = conn_MARTE.BeginTransaction()

            Dados.DeletaDeParaSegmentoSubSeg("1", objtransacao_MARTE)
            Dados.DeletaDeParaSegmentoSubSeg("2", objtransacao_MARTE)

            AlteraStatusProcessamento("PROCESSANDO SEGMENTO E SUB SEGMENTO...")
            For Each dr As DataRow In dtSeg.Rows

                'Verifica se já fez a normalização do codigo
                If sCodAnterior <> dr.Item("CODRAMATV").ToString Then
                    dtSegmentoMarte = New DataTable
                    dtSegmentoMarte = Dados.RetornaSegmentosMARTE(dr.Item("CODRAMATV").ToString)

                    If Not dtSegmentoMarte Is Nothing AndAlso dtSegmentoMarte.Rows.Count > 0 Then
                        sCodSeg = dtSegmentoMarte.Rows(0).Item("COD_MARTE").ToString
                        sOidSeg = dtSegmentoMarte.Rows(0).Item("OID_SEGMENTO").ToString
                    Else
                        sCodSeg = dr.Item("CODRAMATV").ToString
                        sOidSeg = String.Empty
                    End If
                    Log.GravarLog("NORMALIZANDO O SEGMENTO '" & dr.Item("DESRAMATV").ToString &
                                  "' NO MARTE. O CODIGO MARTE ERA '" & sCodSeg & "' E PASSOU A SER '" & dr.Item("CODRAMATV").ToString & "'", pNomeArquivoLog)
                    sOidSegNOVO = Dados.MergeSegmentoMARTE(sCodSeg, dr.Item("DESRAMATV").ToString, dr.Item("CODRAMATV").ToString, sOidSeg, objtransacao_MARTE)

                    Dados.InsereDeParaSegmentoSubSeg(sCodSeg, dr.Item("CODRAMATV").ToString, "1", objtransacao_MARTE)
                End If

                'Armazena o codigo normalizado
                sCodAnterior = dr.Item("CODRAMATV").ToString
                dtSubSegmentoMarte = New DataTable

                If Not String.IsNullOrEmpty(sOidSegNOVO) Then sOidSeg = sOidSegNOVO
                dtSubSegmentoMarte = Dados.RetornaSubSegmentosMARTE(dr.Item("CODSUBRAMATV").ToString, sOidSeg)

                If Not dtSubSegmentoMarte Is Nothing AndAlso dtSubSegmentoMarte.Rows.Count > 0 Then
                    sOidSubSeg = dtSubSegmentoMarte.Rows(0).Item("OID_SUBSEGMENTO").ToString
                    sCodSubSeg = dtSubSegmentoMarte.Rows(0).Item("COD_SUBSEGMENTO").ToString
                    sOidSeg = dtSubSegmentoMarte.Rows(0).Item("OID_SEGMENTO").ToString
                Else
                    sOidSubSeg = String.Empty
                    sCodSubSeg = dr.Item("CODSUBRAMATV").ToString
                End If

                Log.GravarLog("NORMALIZANDO O SUBSEGMENTO '" & dr.Item("DESSUBRAMATV").ToString &
                              "' NO MARTE. O CODIGO MARTE ERA '" & sCodSeg &
                              "' E PASSOU A SER '" & dr.Item("CODSUBRAMATV") & "'", pNomeArquivoLog)

                sOidSubSegNOVO = Dados.MergeSubSegmentoMARTE(sCodSubSeg, dr.Item("DESSUBRAMATV").ToString, _
                                                                 dr.Item("CODSUBRAMATV").ToString, sOidSeg, sOidSubSeg, objtransacao_MARTE)

                If String.IsNullOrEmpty(sOidSubSeg) Then sOidSubSeg = sOidSubSegNOVO

                Log.GravarLog("INSERINDO O DE-PARA DE SUBSEGMENTO, CODMARTE=" & sOidSubSeg & " CODPROFAT=" & dr.Item("CODSUBRAMATV").ToString, pNomeArquivoLog)
                Dados.InsereDeParaSegmentoSubSeg(sOidSubSeg, dr.Item("CODSUBRAMATV").ToString, "2", objtransacao_MARTE)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO SEGMENTO... " & vbNewLine & " Item " & cont & " de " & dtSeg.Rows.Count)
            Next

            objtransacao_MARTE.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE SUBSEGMENTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE SUBSEGMENTO...")

            'TODO: Colocar um msgbox para avisar que tem que rodar a normalização de cliente e subcliete para acertar os segmentos e subsegmentos.


        Catch ex As Exception
            objtransacao_MARTE.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True
    End Function

    Public Shared Function NormalizaSegmentoSubSeg_2(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_MARTE As IDbTransaction

        Try

            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE SEGMENTO E SUB SEGMENTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE SEGMENTO E SUB SEGMENTO")

            Dim cont As Integer = 0
            Dim dtSegmentoMarte As DataTable
            Dim dtSubSegmentoMarte As DataTable
            Dim sCodSeg As String = String.Empty
            Dim sCodSubSeg As String = String.Empty
            Dim sOidSeg As String = String.Empty
            Dim sOidSubSeg As String = String.Empty
            Dim sOidSegNOVO As String = String.Empty
            Dim sOidSubSegNOVO As String = String.Empty
            Dim sCodAnterior As String = String.Empty

            AlteraStatusProcessamento("PROCESSANDO SEGMENTO E SUB SEGMENTO...")

            Dim dtSeg As DataTable = Dados.RetornaSegmentosPROFAT()
            Log.GravarLog("RECUPEROU DADOS DO PROFAT PARA FAZER A NORMALIZAÇÃO", pNomeArquivoLog)

            Log.GravarLog("ABRINDO CONEXÃO COM O MARTE", pNomeArquivoLog)
            Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)
            objtransacao_MARTE = conn_MARTE.BeginTransaction()

            Log.GravarLog("APAGANDO DE-PARA: SEGMENTO X RAMO", pNomeArquivoLog)
            Dados.DeletaDeParaSegmentoSubSeg("1", objtransacao_MARTE)
            Log.GravarLog("APAGANDO DE-PARA: SUBSEGMENTO X SUBRAMO", pNomeArquivoLog)
            Dados.DeletaDeParaSegmentoSubSeg("2", objtransacao_MARTE)

            'INCLUIR SEGMENTO "NÃO INFORMADO"
            Log.GravarLog("INCLUIR SEGMENTO 'NÃO INFORMADO/CLASSIFICADO' NO MARTE", pNomeArquivoLog)
            Dados.InsereSegmentoPadrao(objtransacao_MARTE)

            'INCLUIR SUBSEGMENTO "NÃO INFORMADO"
            Log.GravarLog("INCLUIR SUBSEGMENTO 'NÃO INFORMADO/CLASSIFICADO' NO MARTE", pNomeArquivoLog)
            Dados.InsereSubSegmentoPadrao(objtransacao_MARTE)

            'ALTERAR TODOS OS CLIENTES PARA APONTAR PARA O SEGMENTO E SUBSEGMENTO PADRÃO
            Log.GravarLog("ALTERANDO TODOS OS CLIENTES PARA SEGMENTO E SUBSEGMENTO 'NÃO INFORMADO/CLASSIFICADO' NO MARTE", pNomeArquivoLog)
            Dados.AtualizarSegmentoSubsegmentoCliente(objtransacao_MARTE)



            'For Each dr As DataRow In dtSeg.Rows

            '    'Verifica se já fez a normalização do codigo
            '    If sCodAnterior <> dr.Item("CODRAMATV").ToString Then
            '        dtSegmentoMarte = New DataTable
            '        dtSegmentoMarte = Dados.RetornaSegmentosMARTE(dr.Item("CODRAMATV").ToString)

            '        If Not dtSegmentoMarte Is Nothing AndAlso dtSegmentoMarte.Rows.Count > 0 Then
            '            sCodSeg = dtSegmentoMarte.Rows(0).Item("COD_MARTE").ToString
            '            sOidSeg = dtSegmentoMarte.Rows(0).Item("OID_SEGMENTO").ToString
            '        Else
            '            sCodSeg = dr.Item("CODRAMATV").ToString
            '            sOidSeg = String.Empty
            '        End If
            '        Log.GravarLog("NORMALIZANDO O SEGMENTO '" & dr.Item("DESRAMATV").ToString &
            '                      "' NO MARTE. O CODIGO MARTE ERA '" & sCodSeg & "' E PASSOU A SER '" & dr.Item("CODRAMATV").ToString & "'", pNomeArquivoLog)
            '        sOidSegNOVO = Dados.MergeSegmentoMARTE(sCodSeg, dr.Item("DESRAMATV").ToString, dr.Item("CODRAMATV").ToString, sOidSeg, objtransacao_MARTE)

            '        Dados.InsereDeParaSegmentoSubSeg(sCodSeg, dr.Item("CODRAMATV").ToString, "1", objtransacao_MARTE)
            '    End If

            '    'Armazena o codigo normalizado
            '    sCodAnterior = dr.Item("CODRAMATV").ToString
            '    dtSubSegmentoMarte = New DataTable

            '    If Not String.IsNullOrEmpty(sOidSegNOVO) Then sOidSeg = sOidSegNOVO
            '    dtSubSegmentoMarte = Dados.RetornaSubSegmentosMARTE(dr.Item("CODSUBRAMATV").ToString, sOidSeg)

            '    If Not dtSubSegmentoMarte Is Nothing AndAlso dtSubSegmentoMarte.Rows.Count > 0 Then
            '        sOidSubSeg = dtSubSegmentoMarte.Rows(0).Item("OID_SUBSEGMENTO").ToString
            '        sCodSubSeg = dtSubSegmentoMarte.Rows(0).Item("COD_SUBSEGMENTO").ToString
            '        sOidSeg = dtSubSegmentoMarte.Rows(0).Item("OID_SEGMENTO").ToString
            '    Else
            '        sOidSubSeg = String.Empty
            '        sCodSubSeg = dr.Item("CODSUBRAMATV").ToString
            '    End If

            '    Log.GravarLog("NORMALIZANDO O SUBSEGMENTO '" & dr.Item("DESSUBRAMATV").ToString &
            '                  "' NO MARTE. O CODIGO MARTE ERA '" & sCodSeg &
            '                  "' E PASSOU A SER '" & dr.Item("CODSUBRAMATV") & "'", pNomeArquivoLog)

            '    sOidSubSegNOVO = Dados.MergeSubSegmentoMARTE(sCodSubSeg, dr.Item("DESSUBRAMATV").ToString,
            '                                                     dr.Item("CODSUBRAMATV").ToString, sOidSeg, sOidSubSeg, objtransacao_MARTE)

            '    If String.IsNullOrEmpty(sOidSubSeg) Then sOidSubSeg = sOidSubSegNOVO

            '    Log.GravarLog("INSERINDO O DE-PARA DE SUBSEGMENTO, CODMARTE=" & sOidSubSeg & " CODPROFAT=" & dr.Item("CODSUBRAMATV").ToString, pNomeArquivoLog)
            '    Dados.InsereDeParaSegmentoSubSeg(sOidSubSeg, dr.Item("CODSUBRAMATV").ToString, "2", objtransacao_MARTE)

            '    cont = cont + 1
            '    AlteraStatusProcessamento("PROCESSANDO SEGMENTO... " & vbNewLine & " Item " & cont & " de " & dtSeg.Rows.Count)
            'Next

            objtransacao_MARTE.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE SUBSEGMENTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE SUBSEGMENTO...")

            'TODO: Colocar um msgbox para avisar que tem que rodar a normalização de cliente e subcliete para acertar os segmentos e subsegmentos.


        Catch ex As Exception
            objtransacao_MARTE.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True
    End Function

#End Region

#Region "DIVISÃO"
    Public Shared Function NormalizaDivisao(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_PROFAT As IDbTransaction
        Dim objtransacao_NOVI As IDbTransaction

        Try
            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE DIVISÃO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE DIVISÃO")

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            Dim conn_PROFAT As New SqlConnection(AcessoDados.RecuperarStringConexao(Dados.CONEXAO_PROFAT))

            Dim cont As Integer = 0
            Dim dt As New DataTable
            Dim dtProfat As New DataTable

            Log.GravarLog("BUSCOU DADOS DE DIVISÃO DO MARTE", pNomeArquivoLog)
            dt = Dados.RetornaDivisaoMARTE()

            conn_PROFAT.Open()

            objtransacao_PROFAT = conn_PROFAT.BeginTransaction
            objtransacao_NOVI = conn_NOVI.BeginTransaction

            AlteraStatusProcessamento("PROCESSANDO...")
            For Each dr As DataRow In dt.Rows

                Log.GravarLog("BUSCOU DADOS DE DIVISÃO DO PROFAT", pNomeArquivoLog)
                dtProfat = Dados.RetornaDivisaoPROFAT(dr.Item("COD_PROFAT"))

                For Each drPro As DataRow In dtProfat.Rows
                    Log.GravarLog("ATUALIZOU O ITEM DE COBRANÇA '" & drPro.Item("CODITECOB") & "' PARA O CODIGO COMERCIAL '" & dr.Item("COD_MARTE") & "'", pNomeArquivoLog)
                    Dados.AtualizaCodComercialDivisaoPROFAT(dr.Item("COD_MARTE"), drPro.Item("CODITECOB"), objtransacao_PROFAT)
                Next

                Log.GravarLog("ATUALIZOU A DIVISÃO DO NOVI NA TABELA ADA_OTARI '" & dr.Item("COD_NOVI") & "' PARA O CODIGO COMERCIAL '" & dr.Item("COD_MARTE") & "'", pNomeArquivoLog)
                Dados.AtualizaCodComercialDivisaoNOVI(dr.Item("COD_MARTE"), dr.Item("COD_NOVI"), objtransacao_NOVI)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO DIVISÃO... " & vbNewLine & " Item " & cont & " de " & dt.Rows.Count)
            Next

            objtransacao_PROFAT.Commit()
            objtransacao_NOVI.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE DIVISÃO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE DIVISÃO...")

        Catch ex As Exception

            objtransacao_PROFAT.Rollback()
            objtransacao_NOVI.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True
    End Function

#End Region

    Public Shared Sub AlteraStatusProcessamento(ByVal ptexto As String)
        frmMigracao.txtStatus.Text = ptexto
        frmMigracao.txtStatus.Refresh()
    End Sub

End Class
