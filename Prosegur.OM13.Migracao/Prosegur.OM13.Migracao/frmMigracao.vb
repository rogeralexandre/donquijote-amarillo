Public Class frmMigracao

    Private Sub btnCliente_Click(sender As Object, e As EventArgs) Handles btnCliente.Click

        Try

            Dim sNomeArquivoLog As String = "ValidacaoCliente_" & Date.Now.ToString.Replace("/", "").Replace(":", "") & ".txt"
            Normalizacao.NormalizaCliente(sNomeArquivoLog)

        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO CLIENTES")
        End Try

    End Sub

    Private Sub btnSubCliente_Click(sender As Object, e As EventArgs) Handles btnSubCliente.Click

        Try
            Dim sNomeArquivoLog As String = "ValidacaoSubCliente_" & Date.Now.ToString.Replace("/", "").Replace(":", "") & ".txt"
            Normalizacao.NormalizaSubCliente(sNomeArquivoLog)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO SUB CLIENTE")
        End Try

    End Sub

    Private Sub btnLogradouro_Click(sender As Object, e As EventArgs) Handles btnLogradouro.Click

        Try
            Dim sNomeArquivoLog As String = "ValidacaoTipoLogradouro_" & Date.Now.ToString.Replace("/", "").Replace(":", "")
            Normalizacao.NormalizaTipoLogradouro(sNomeArquivoLog)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO TIPO LOGRADOURO")
        End Try

    End Sub

    Private Sub btnCidade_Click(sender As Object, e As EventArgs) Handles btnCidade.Click

        Try
            Dim sNomeArquivoLog As String = "ValidacaoCidade_" & Date.Now.ToString.Replace("/", "").Replace(":", "")
            Normalizacao.NormalizaCidades(sNomeArquivoLog)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO CIDADES")
        End Try

    End Sub

    ' BOTÃO DE REGIONAL
    Private Sub btnDelegacao_Click(sender As Object, e As EventArgs) Handles btnDelegacao.Click

        MsgBox("ESTA NORMALIZAÇÃO FOI DESATIVADA, NÃO IREMOS NORMALIZAR REGIONAIS.", MsgBoxStyle.Information, "NORMALIZAÇÃO REGIONAL")

        ' DESLIGANDO
        If (False) Then
            Try
                Dim sNomeArquivoLog As String = "ValidacaoDelegacao_" & Date.Now.ToString.Replace("/", "").Replace(":", "")
                Normalizacao.NormalizaDelegacao(sNomeArquivoLog)
            Catch ex As Exception
                MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO DELEGAÇÃO")
            End Try
        End If

    End Sub

    Private Sub btnFilial_Click(sender As Object, e As EventArgs) Handles btnFilial.Click
        Try
            Dim sNomeArquivoLog As String = "ValidacaoFilial_" & Date.Now.ToString.Replace("/", "").Replace(":", "")
            Normalizacao.NormalizaFilial(sNomeArquivoLog)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO FILIAL")
        End Try
    End Sub

    Private Sub btnTipoPosto_Click(sender As Object, e As EventArgs) Handles btnTipoPosto.Click
        Try
            Dim sNomeArquivoLog As String = "ValidacaoTipoPosto_" & Date.Now.ToString.Replace("/", "").Replace(":", "")
            Normalizacao.NormalizaTipoPosto(sNomeArquivoLog)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO TIPO DE POSTO")
        End Try
    End Sub

    Private Sub btnSegmento_Click(sender As Object, e As EventArgs) Handles btnSegmento.Click
        Try
            Dim sNomeArquivoLog As String = "ValidacaoSegmento_SubSeg_" & Date.Now.ToString.Replace("/", "").Replace(":", "")
            'Normalizacao.NormalizaSegmentoSubSeg(sNomeArquivoLog)
            Normalizacao.NormalizaSegmentoSubSeg_2(sNomeArquivoLog)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO SEGMENTO/SUB SEGMENTO")
        End Try
    End Sub

    Private Sub btnDivisao_Click(sender As Object, e As EventArgs) Handles btnDivisao.Click

        Try
            Dim sNomeArquivoLog As String = "ValidacaoDivisao_" & Date.Now.ToString.Replace("/", "").Replace(":", "")

            Normalizacao.NormalizaDivisao(sNomeArquivoLog)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO DIVISÃO")
        End Try

    End Sub

    Private Sub btnEscala_Click(sender As Object, e As EventArgs) Handles btnEscala.Click
        Try
            Dim sNomeArquivoLog As String = "Normaliza_Escala" & Date.Now.ToString.Replace("/", "").Replace(":", "")

            Normalizacao.NormalizaEscala(sNomeArquivoLog)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "NORMALIZAÇÃO ESCALA")
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim sNomeArquivoLog As String = "CARGA_PROFAT_POSTOS_01" & Date.Now.ToString.Replace("/", "").Replace(":", "")

            Normalizacao.CargaPROFATPostos01(sNomeArquivoLog)
            MsgBox("Carga postos PROFAT finalizado!", MsgBoxStyle.Information, "Aviso")

        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "CARGA DADOS FALTANTES DE POSTOS - PROFAT")
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim sNomeArquivoLog As String = "CARGA_PROFAT_GERAR_SINCRONIA_DUO_OT" & Date.Now.ToString.Replace("/", "").Replace(":", "")

            Normalizacao.CargaPROFATGerarDUO_OT(sNomeArquivoLog)
            MsgBox("Geração dos agendamentos finalizado!", MsgBoxStyle.Information, "Aviso")

        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "PROFAT - GERAR SINCRONIA DUO OT")
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            'Dim sNomeArquivoLog As String = "PAYLOAD" & Date.Now.ToString.Replace("/", "").Replace(":", "")

            Normalizacao.GerarPayload()
            MsgBox("Geração dos payloads finalizado!", MsgBoxStyle.Information, "Aviso")

        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "GERAR PAYLOADS")
        End Try
    End Sub

    Private Sub btnGerarGrupoTarifario_Click(sender As Object, e As EventArgs) Handles btnGerarGrupoTarifario.Click
        Try
            Dim sNomeArquivoLog As String = "TARIFARIO_" & Date.Now.ToString.Replace("/", "").Replace(":", "")

            Normalizacao.ExporteGrupoTarifario(sNomeArquivoLog)
            MsgBox("Exporte do tarifario terminado!", MsgBoxStyle.Information, "Aviso")

        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace, MsgBoxStyle.Critical, "EXPORTE TARIFARIO")
        End Try

    End Sub
End Class
