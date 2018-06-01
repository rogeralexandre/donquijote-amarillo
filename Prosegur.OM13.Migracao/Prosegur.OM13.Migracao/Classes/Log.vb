Imports System.Configuration
Imports System.Text

Public Class Log

    Public Shared ReadOnly Property HabilitarLog() As Boolean
        Get
            Return If(ConfigurationManager.AppSettings("HabilitarLog") = 1, True, False)
        End Get
    End Property

    Public Shared ReadOnly Property DirLogs() As String
        Get
            Return ConfigurationManager.AppSettings("DirLogs")
        End Get
    End Property

    Public Enum SISTEMAS
        PROTAT = 1
        MARTE = 2
        NOVI = 3
    End Enum

    ''' <summary>
    ''' Gera arquivo de log na pasta do processo
    ''' </summary>
    ''' <param name="pMensagem"></param>
    ''' <param name="pNomeArq"></param>
    ''' <remarks></remarks>
    Public Shared Sub GravarLog(ByVal pMensagem As String,
                                ByVal pNomeArq As String,
                                Optional ByVal pColocaData As Boolean = True)

        If Not HabilitarLog Then Return

        Try

            Dim pastaLog As String = DirLogs & "\Log"
            If Not System.IO.Directory.Exists(pastaLog) Then
                System.IO.Directory.CreateDirectory(pastaLog)
            End If

            Dim Arquivo As String = String.Format("{0}\" + pNomeArq + ".txt", pastaLog)
            Using srt As New System.IO.StreamWriter(Arquivo, True)
                If (pColocaData) Then
                    srt.WriteLine(Date.Now.ToString + " - " + pMensagem)
                Else
                    srt.WriteLine(pMensagem.Replace(vbCr, "").Replace(vbLf, "").ToString())
                End If

                srt.Close()
            End Using

        Catch ex As Exception
            Throw ex 'Sobe o erro
        End Try

    End Sub

    Public Shared Sub ApagarLog(ByVal competencia As Boolean)

        Dim pastaLog As String = DirLogs & "\Log\"
        If Not System.IO.Directory.Exists(pastaLog) Then
            Return
        End If
        Dim pNomeArq As String

        If Not competencia Then
            pNomeArq = "consultarPorMes_"
        Else : pNomeArq = "consultarPorMesCompetencia_"
        End If

        pNomeArq = pNomeArq + RTrim(Date.Now.ToShortDateString.Replace(" ", "").Replace("/", ""))

        Dim Arquivo As String = String.Format("{0}" + pNomeArq + ".txt", pastaLog)
        If System.IO.File.Exists(Arquivo) Then
            System.IO.File.Delete(Arquivo)
        End If

    End Sub

End Class
