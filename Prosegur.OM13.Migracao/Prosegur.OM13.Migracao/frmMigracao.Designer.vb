<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMigracao
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMigracao))
        Me.btnCliente = New System.Windows.Forms.Button()
        Me.txtStatus = New System.Windows.Forms.TextBox()
        Me.lblTitulo = New System.Windows.Forms.Label()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.btnSubCliente = New System.Windows.Forms.Button()
        Me.btnCidade = New System.Windows.Forms.Button()
        Me.btnDivisao = New System.Windows.Forms.Button()
        Me.btnFilial = New System.Windows.Forms.Button()
        Me.btnDelegacao = New System.Windows.Forms.Button()
        Me.btnTipoPosto = New System.Windows.Forms.Button()
        Me.btnEscala = New System.Windows.Forms.Button()
        Me.btnSegmento = New System.Windows.Forms.Button()
        Me.btnLogradouro = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.btnGerarGrupoTarifario = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnCliente
        '
        Me.btnCliente.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCliente.Location = New System.Drawing.Point(351, 253)
        Me.btnCliente.Name = "btnCliente"
        Me.btnCliente.Size = New System.Drawing.Size(102, 55)
        Me.btnCliente.TabIndex = 0
        Me.btnCliente.Text = "CLIENTE (10)"
        Me.btnCliente.UseVisualStyleBackColor = True
        '
        'txtStatus
        '
        Me.txtStatus.Location = New System.Drawing.Point(79, 83)
        Me.txtStatus.Multiline = True
        Me.txtStatus.Name = "txtStatus"
        Me.txtStatus.Size = New System.Drawing.Size(478, 55)
        Me.txtStatus.TabIndex = 1
        '
        'lblTitulo
        '
        Me.lblTitulo.AutoSize = True
        Me.lblTitulo.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitulo.Location = New System.Drawing.Point(134, 26)
        Me.lblTitulo.Name = "lblTitulo"
        Me.lblTitulo.Size = New System.Drawing.Size(328, 31)
        Me.lblTitulo.TabIndex = 2
        Me.lblTitulo.Text = "MIGRAÇÃO DE DADOS"
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(20, 104)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(53, 13)
        Me.lblStatus.TabIndex = 2
        Me.lblStatus.Text = "STATUS:"
        '
        'btnSubCliente
        '
        Me.btnSubCliente.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSubCliente.Location = New System.Drawing.Point(459, 253)
        Me.btnSubCliente.Name = "btnSubCliente"
        Me.btnSubCliente.Size = New System.Drawing.Size(102, 55)
        Me.btnSubCliente.TabIndex = 0
        Me.btnSubCliente.Text = "SUBCLIENTE (11)"
        Me.btnSubCliente.UseVisualStyleBackColor = True
        '
        'btnCidade
        '
        Me.btnCidade.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCidade.Location = New System.Drawing.Point(135, 196)
        Me.btnCidade.Name = "btnCidade"
        Me.btnCidade.Size = New System.Drawing.Size(102, 51)
        Me.btnCidade.TabIndex = 0
        Me.btnCidade.Text = "CIDADE (7)"
        Me.btnCidade.UseVisualStyleBackColor = True
        '
        'btnDivisao
        '
        Me.btnDivisao.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDivisao.Location = New System.Drawing.Point(351, 196)
        Me.btnDivisao.Name = "btnDivisao"
        Me.btnDivisao.Size = New System.Drawing.Size(102, 51)
        Me.btnDivisao.TabIndex = 0
        Me.btnDivisao.Text = "DIVISÃO (9) ??"
        Me.btnDivisao.UseVisualStyleBackColor = True
        '
        'btnFilial
        '
        Me.btnFilial.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFilial.Location = New System.Drawing.Point(459, 196)
        Me.btnFilial.Name = "btnFilial"
        Me.btnFilial.Size = New System.Drawing.Size(102, 51)
        Me.btnFilial.TabIndex = 0
        Me.btnFilial.Text = "FILIAL (4,13)"
        Me.btnFilial.UseVisualStyleBackColor = True
        '
        'btnDelegacao
        '
        Me.btnDelegacao.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDelegacao.Location = New System.Drawing.Point(27, 253)
        Me.btnDelegacao.Name = "btnDelegacao"
        Me.btnDelegacao.Size = New System.Drawing.Size(102, 55)
        Me.btnDelegacao.TabIndex = 0
        Me.btnDelegacao.Text = "REGIONAL (9)"
        Me.btnDelegacao.UseVisualStyleBackColor = True
        '
        'btnTipoPosto
        '
        Me.btnTipoPosto.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnTipoPosto.Location = New System.Drawing.Point(243, 195)
        Me.btnTipoPosto.Name = "btnTipoPosto"
        Me.btnTipoPosto.Size = New System.Drawing.Size(102, 52)
        Me.btnTipoPosto.TabIndex = 0
        Me.btnTipoPosto.Text = "TIPO DE POSTO (8)"
        Me.btnTipoPosto.UseVisualStyleBackColor = True
        '
        'btnEscala
        '
        Me.btnEscala.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnEscala.Location = New System.Drawing.Point(135, 253)
        Me.btnEscala.Name = "btnEscala"
        Me.btnEscala.Size = New System.Drawing.Size(102, 55)
        Me.btnEscala.TabIndex = 0
        Me.btnEscala.Text = "ESCALA (12) - profat não usa mais"
        Me.btnEscala.UseVisualStyleBackColor = True
        '
        'btnSegmento
        '
        Me.btnSegmento.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSegmento.Location = New System.Drawing.Point(243, 253)
        Me.btnSegmento.Name = "btnSegmento"
        Me.btnSegmento.Size = New System.Drawing.Size(102, 55)
        Me.btnSegmento.TabIndex = 0
        Me.btnSegmento.Text = "SEGMENTO/SUBSEGMENTO (1,2)"
        Me.btnSegmento.UseVisualStyleBackColor = True
        '
        'btnLogradouro
        '
        Me.btnLogradouro.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLogradouro.Location = New System.Drawing.Point(27, 196)
        Me.btnLogradouro.Name = "btnLogradouro"
        Me.btnLogradouro.Size = New System.Drawing.Size(102, 51)
        Me.btnLogradouro.TabIndex = 0
        Me.btnLogradouro.Text = "TIPO LOGRADOURO (6)"
        Me.btnLogradouro.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.SystemColors.Highlight
        Me.Button1.Location = New System.Drawing.Point(27, 315)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(102, 47)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Carga dados postos PROFAT"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Yellow
        Me.Button2.Location = New System.Drawing.Point(136, 315)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(101, 47)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Gerar agendamentos DUO - DSV"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Button3
        '
        Me.Button3.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Button3.Location = New System.Drawing.Point(135, 369)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(102, 23)
        Me.Button3.TabIndex = 5
        Me.Button3.Text = "Gerar XMLs"
        Me.Button3.UseVisualStyleBackColor = False
        '
        'btnGerarGrupoTarifario
        '
        Me.btnGerarGrupoTarifario.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnGerarGrupoTarifario.Location = New System.Drawing.Point(244, 315)
        Me.btnGerarGrupoTarifario.Name = "btnGerarGrupoTarifario"
        Me.btnGerarGrupoTarifario.Size = New System.Drawing.Size(101, 47)
        Me.btnGerarGrupoTarifario.TabIndex = 7
        Me.btnGerarGrupoTarifario.Text = "Gerar Grupos Tarifários"
        Me.btnGerarGrupoTarifario.UseVisualStyleBackColor = False
        '
        'frmMigracao
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(585, 412)
        Me.Controls.Add(Me.btnGerarGrupoTarifario)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblTitulo)
        Me.Controls.Add(Me.txtStatus)
        Me.Controls.Add(Me.btnDelegacao)
        Me.Controls.Add(Me.btnFilial)
        Me.Controls.Add(Me.btnTipoPosto)
        Me.Controls.Add(Me.btnDivisao)
        Me.Controls.Add(Me.btnEscala)
        Me.Controls.Add(Me.btnCidade)
        Me.Controls.Add(Me.btnSegmento)
        Me.Controls.Add(Me.btnSubCliente)
        Me.Controls.Add(Me.btnLogradouro)
        Me.Controls.Add(Me.btnCliente)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMigracao"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Migração de dados OM13"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCliente As System.Windows.Forms.Button
    Friend WithEvents txtStatus As System.Windows.Forms.TextBox
    Friend WithEvents lblTitulo As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents btnSubCliente As System.Windows.Forms.Button
    Friend WithEvents btnCidade As System.Windows.Forms.Button
    Friend WithEvents btnDivisao As System.Windows.Forms.Button
    Friend WithEvents btnFilial As System.Windows.Forms.Button
    Friend WithEvents btnDelegacao As System.Windows.Forms.Button
    Friend WithEvents btnTipoPosto As System.Windows.Forms.Button
    Friend WithEvents btnEscala As System.Windows.Forms.Button
    Friend WithEvents btnSegmento As System.Windows.Forms.Button
    Friend WithEvents btnLogradouro As System.Windows.Forms.Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents btnGerarGrupoTarifario As Button
End Class
