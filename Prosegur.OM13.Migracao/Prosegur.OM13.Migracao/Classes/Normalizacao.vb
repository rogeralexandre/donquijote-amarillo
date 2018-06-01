Imports Prosegur.DbHelper
Imports System.Data.SqlClient
Imports System.Text
Imports System.Globalization

Public Class Normalizacao

#Region "CLIENTE"

    Public Shared Function NormalizaCliente(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_NOVI As IDbTransaction
        Dim objtransacao_PROFAT As IDbTransaction
        Dim objtransacao_MARTE As IDbTransaction
        'Por desconhecimento de como obter o stado da transação, apelei.
        Dim TransacaoMARTE_ON As Boolean = False
        Dim TransacaoPROFAT_ON As Boolean = False
        Dim TransacaoNOVI_ON As Boolean = False

        Dim UsaWS As Boolean = CType(System.Configuration.ConfigurationManager.AppSettings("HabilitarWSPROFAT"), Boolean)

        Try

            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE CLIENTE ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE CLIENTE")

            UsaWS = False

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            Dim conn_PROFAT As New SqlConnection(AcessoDados.RecuperarStringConexao(Dados.CONEXAO_PROFAT))
            conn_PROFAT.Open()

            objtransacao_NOVI = conn_NOVI.BeginTransaction
            TransacaoNOVI_ON = True
            objtransacao_PROFAT = conn_PROFAT.BeginTransaction
            TransacaoPROFAT_ON = True

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
                                Log.GravarLog("Cod Retorno WSPROFAT = " & msgRet.Codigo & vbNewLine &
                                              "Mensagem retorno = " & msgRet.Mensagem & vbNewLine &
                                              "Detalhe = " & msgRet.Detalhe, pNomeArquivoLog)
                            Next
                            Log.GravarLog("-----------------------------------------------------------", pNomeArquivoLog)
                        Else
                            Log.GravarLog("CLIENTE ATUALIZADO COM SUCESSO", pNomeArquivoLog)
                        End If

                    End If

                    Log.GravarLog("ATUALIZOU NO PROFAT NA TABELA 'FAT_TCADCLI' O CLIENTE " & dr.Item("COD_PROFAT").ToString & " COM O CODCLICOM = " & dr.Item("COD_CLIENTE").ToString, pNomeArquivoLog)
                    Dados.AtualizaCodComercialClientePROFAT(dr.Item("COD_CLIENTE").ToString, dr.Item("COD_PROFAT").ToString, objtransacao_PROFAT)
                End If

                Log.GravarLog("ATUALIZOU NA TABELA 'DDLVIG.OCLTE' O CLIENTE " & dr.Item("COD_NOVI").ToString & " COD_COMERCIAL = " & dr.Item("COD_CLIENTE").ToString, pNomeArquivoLog)
                Dados.AtualizaCodComercialClienteNOVI(dr.Item("COD_NOVI").ToString, dr.Item("COD_IDENTIFICACION_FISCAL").ToString,
                                                      dr.Item("DES_NOMBRE_FANTASIA").ToString, dr.Item("DES_RAZON_SOCIAL").ToString,
                                                      dr.Item("COD_CLIENTE").ToString, dr.Item("DES_TIPO_CALLE").ToString, objtransacao_NOVI)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dtClientes.Rows.Count)
            Next

            Log.GravarLog("Realizando Commit no PROFAT", pNomeArquivoLog)
            objtransacao_PROFAT.Commit()
            TransacaoPROFAT_ON = False
            Log.GravarLog("Realizando Commit no NOVI", pNomeArquivoLog)
            objtransacao_NOVI.Commit()
            TransacaoNOVI_ON = False

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
            Else
                Log.GravarLog("BOA NOTÍCIA = NÃO FORAM ENCONTRADOS CLIENTES COM NIF DUPLICADOS NO MARTE." & vbNewLine, pNomeArquivoLog)
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
            Else
                Log.GravarLog("NÃO FORAM ENCONTRADOS CLIENTES COM NIF NULO NO MARTE:" & vbNewLine, pNomeArquivoLog)
            End If

            If (False) Then
                'atualizar o código do segmento e subsegmento no marte
                Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)
                Log.GravarLog("CARREGANDO OS RAMOS E SUBRAMOS DO PROFAT NO SEGMENTO/SUBSEGMENTO NO MARTE.", pNomeArquivoLog)

                Dim dtClientesPROFAT As New DataTable
                dtClientesPROFAT = Dados.RetornaRamoSubramoPROFAT()

                objtransacao_MARTE = conn_MARTE.BeginTransaction()
                TransacaoMARTE_ON = True
                For Each dr As DataRow In dtClientesPROFAT.Rows
                    Log.GravarLog("ATUALIZANDO O CLIENTE " & dr.Item("CODCLICOM"), pNomeArquivoLog)
                    Dados.AtualizaSegmentoSubsegmentoClientecomBasePROFAT(dr("CODCLICOM").ToString,
                                                                      dr("CODRAMATV").ToString,
                                                                      dr("CODSUBRAMATV").ToString,
                                                                      pNomeArquivoLog,
                                                                      objtransacao_MARTE)
                Next

                Log.GravarLog("REALIZANDO COMMIT NO MARTE.", pNomeArquivoLog)
                objtransacao_MARTE.Commit()
                TransacaoMARTE_ON = False
                Log.GravarLog("FIM DAS CARGAS DE RAMO/SUBRAMO NO MARTE.", pNomeArquivoLog)
            End If



            Log.GravarLog("FIM DA NORMALIZAÇÃO DE CLIENTE ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento(msgInfo & vbNewLine & "FIM DA NORMALIZAÇÃO DE CLIENTE")

        Catch ex As Exception
            Log.GravarLog(ex.ToString, pNomeArquivoLog)

            Log.GravarLog("EXECUTANDO O ROLLBACK NAS BASES", pNomeArquivoLog)
            If TransacaoMARTE_ON Then
                objtransacao_MARTE.Rollback()
            End If
            If TransacaoPROFAT_ON Then
                objtransacao_PROFAT.Rollback()
            End If
            If TransacaoNOVI_ON Then
                objtransacao_NOVI.Rollback()
            End If

            'Log.GravarLog(ex.ToString, pNomeArquivoLog)
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

            'Travar o uso do WS para sempre não
            UsaWS = False

            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE SUB CLIENTE ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE SUB CLIENTE")

            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            'Tive que alterar o timeout do PROFAT, está demorando mais de 30 segundos.
            Dim conn_PROFAT_String As String = AcessoDados.RecuperarStringConexao(Dados.CONEXAO_PROFAT)
            Dim builder As New SqlConnectionStringBuilder(conn_PROFAT_String)
            builder.ConnectTimeout = 90
            Dim conn_PROFAT As New SqlConnection(builder.ConnectionString)
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
                            Log.GravarLog("ERRO AO TENTAR ATUALIZAR O SUB CLIENTE:  " & dr.Item("COD_SUB_MARTE").ToString, pNomeArquivoLog)
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

                    Log.GravarLog("ATUALIZAR NA TABELA 'FAT_TCADPONATE' CODPONATE = " & dr.Item("COD_PROFAT").ToString & " CODPONATECOM = " & dr.Item("COD_SUB_MARTE").ToString, pNomeArquivoLog)
                    Dados.AtualizaCodComercialSubCliPROFAT(dr.Item("COD_SUB_MARTE").ToString, dr.Item("COD_PROFAT").ToString, objtransacao_PROFAT)
                End If

                Log.GravarLog("ATUALIZAR NA TABELA 'DDLVIG.OSBCL' CLIENTE E SUB NOVI: " & dr.Item("COD_CLI_NOVI").ToString & "/" & dr.Item("COD_SUB_NOVI").ToString & " COD_SUBCLI_COMERCIAL = " & dr.Item("COD_SUB_MARTE").ToString, pNomeArquivoLog)
                Dados.AtualizaCodComercialSubCliNOVI(dr.Item("DES_SUBCLIENTE").ToString, dr.Item("DES_CORTA_SUBCLIENTE").ToString, dr.Item("DES_TIPO_CALLE").ToString, _
                                                     dr.Item("COD_CLI_NOVI").ToString, dr.Item("COD_SUB_NOVI").ToString, dr.Item("COD_SUB_MARTE").ToString, objtransacao_NOVI)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dtSubClientes.Rows.Count)
            Next
            Log.GravarLog("Rodando os commit no PROFAT e NOVI", pNomeArquivoLog)
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

            MessageBox.Show("Normalização executada!")

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
        Dim objTransacao_PROFAT As IDbTransaction

        Dim intResposta As Integer

        intResposta = MsgBox("Já foi feito o backup da tabela FAT_TTABTIPLOG?", MsgBoxStyle.YesNo, "ATENÇÃO")

        If (intResposta = MsgBoxResult.No) Then
            MsgBox("Faça o backup da FAT_TTABTIPLOG antes de continuar, normalização cancelada!", MsgBoxStyle.Exclamation, "ATENÇÃO")
        Else
            Try

                Log.GravarLog("INICIO DA NORMALIZAÇÃO DE TIPO DE LOGRADOURO ----------------------------------------", pNomeArquivoLog)

                AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE TIPO DE LOGRADOURO")
                AlteraStatusProcessamento("PROCESSANDO...")

                Dim DadosMaestro As New DataTable
                Dim cont As Integer = 0

                Log.GravarLog("ABRINDO CONEXÃO COM NOVI E INICIANDO TRANSAÇÃO", pNomeArquivoLog)
                Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
                objtransacao_NOVI = conn_NOVI.BeginTransaction

                Log.GravarLog("ABRINDO CONEXÃO COM PROFAT E INICIANDO TRANSAÇÃO", pNomeArquivoLog)
                Dim conn_PROFAT As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_PROFAT)
                objTransacao_PROFAT = conn_PROFAT.BeginTransaction

                Log.GravarLog("BUSCAR DADOS DO MARTE PARA A MEMORIA", pNomeArquivoLog)
                DadosMaestro = Dados.TipoLogradouroMARTE()

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

                    Log.GravarLog("NOVI, ATUALIZOU CAMPO COD_COMERCIAL recebeu o DES_TIPO_CALLE do MARTE. COD_NOVI:" & dr("COD_NOVI") & " COD_COMERCIAL=" & dr("DES_TIPO_CALLE"), pNomeArquivoLog)
                    Dados.AtualizaTipoLogradouroNOVI(dr("COD_NOVI"), dr("DES_TIPO_CALLE"), objtransacao_NOVI)

                    ' FALTA NORMALIZAR A DESCRIÇÃO DO TIPO CALLE ENTRE MARTE E PROFAT 
                    Log.GravarLog("PROFAT, ATUALIZOU O CAMPO DESTIPLOG COM A DESCRIÇÃO DO MARTE. CODPROFAT=" & dr("COD_PROFAT") & " DESCRIÇÃO=" & dr("DES_TIPO_CALLE"), pNomeArquivoLog)
                    Dados.AtualizaTipoLogradouroPROFAT(dr("COD_PROFAT"), dr("DES_TIPO_CALLE"), objTransacao_PROFAT)

                    'ROGER: Se eu não destrui o DE-PARA não tem porque reconstruí-lo.
                    'Dados.InsereTipoLogradouroDEPARA(dr("COD_PROFAT"), dr("COD_NOVI"), dr.Item("COD_PROFAT"), objtransacao_NOVI)
                    'Log.GravarLog("INSERIU OS DADOS DE TIPO DE LOGRADOURO NA TABELA DE DEPARA --", pNomeArquivoLog)
                    'Log.GravarLog("COD MARTE --" & dr("COD_PROFAT"), pNomeArquivoLog)
                    'Log.GravarLog("COD PROFAT --" & dr.Item("COD_PROFAT"), pNomeArquivoLog)
                    'Log.GravarLog("COD NOVI --" & dr("COD_NOVI"), pNomeArquivoLog)

                    cont = cont + 1
                    AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & DadosMaestro.Rows.Count)
                Next

                Log.GravarLog("COMMIT NO NOVI", pNomeArquivoLog)
                objtransacao_NOVI.Commit()
                Log.GravarLog("COMMIT NO PROFAT", pNomeArquivoLog)
                objTransacao_PROFAT.Commit()

                Log.GravarLog("FIM DA NORMALIZAÇÃO DE TIPO DE LOGRADOURO ----------------------------------------", pNomeArquivoLog)
                AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE TIPO DE LOGRADOURO")

            Catch ex As Exception
                Log.GravarLog(ex.ToString, pNomeArquivoLog)

                Log.GravarLog("REALIZANDO ROLLBACKS", pNomeArquivoLog)
                objtransacao_NOVI.Rollback()
                objTransacao_PROFAT.Rollback()

                Throw ex
            End Try
        End If

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

            'dt = Dados.RetornaCidadesSemDeparaMARTE()
            'AlteraStatusProcessamento("INSERINDO DADOS NA TABELA DE DEPARA PARA NOVAS CIDADES...")
            'For Each dr As DataRow In dt.Rows

            '    dtProfat = Dados.RetornaCidadesPROFAT(" NOMMUN = '" & dr.Item("DES_CIUDAD") & "'")
            '    If dtProfat.Rows.Count > 0 AndAlso Not String.IsNullOrEmpty(dtProfat.Rows(0).Item("CODIBGE").ToString) Then
            '        Log.GravarLog("INSERE CIDADE NO NOVI - " & dr.Item("COD_CIUDAD") & "-" & dr.Item("DES_CIUDAD") & "-" & dr.Item("OID_ESTADO").ToString.Trim, pNomeArquivoLog)
            '        Dados.InsereCidadeNOVI(dr.Item("COD_CIUDAD").ToString, dr.Item("DES_CIUDAD").ToString & "-" & dr.Item("OID_ESTADO").ToString.Trim,
            '                               dtProfat.Rows(0).Item("CODIBGE").ToString, objtransacao_NOVI)

            '        Log.GravarLog("INSERE DEPARA PARA ITENS QUE NÃO TINHAM DEPARA - " & dr.Item("COD_CIUDAD") & "-" & dr.Item("DES_CIUDAD") & "-" & dr.Item("OID_ESTADO").ToString.Trim, pNomeArquivoLog)
            '        Dados.InsereCidadeDEPARA(dr.Item("COD_CIUDAD"), dtProfat.Rows(0).Item("CODMUN"), dr.Item("COD_CIUDAD"), objtransacao_MARTE)
            '    Else
            '        Log.GravarLog("A CIDADE " & dr.Item("COD_CIUDAD") & "-" & dr.Item("DES_CIUDAD") & "-" & dr.Item("OID_ESTADO").ToString.Trim & "NÃO FOI ENCONTRADA PELA DESCRIÇÃO OU NÃO TEM CODIBGE PREENCHIDO NO PROFAT.", pNomeArquivoLog)
            '    End If
            'Next

            'Log.GravarLog("LIMPOU LIXO DAS TABELAS DO NOVI E DO DEPARA", pNomeArquivoLog)
            'Dados.DeletaCidadesSemDeparaNOVI(objtransacao_NOVI)
            'Dados.DeletaCidadesSemDepara(objtransacao_MARTE)

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
                        Log.GravarLog("BUSCAR FILIAL NO PROFAT COM O CODIGO = " & drProfat.Item("COD_PROFAT").ToString(), pNomeArquivoLog)
                        dtaux = Dados.RetornaFilialPROFAT(drProfat.Item("COD_PROFAT").ToString)

                        If (dtaux.Rows.Count > 0) Then
                            Log.GravarLog("INSERIU DADOS NA TABELA ESPELHO DE FATURAMENTO COPR_TCAD_FILIAL_FAT CODFIL = " & dtaux.Rows(0).Item("CODFIL") & " NOMFIL = " & dtaux.Rows(0).Item("NOMFIL") & " CNP = " & dtaux.Rows(0).Item("CNP"), pNomeArquivoLog)
                            Dados.InsereTabelaEspelhoFaturamento(dtaux.Rows(0).Item("CODFIL").ToString, dtaux.Rows(0).Item("NOMFIL").ToString, dtaux.Rows(0).Item("CNP").ToString, objtransacao_MARTE)

                            Log.GravarLog("INSERIU DADOS NA TABELA DEPARA DE FATURAMENTO COPR_TFILIAL_COM_X_FAT COMERCIAL = " & dr.Item("COMERCIAL") & " CODEMP = " & dtaux.Rows(0).Item("CODEMP") & " CODFIL = " & dtaux.Rows(0).Item("CODFIL"), pNomeArquivoLog)
                            Dados.InsereDeParaFaturamento(dr.Item("COMERCIAL"), drProfat.Item("COD_PARAM_TAB").ToString, dtaux.Rows(0).Item("CODFIL"), objtransacao_MARTE)
                        Else
                            Log.GravarLog("NÃO FOI ENCONTRADO A FILIAL COM COD_PROFAT " & drProfat.Item("COD_PROFAT").ToString(), pNomeArquivoLog)
                            Log.GravarLog("NÃO FOI CRIADO DE-PARA NAS TABELAS COPR_TCAD_FILIAL_FAT E COPR_TFILIAL_COM_X_FAT ", pNomeArquivoLog)
                        End If

                    Else
                        Log.GravarLog("NÃO FOI ENCONTRADO VALOR DO CAMPO COD_PROFAT DO DE-PARA PARA A FILIAL COMERCIAL " & dr.Item("COMERCIAL"), pNomeArquivoLog)
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
            Log.GravarLog(ex.ToString, pNomeArquivoLog)

            Log.GravarLog("Executando o ROLLBACK no NOVI e MARTE", pNomeArquivoLog)
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
        'Dim objtransacao_PROFAT As IDbTransaction

        Try

            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE TIPO DE POSTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE TIPO DE POSTO")

            Dim cont As Integer = 0
            Log.GravarLog("RECUPERAR DADOS DO MARTE PARA FAZER A NORMALIZAÇÃO", pNomeArquivoLog)
            Dim dtTipoPosto As DataTable = Dados.RetornaTipoPostoMARTE()

            Log.GravarLog("ABRINDO CONEXÃO COM O NOVI", pNomeArquivoLog)
            Dim conn_NOVI As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_NOVI)
            'Log.GravarLog("ABRINDO CONEXÃO COM O PROFAT", pNomeArquivoLog)
            'Dim conn_PROFAT As New SqlConnection(AcessoDados.RecuperarStringConexao(Dados.CONEXAO_PROFAT))
            'conn_PROFAT.Open()

            'objtransacao_PROFAT = conn_PROFAT.BeginTransaction
            Log.GravarLog("INICIANDO TRANSAÇÃO NO NOVI", pNomeArquivoLog)
            objtransacao_NOVI = conn_NOVI.BeginTransaction

            AlteraStatusProcessamento("PROCESSANDO...")
            For Each dr As DataRow In dtTipoPosto.Rows
                Log.GravarLog("ATUALIZANDO A TABELA XELEM COD_TABLA = 73, COD_ATRIBUTO (2,3,4) E COD_ELEMENTO = '" & dr.Item("COD_NOVI") & "', COM INFORMAÇÕES DO TIPO DE POSTO COMERCIAL NO CAMPO COD_COMERCIAL = " & dr.Item("COD_COMERCIAL"), pNomeArquivoLog)
                Dados.AtualizaCodComercialTipoPostoNOVI(dr.Item("COD_COMERCIAL"), dr.Item("COD_NOVI"), objtransacao_NOVI)

                'DESLIGADO: não existe de-para com o PROFAT do TIPO DE POSTO.
                'Log.GravarLog("VERIFICANDO SE O TIPO DE POSTO JÁ EXISTE NO PROFAT COD = '" & dr.Item("COD_COMERCIAL") & "', SE SIM ATUALIZOU O CAMPO CODFUNPATCOM COM TAL CODIGO, SENÃO CRIOU O ITEM " & dr.Item("COD_NOVI"), pNomeArquivoLog)
                'Dados.MergeTipoPostoPROFAT(dr.Item("COD_NOVI"), dr.Item("DES_TIPO_PUESTO"), dr.Item("COD_COMERCIAL"), objtransacao_PROFAT)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO... " & vbNewLine & " Item " & cont & " de " & dtTipoPosto.Rows.Count)
            Next

            Log.GravarLog("COMMIT NO NOVI", pNomeArquivoLog)
            'objtransacao_PROFAT.Commit()
            objtransacao_NOVI.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE TIPO DE POSTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE TIPO DE POSTO...")

        Catch ex As Exception

            Log.GravarLog(ex.ToString, pNomeArquivoLog)

            Log.GravarLog("ROLLBACK NO NOVI!", pNomeArquivoLog)
            'objtransacao_PROFAT.Rollback()
            objtransacao_NOVI.Rollback()

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
            'Dim dtSegmentoMarte As DataTable
            'Dim dtSubSegmentoMarte As DataTable
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

            'APAGAR TODOS OS SEGMENTOS NO MARTE.
            Log.GravarLog("APAGANDO TODOS OS SEGMENTOS NO MAESTRO DO MARTE", pNomeArquivoLog)
            Dados.ApagarSegmento(objtransacao_MARTE)

            'APAGAR TODOS OS SUBSEGMENTOS NO MARTE.
            Log.GravarLog("APAGANDO TODOS OS SUBSEGMENTOS NO MAESTRO DO MARTE", pNomeArquivoLog)
            Dados.ApagarSubSegmento(objtransacao_MARTE)

            'INSERIR SEGMENTOS TOMANDO COMO BASE O PROFAT
            For Each dr As DataRow In dtSeg.Rows

                ' Verifica se este segmento já está normalizado.
                If sCodAnterior <> dr.Item("CODRAMATV").ToString() Then
                    Log.GravarLog("NORMALIZANDO O SEGMENTO " & dr.Item("CODRAMATV").ToString() & " " & dr.Item("DESRAMATV").ToString(), pNomeArquivoLog)

                    sCodSeg = dr.Item("CODRAMATV").ToString()
                    sOidSeg = String.Empty
                    sCodAnterior = dr.Item("CODRAMATV").ToString()

                    sOidSegNOVO = Dados.MergeSegmentoMARTE(sCodSeg, dr.Item("DESRAMATV").ToString, dr.Item("CODRAMATV").ToString, sOidSeg, objtransacao_MARTE)

                    Log.GravarLog("CRIANDO DE-PARA MARTExPROFAT DO SEGMENTO " & dr.Item("CODRAMATV").ToString() & " " & dr.Item("DESRAMATV").ToString(), pNomeArquivoLog)
                    Dados.InsereDeParaSegmentoSubSeg(sCodSeg, dr.Item("CODRAMATV").ToString, "1", objtransacao_MARTE)
                End If

                'NORMALIZAR OS SUBSEGMENTOS DESTE SEGMENTO
                Log.GravarLog("  -> SEG:" & dr.Item("CODRAMATV").ToString & " NORMALIZANDO O SUBSEGMENTO:" & dr.Item("CODSUBRAMATV").ToString() & " " & dr.Item("DESSUBRAMATV").ToString(), pNomeArquivoLog)
                sOidSubSeg = String.Empty
                sCodSubSeg = dr.Item("CODSUBRAMATV").ToString()
                sOidSubSegNOVO = Dados.MergeSubSegmentoMARTE(sCodSubSeg,
                                                             dr.Item("DESSUBRAMATV").ToString,
                                                             dr.Item("CODSUBRAMATV").ToString,
                                                             sOidSegNOVO,
                                                             sOidSubSeg,
                                                             objtransacao_MARTE)

                Log.GravarLog("  -> INSERINDO O DE-PARA DE SUBSEGMENTO, CODMARTE=" & sOidSubSegNOVO & " (" & sCodSubSeg & ") CODPROFAT=" & dr.Item("CODSUBRAMATV").ToString, pNomeArquivoLog)
                Dados.InsereDeParaSegmentoSubSeg(sOidSubSegNOVO, dr.Item("CODSUBRAMATV").ToString, "2", objtransacao_MARTE)

                cont = cont + 1
                AlteraStatusProcessamento("PROCESSANDO SEGMENTOS:" & vbNewLine & " Item " & cont & " de " & dtSeg.Rows.Count)
            Next

            objtransacao_MARTE.Commit()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE SUBSEGMENTO ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE SUBSEGMENTO...")

            'TODO: Colocar um msgbox para avisar que tem que rodar a normalização de cliente e subcliete para acertar os segmentos e subsegmentos.
            MsgBox("Normalização terminada!" & vbCrLf & "Não esqueça de executar a normalização de cliente para atualizar o SEGMENTO/SUBSEGMENTO nos clientes.", MsgBoxStyle.Information, "Aviso")

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
            Log.GravarLog(ex.ToString, pNomeArquivoLog)

            Log.GravarLog("Executando Rollback!", pNomeArquivoLog)
            objtransacao_PROFAT.Rollback()
            objtransacao_NOVI.Rollback()

            Log.GravarLog(ex.ToString, pNomeArquivoLog)
            Throw ex
        End Try

        Return True
    End Function

#End Region

#Region "ESCALA"
    Public Shared Function NormalizaEscala(ByVal pNomeArquivoLog As String) As Boolean

        Dim objtransacao_PROFAT As IDbTransaction
        Dim objtransacao_MARTE As IDbTransaction
        Dim DadosMaestro As New DataTable
        Dim EscalaPROFAT As New DataTable
        Dim CodEsc As Integer

        Try
            Log.GravarLog("INICIO DA NORMALIZAÇÃO DE ESCALA ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA NORMALIZAÇÃO DE ESCALA")

            Log.GravarLog("BUSCAR DADOS DO MARTE PARA A MEMORIA", pNomeArquivoLog)
            DadosMaestro = Dados.BuscarEscalaMARTE()

            Log.GravarLog("ABRINDO CONEXÃO COM PROFAT E INICIANDO TRANSAÇÃO", pNomeArquivoLog)
            Dim conn_PROFAT As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_PROFAT)
            objtransacao_PROFAT = conn_PROFAT.BeginTransaction

            Log.GravarLog("ABRINDO CONEXÃO COM O MARTE", pNomeArquivoLog)
            Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)
            objtransacao_MARTE = conn_MARTE.BeginTransaction()

            'ZERAR O CAMPO CODMARTE DENTRO DO PROFAT
            Dados.ZerarCodMarte_EscalasPROFAT(objtransacao_PROFAT)

            'SERÁ FEITO SOMENTE PARA NOVAS ESCALAS NO MARTE QUE NÃO EXISTAM DIRETAMENTE NO PROFAT.
            'O DE-PARA SERÁ ATUALIZADO PARA QUE O IVA FIQUE OK ENQUANTO EXISTIR.
            For Each dr As DataRow In DadosMaestro.Rows
                Log.GravarLog("REALIZAR O MERGE NO PROFAT DA ESCALA MARTE " & dr("COD_ESCALA").ToString(), pNomeArquivoLog)
                CodEsc = Dados.MergeEscalaProfat(dr("COD_ESCALA"), dr("DES_ESCALA"), dr("COD_PROFAT"), objtransacao_PROFAT)

                If (CodEsc > 0) Then
                    'TODO ATUALIZAR O DE-PARA
                    Log.GravarLog("ATUALIZANDO O DE-PARA DA ESCALA " & dr("COD_ESCALA").ToString() & " (" & dr("OID_ESCALA") & ") " & " COD_PROFAT=" & CodEsc.ToString(), pNomeArquivoLog)
                    Dados.InsereEscalaDePara(dr("OID_ESCALA"), CodEsc.ToString(), objtransacao_MARTE)
                End If
            Next

            Log.GravarLog("COMMIT NO PROFAT", pNomeArquivoLog)
            objtransacao_PROFAT.Commit()
            objtransacao_MARTE.Commit()
            'PARA TESTES
            'objtransacao_PROFAT.Rollback()
            'objtransacao_MARTE.Rollback()

            Log.GravarLog("FIM DA NORMALIZAÇÃO DE ESCALA ----------------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("FIM DA NORMALIZAÇÃO DE ESCALA")
        Catch ex As Exception
            Log.GravarLog(ex.ToString, pNomeArquivoLog)

            Log.GravarLog("Realizando o ROLLBACK no PROFAT", pNomeArquivoLog)
            If (objtransacao_PROFAT IsNot Nothing) And (objtransacao_PROFAT.Connection.State = ConnectionState.Open) Then
                objtransacao_PROFAT.Rollback()
            End If
            objtransacao_MARTE.Rollback()

            Throw ex
        End Try

        Return True
    End Function
#End Region

#Region "CARGAS_PROFAT"

    Public Shared Sub CargaPROFATPostos01(ByVal pNomeArquivoLog As String)

        '15/02/2018
        '1) Recebi a carga em Excel e gerei um script. [somente create table d:\Prosegur\profat\01-create_table.sql]
        '2) Fiz os inserts na tabela marte.TMP_AcertoPROFAT [d:\Prosegur\profat\02-carga_excel.sql]
        '3) adicionei os campos que seram carregados com os dados do MARTE. [ALTER TABLE do arquivo d:\Prosegur\profat\01-create_table.sql]
        '4) executar esse código 
        '5) exportar para excel direto do PL/SQL

        'TESTE
        Dim contador As Integer
        Dim QtdePostos As Integer

        Dim objtransacao_MARTE As IDbTransaction
        Dim DadosTMP_AcertoProfat As New DataTable
        Dim DadosPostosMarte As New DataTable

        Dim mCodUnicoPosto As String
        Dim mFechaInicio As String
        Dim mHoraFechaInicio As String
        Dim mFechaFin As String
        Dim mHoraFechaFin As String
        Dim mDiasTrabajo As String
        Dim mTipoDia As String
        Dim mHorarios As String
        Dim mNumeroHoras As String
        Dim mIntervaloAlmuerzo As String
        Dim mTrabajaAlmuerzo As String
        Dim mQtdePostos As String

        Dim vData As Date
        Dim vHora As Date
        Dim vHorasPorDiaXVigilante As Decimal
        Dim vHorasAlmoco As Decimal
        Dim ParteInteira As Decimal
        Dim ParteDecimal As Decimal

        Try
            Log.GravarLog("INICIO DA CARGA DE DADOS PARA PROFAT---------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIO DA CARGA DE DADOS PARA PROFAT")

            Log.GravarLog("BUSCAR DADOS DO MARTE PARA A MEMORIA", pNomeArquivoLog)
            DadosTMP_AcertoProfat = Dados.BuscarTMP_AcertoPROFAT()

            Log.GravarLog("ABRINDO CONEXÃO COM O MARTE", pNomeArquivoLog)
            Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)

            For Each drPROFAT As DataRow In DadosTMP_AcertoProfat.Rows
                Log.GravarLog("INICIANDO TRANSAÇÃO", pNomeArquivoLog)
                objtransacao_MARTE = conn_MARTE.BeginTransaction()

                Log.GravarLog("Buscando dados OIDPUEMAR : " & drPROFAT("OIDPUEMAR"), pNomeArquivoLog)

                DadosPostosMarte = Dados.BuscarDadosPostosMARTE(drPROFAT("OIDPUEMAR"))

                QtdePostos = DadosPostosMarte.Rows.Count
                For Each dr As DataRow In DadosPostosMarte.Rows
                    'INICIAR TRATAMENTO DOS CAMPOS
                    mCodUnicoPosto = dr("cod_empresa_erp").ToString().Trim()
                    mCodUnicoPosto = mCodUnicoPosto & "|" & dr("cod_cliente").ToString().Trim()
                    mCodUnicoPosto = mCodUnicoPosto & "|" & dr("cod_subcliente").ToString().Trim()
                    mCodUnicoPosto = mCodUnicoPosto & "|" & dr("cod_puesto").ToString().Trim()

                    Log.GravarLog("Código único posto a processar: " & mCodUnicoPosto, pNomeArquivoLog)

                    'If (mCodUnicoPosto = "B13|016930|238|4") Then
                    '    contador = contador
                    'End If

                    vData = DateTime.Parse(dr("fec_inicio_servicio"))
                    mFechaInicio = vData.Year.ToString() & "-" & vData.Month.ToString("00") & "-" & vData.Day.ToString("00")

                    If (Not IsDBNull(dr("hor_inicio_servicio"))) Then
                        vHora = DateTime.Parse(dr("hor_inicio_servicio"))
                        mHoraFechaInicio = vHora.Hour.ToString("00") & ":" & vHora.Minute.ToString("00") & ":" & vHora.Second.ToString("00")
                    Else
                        mHoraFechaInicio = String.Empty
                    End If

                    If (IsDBNull(dr("fec_fin_servicio"))) Then
                        'busca a data da baixa, se houver
                        If (IsDBNull(dr("fec_baixa"))) Then
                            'não tem baixa
                            mFechaFin = String.Empty
                            mHoraFechaFin = String.Empty
                        Else
                            vData = DateTime.Parse(dr("fec_baixa"))
                            mFechaFin = vData.Year.ToString("00") & "-" & vData.Month.ToString("00") & "-" & vData.Day.ToString("00")
                            If (IsDBNull(dr("hor_baixa"))) Then
                                mHoraFechaFin = String.Empty
                            Else
                                vHora = DateTime.Parse(dr("hor_baixa"))
                                mHoraFechaFin = vHora.Hour.ToString("00") & ":" & vHora.Minute.ToString("00") & ":" & vHora.Second.ToString("00")
                            End If
                        End If
                    Else
                        'busca a data da baixa para os ESPO.
                        vData = DateTime.Parse(dr("fec_fin_servicio"))
                        mFechaFin = vData.Year.ToString("00") & "-" & vData.Month.ToString("00") & "-" & vData.Day.ToString("00")
                        If (IsDBNull(dr("hor_fin_servicio"))) Then
                            mHoraFechaFin = String.Empty
                        Else
                            vHora = DateTime.Parse(dr("hor_fin_servicio"))
                            mHoraFechaFin = vHora.Hour.ToString("00") & ":" & vHora.Minute.ToString("00") & ":" & vHora.Second.ToString("00")
                        End If

                    End If

                    mDiasTrabajo = dr("segunda").ToString &
                                   dr("terça").ToString &
                                   dr("quarta").ToString &
                                   dr("quinta").ToString &
                                   dr("sexta").ToString &
                                   dr("sabado").ToString &
                                   dr("domingo").ToString

                    mTipoDia = dr("oid_tipo_jornada").ToString().Trim()

                    vHora = DateTime.Parse(dr("hor_inicio_1"))
                    mHorarios = vHora.Hour.ToString("00") & ":" & vHora.Minute.ToString("00")
                    vHora = DateTime.Parse(dr("hor_fin_1"))
                    mHorarios = mHorarios & "-" & vHora.Hour.ToString("00") & ":" & vHora.Minute.ToString("00")

                    vHorasPorDiaXVigilante = dr("HorasDiaXVigilante")
                    ParteInteira = Math.Truncate(vHorasPorDiaXVigilante)
                    ParteDecimal = (vHorasPorDiaXVigilante - ParteInteira) * 60
                    mNumeroHoras = ParteInteira.ToString("00") & ParteDecimal.ToString("00")

                    vHorasAlmoco = dr("AlmocoHoras")
                    ParteInteira = Math.Truncate(vHorasAlmoco)
                    ParteDecimal = (vHorasAlmoco - ParteInteira) * 60
                    mIntervaloAlmuerzo = ParteInteira.ToString("00") & ":" & ParteDecimal.ToString("00")

                    mTrabajaAlmuerzo = dr("des_cobertura_almuerzo").ToString().Trim()

                    mQtdePostos = dr("num_cantidad_pto").ToString.Trim

                    'ATUALIZAR A TABELA TEMPORÁRIA.
                    Dados.AtualizarTMP_AcertoPROFAT(objtransacao_MARTE,
                                                    mCodUnicoPosto,
                                                    mFechaInicio,
                                                    mHoraFechaInicio,
                                                    mFechaFin,
                                                    mHoraFechaFin,
                                                    mDiasTrabajo,
                                                    mTipoDia,
                                                    mHorarios,
                                                    mNumeroHoras,
                                                    mIntervaloAlmuerzo,
                                                    mTrabajaAlmuerzo,
                                                    drPROFAT("OIDPUEMAR"),
                                                    QtdePostos,
                                                    mQtdePostos)
                Next

                Log.GravarLog("COMITANDO TRANSAÇÃO", pNomeArquivoLog)
                objtransacao_MARTE.Commit()

                DadosPostosMarte.Clear()

                'TESTE
                contador = contador + 1

                Log.GravarLog(String.Format("Registro número -> {0}", contador), pNomeArquivoLog)
                'If (contador > 10) Then
                '    Exit For
                'End If

            Next

            DadosTMP_AcertoProfat.Clear()
            conn_MARTE.Close()
            conn_MARTE.Dispose()

        Catch ex As Exception
            Log.GravarLog(ex.ToString, pNomeArquivoLog)

            Log.GravarLog("Realizando o ROLLBACK no MARTE", pNomeArquivoLog)
            objtransacao_MARTE.Rollback()

            Throw ex
        End Try

    End Sub

    Public Shared Sub CargaPROFATGerarDUO_OT(ByVal pNomeArquivoLog As String)
        'rotina que lê uma tabela em que carreguei os oidpuemar enviado pela equipe PROFAT.
        '00-ENVIO_AUTOMATICO_CRIARTABELA.sql
        '00-ENVIO_AUTOMATICO_CARGAEXCEL.sql

        Dim objtransacao_MARTE As IDbTransaction
        Dim TransacaoAberta As Boolean = False

        Dim TMP_CARGAENVIOOTS_SUB As New DataTable
        Dim TMP_CARGAENVIOOTS_POSTOS As New DataTable
        Dim OTsParaAgendar As New DataTable

        Dim ListaPostosIN As String
        Dim OID_ITEM_PROCESSO As String
        Dim DataAgendamento As Date = Date.Now

        Try
            Log.GravarLog("INICIO DO PROCESSO DE GERAÇÃO DE AGENDAMENTOS DUO PARA PROFAT---------------------------------", pNomeArquivoLog)
            AlteraStatusProcessamento("INICIANDO")

            Log.GravarLog("ABRINDO CONEXÃO COM O MARTE", pNomeArquivoLog)
            Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE_DSV)

            'BUSCAR OS DADOS DA TEMPORÁRIA EM DSV.
            TMP_CARGAENVIOOTS_SUB = Dados.BuscarTMP_CARGAENVIOOTS()

            For Each drSub As DataRow In TMP_CARGAENVIOOTS_SUB.Rows
                'AQUI VOU TER EMPRESA,CLIENTE E SUB
                Log.GravarLog($"Empresa = {drSub("P_CODEMPRESA")}; Cliente = {drSub("P_CODCLIENTE")}; Subcliente = {drSub("P_CODSUBCLI")}", pNomeArquivoLog)
                'Dim aOIDPUEMAR As String() = drSub("P_OIDPUEMAR").ToString.Split("|")

                'buscar os postos desta combinação de empresa, cliente e subcliente
                TMP_CARGAENVIOOTS_POSTOS = Dados.BuscarPostosListaPROFAT(drSub("P_CODEMPRESA"), drSub("P_CODCLIENTE"), drSub("P_CODSUBCLI"))

                'Montar o IN(lista de postos)
                ListaPostosIN = String.Empty
                For Each drPostos As DataRow In TMP_CARGAENVIOOTS_POSTOS.Rows
                    Dim aOIDPUEMAR_POSTO As String() = drPostos("P_OIDPUEMAR").ToString.Split("|")
                    ListaPostosIN = ListaPostosIN & aOIDPUEMAR_POSTO(3) & ","
                Next
                'Retirar a última virgula
                ListaPostosIN = ListaPostosIN.Substring(0, ListaPostosIN.Length - 1)

                TMP_CARGAENVIOOTS_POSTOS.Clear()

                'BUSCAR AS OTS, ORDENADAS POR DATA.
                Log.GravarLog("BUSCANDO OTS", pNomeArquivoLog)
                OTsParaAgendar = Dados.BuscarOTPorEmpresaClienteSubPosto(drSub("P_CODEMPRESA"), drSub("P_CODCLIENTE"), drSub("P_CODSUBCLI"), ListaPostosIN)

                If (OTsParaAgendar.Rows.Count > 0) Then
                    Log.GravarLog("INICIANDO TRANSAÇÃO", pNomeArquivoLog)
                    objtransacao_MARTE = conn_MARTE.BeginTransaction()
                    TransacaoAberta = True

                    For Each drOT As DataRow In OTsParaAgendar.Rows
                        'MONTAR OS AGENDAMENTOS.
                        Log.GravarLog($"AGENDAR ITEM PROCESSO: |{drOT("cod_empresa").ToString.Trim}|{drOT("cod_cliente").ToString.Trim}|{drOT("cod_subcliente").ToString.Trim}|{drOT("num_nro_ot").ToString.Trim}|{drOT("oid_tipo_servicio").ToString.Trim}|{drOT("cod_comprobante").ToString.Trim}|", pNomeArquivoLog)
                        Log.GravarLog($"AGENDADO PARA ÀS {DataAgendamento.ToString()}", pNomeArquivoLog)
                        OID_ITEM_PROCESSO = Dados.AgendarSincronizacaoDUOOT(drOT("cod_empresa"),
                                                                            drOT("cod_cliente"),
                                                                            drOT("cod_subcliente"),
                                                                            drOT("num_nro_ot"),
                                                                            drOT("oid_tipo_servicio"),
                                                                            drOT("cod_comprobante"),
                                                                            DataAgendamento,
                                                                            objtransacao_MARTE)
                        Log.GravarLog($"AGENDADO ITEM PROCESSO: {OID_ITEM_PROCESSO}", pNomeArquivoLog)

                        'AVANÇAR A DATA DE AGENDAMENTO PARA AS PRÓXIMAS OTS
                        ' ISSO É PARA EVITAR DE DUAS OTS SEREM EXECUTADAS NO MESMO SEGUNDO, FAZENDO COM QUE AS OTS SEJAM EXECUTADAS FORA DE ORDEM
                        DataAgendamento = DataAgendamento.AddSeconds(30)
                    Next

                    Log.GravarLog($"COMMIT NA TRANSAÇÃO - |{drSub("P_CODEMPRESA")}|{drSub("P_CODCLIENTE")}|{drSub("P_CODSUBCLI")}|", pNomeArquivoLog)
                    objtransacao_MARTE.Commit()
                    TransacaoAberta = False
                Else
                    Log.GravarLog($"Não foi encontrado serviço para a chave Empresa/Cliente/Subcliente - |{drSub("P_CODEMPRESA")}|{drSub("P_CODCLIENTE")}|{drSub("P_CODSUBCLI")}|" & Environment.NewLine & $" postos {ListaPostosIN}", pNomeArquivoLog)
                End If

                'PARA TESTES
                'Exit For
            Next
            TMP_CARGAENVIOOTS_SUB.Clear()

            Log.GravarLog($"EXECUÇÃO FINALIZADA COM SUCESSO ÀS {Date.Now.ToString()}", pNomeArquivoLog)

        Catch ex As Exception

            Log.GravarLog("Ocorreu uma exceção!", pNomeArquivoLog)
            Log.GravarLog(ex.ToString, pNomeArquivoLog)

            If (TransacaoAberta) Then
                Log.GravarLog("Executar ROLLBACK!", pNomeArquivoLog)
                objtransacao_MARTE.Rollback()
            End If


            Throw ex
        End Try
    End Sub

#End Region

#Region "GERAR_PAYLOAD"
    Public Shared Sub GerarPayload()

        Dim ListaPayloads As New DataTable
        Dim sbPayload As New StringBuilder
        Dim nomeArquivoPayload As String
        Dim sequencia As Integer

        'INFORMAR A DATA DE FILTRAGEM
        Dim DataCriacao As Date = Date.ParseExact("28/03/2018", "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        'INFORMAR O OID_INTEGRACAO DE FILTRAGEM
        ' OT = "4"
        Dim oidIntegracao As String = "4"

        Try
            sequencia = 1

            ListaPayloads = Dados.BuscarPAYLOADS(DataCriacao, oidIntegracao)

            For Each drPayload In ListaPayloads.Rows

                sbPayload.Append(drPayload("obs_payload"))
                nomeArquivoPayload = $"{sequencia.ToString("0000")}-{drPayload("obs_cod_legado").ToString.Replace("|", "_").Substring(1, drPayload("obs_cod_legado").ToString.Length - 1).Trim}"
                Log.GravarLog(sbPayload.ToString(), nomeArquivoPayload, False)

                sequencia = sequencia + 1
                'Esvaziar o stringbuilder
                sbPayload.Length = 0
            Next

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "EXPORTE_GRUPO_TARIFARIO"
    Public Shared Sub ExporteGrupoTarifario(ByVal pNomeArquivoLog As String)
        'Rotina que gera todos os GRUPOS TARIFARIOS de TODAS AS OTS APROVADAS.
        'TABELA DESTINO
        'create table BRINT_CARGA_GT_RESULTADO (IDPOSTO VARCHAR2(100) Not NULL,
        '                                       DATA_INICIO Date Not NULL,
        '                                       COD_ITEM_TARIFA VARCHAR2(15) Not NULL,
        '                                       DES_ITEM_TARIFA VARCHAR2(50) Not NULL,
        '                                       PRECO FLOAT Not NULL);
        '****
        Dim objtransacao_MARTE As IDbTransaction
        Dim transacaoAberta As Boolean = False
        Dim Tarifarios As New DataTable
        Dim conn_MARTE As IDbConnection = AcessoDados.Conectar(Dados.CONEXAO_MARTE)
        Dim IDPosto As String
        Dim DataInicio As Date
        Dim CodItemTarifario As String
        Dim DesItemTarifario As String
        Dim Preco As Decimal

        Dim LinhasEsquema As DataTable
        Dim EncontradaPeloTarifario As Boolean

        Dim ExportarLinhaTarifario As Boolean = False

        Try
            Log.GravarLog("EXECUÇÃO INICIADA.", pNomeArquivoLog)

            Log.GravarLog("ZERANDO TABELA DE EXPORTE BRINT_CARGA_GT_RESULTADO", pNomeArquivoLog)
            objtransacao_MARTE = conn_MARTE.BeginTransaction()
            Dados.ApagaTodosRegistrosResultado(objtransacao_MARTE)
            objtransacao_MARTE.Commit()

            'Buscar os dados de todos os grupos tarifários
            Log.GravarLog("INICIANDO BUSCA DOS GRUPOS TARIFARIOS.", pNomeArquivoLog)
            Tarifarios = Dados.BuscarGruposTarifariosOT()
            Log.GravarLog("FINALIZADA BUSCA DOS GRUPOS TARIFARIOS.", pNomeArquivoLog)

            Log.GravarLog($"RETORNADOS {Tarifarios.Rows.Count} TARIFÁRIOS A EXPORTAR.", pNomeArquivoLog)
            For Each Tarifario As DataRow In Tarifarios.Rows
                'Verificar se o tarifario é de uma OT com ou sem linhas de esquema.
                If (IsDBNull(Tarifario.Item("OID_PUESTOSXOT"))) Then
                    'OT sem linha de esquema
                    LinhasEsquema = Dados.RetornaLinhaEsquemaPorTarifario(Tarifario.Item("OID_GRUPO_TARIFARIO"))
                    EncontradaPeloTarifario = True
                Else
                    'OT com linha de esquema
                    LinhasEsquema = Dados.RetornaLinhaEsquema(Tarifario.Item("OID_PUESTOSXOT"))
                    EncontradaPeloTarifario = False
                End If

                If (LinhasEsquema.Rows.Count > 0) Then
                    If (LinhasEsquema.Rows.Count > 1) Then
                        Dim mensagemErro = $"Encontrado mais de um OID_PUESTOSXOT {Tarifario.Item("OID_PUESTOSXOT")}"
                        'Throw New Exception(mensagemErro)
                    End If
                    For Each linhaEsquema As DataRow In LinhasEsquema.Rows
                        'Não exportaremos os grupos tarifários da linha de baixa.
                        If (linhaEsquema.Item("des_condicion") <> "B") Then
                            IDPosto = MontaIDPosto(linhaEsquema.Item("COD_EMPRESA_ERP"),
                                                   linhaEsquema.Item("COD_CLIENTE"),
                                                   linhaEsquema.Item("COD_SUBCLIENTE"),
                                                   linhaEsquema.Item("COD_PUESTO"))
                            'DEBUG
                            'If (IDPosto = "B01|001056|13|1") Then
                            '    Dim mensagemErro = "teste"
                            'End If
                            If (EncontradaPeloTarifario) Then
                                DataInicio = Tarifario.Item("DATA_INICIO_OT")
                            Else
                                DataInicio = linhaEsquema.Item("FEC_INICIO_SERVICIO")
                            End If

                            CodItemTarifario = Tarifario.Item("COD_ITEM_TARIFA")
                            DesItemTarifario = Tarifario.Item("DES_ITEM_TARIFA")
                            'TODO: ZERAR PREÇOS DE ITENS DE TARIFA 30 E 31 DE POSTOS COMPLEMENTARES.
                            If (linhaEsquema.Item("posto_principal") = "1" OrElse CodItemTarifario = "34") Then
                                'Retorna os preços para todos os itens de tarifa de postos principais
                                'e para o item de tarifa 34 para as linhas complementares
                                Preco = Tarifario.Item("NUM_PRECIO")
                            Else
                                Preco = 0
                            End If

                            objtransacao_MARTE = conn_MARTE.BeginTransaction()
                            transacaoAberta = True
                            Dados.GravaTarifaParaExporte(objtransacao_MARTE,
                                                         IDPosto,
                                                         DataInicio,
                                                         CodItemTarifario,
                                                         DesItemTarifario,
                                                         Preco,
                                                         EncontradaPeloTarifario)
                            objtransacao_MARTE.Commit()
                            transacaoAberta = False
                        End If
                    Next
                Else
                    'Throw New Exception($"Não encontrado o OID_PUESTOSXOT {Tarifario.Item("OID_PUESTOSXOT")}")
                    Log.GravarLog("NÃO FOI ENCONTRADA LINHA NA COPR_TPUESTOSXOT", pNomeArquivoLog)
                    Log.GravarLog($"EncontradaPeloTarifario = {EncontradaPeloTarifario.ToString()}", pNomeArquivoLog)
                    If (EncontradaPeloTarifario) Then
                        Log.GravarLog($"NÃO FOI ENCONTRADA LINHA PELO FILTRO TARIFARIO '{Tarifario.Item("OID_GRUPO_TARIFARIO")}'", pNomeArquivoLog)
                    Else
                        Log.GravarLog($"NÃO FOI ENCONTRADA LINHA PELO FILTRO OID_PUESTOSXOT '{Tarifario.Item("OID_PUESTOSXOT")}'", pNomeArquivoLog)
                    End If
                End If

            Next
        Catch ex As Exception
            If transacaoAberta Then
                objtransacao_MARTE.Rollback()
            End If
            Throw ex
        End Try

    End Sub

    Private Shared Function MontaIDPosto(pCodEmpresa As String,
                                         pCodCliente As String,
                                         pCodSubcliente As String,
                                         pIDPosto As String) As String

        Return pCodEmpresa.Trim() + "|" + pCodCliente.Trim() + "|" + pCodSubcliente.Trim() + "|" + pIDPosto.Trim()

    End Function

    Private Shared Sub ExportarGrupoTarifario(pNomeArquivo As String)



    End Sub
#End Region

    Public Shared Sub AlteraStatusProcessamento(ByVal ptexto As String)
        frmMigracao.txtStatus.Text = ptexto
        frmMigracao.txtStatus.Refresh()
    End Sub

End Class
