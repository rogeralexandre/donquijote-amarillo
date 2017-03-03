﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace adapter.rateio.WsIntegracaoRateioOT {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using tempuri.org;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="RateioOTSoap", Namespace="http://tempuri.org/")]
    public partial class RateioOT : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback IntegracaoRateioOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public RateioOT() {
            this.Url = global::adapter.rateio.Properties.Settings.Default.adapter_rateio_WsIntegracaoRateioOT_RateioOT;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event IntegracaoRateioCompletedEventHandler IntegracaoRateioCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IntegracaoRateio", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Retorno IntegracaoRateio(DadosContratoXmlRateio Rateio) {
            object[] results = this.Invoke("IntegracaoRateio", new object[] {
                        Rateio});
            return ((Retorno)(results[0]));
        }
        
        /// <remarks/>
        public void IntegracaoRateioAsync(DadosContratoXmlRateio Rateio) {
            this.IntegracaoRateioAsync(Rateio, null);
        }
        
        /// <remarks/>
        public void IntegracaoRateioAsync(DadosContratoXmlRateio Rateio, object userState) {
            if ((this.IntegracaoRateioOperationCompleted == null)) {
                this.IntegracaoRateioOperationCompleted = new System.Threading.SendOrPostCallback(this.OnIntegracaoRateioOperationCompleted);
            }
            this.InvokeAsync("IntegracaoRateio", new object[] {
                        Rateio}, this.IntegracaoRateioOperationCompleted, userState);
        }
        
        private void OnIntegracaoRateioOperationCompleted(object arg) {
            if ((this.IntegracaoRateioCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.IntegracaoRateioCompleted(this, new IntegracaoRateioCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class DadosContratoXmlRateio {
        
        private string empresaField;
        
        private string clienteComercialField;
        
        private string objetivoComercialField;
        
        private DadosListaRateio[] listaDoRateioField;
        
        /// <remarks/>
        public string Empresa {
            get {
                return this.empresaField;
            }
            set {
                this.empresaField = value;
            }
        }
        
        /// <remarks/>
        public string ClienteComercial {
            get {
                return this.clienteComercialField;
            }
            set {
                this.clienteComercialField = value;
            }
        }
        
        /// <remarks/>
        public string ObjetivoComercial {
            get {
                return this.objetivoComercialField;
            }
            set {
                this.objetivoComercialField = value;
            }
        }
        
        /// <remarks/>
        public DadosListaRateio[] ListaDoRateio {
            get {
                return this.listaDoRateioField;
            }
            set {
                this.listaDoRateioField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class DadosListaRateio {
        
        private string clienteFaturarField;
        
        private string objetivoFaturarField;
        
        private string clienteCobrancaField;
        
        private string objetivoCobrancaField;
        
        private string percentualField;
        private tempuri.org.DadosListaRateio[] listaDoRateio;
        private Func<tempuri.org.DadosListaRateio[]> toArray;

        public DadosListaRateio()
        {
            // construtor sem parâmetros para atender Exception de que para serializar se deve ter um construtor sem parâmetros.
        }

        public DadosListaRateio(tempuri.org.DadosListaRateio[] listaDoRateio)
        {
            this.listaDoRateio = listaDoRateio;
        }

        public DadosListaRateio(Func<tempuri.org.DadosListaRateio[]> toArray)
        {
            this.toArray = toArray;
        }

        /// <remarks/>
        public string ClienteFaturar {
            get {
                return this.clienteFaturarField;
            }
            set {
                this.clienteFaturarField = value;
            }
        }
        
        /// <remarks/>
        public string ObjetivoFaturar {
            get {
                return this.objetivoFaturarField;
            }
            set {
                this.objetivoFaturarField = value;
            }
        }
        
        /// <remarks/>
        public string ClienteCobranca {
            get {
                return this.clienteCobrancaField;
            }
            set {
                this.clienteCobrancaField = value;
            }
        }
        
        /// <remarks/>
        public string ObjetivoCobranca {
            get {
                return this.objetivoCobrancaField;
            }
            set {
                this.objetivoCobrancaField = value;
            }
        }
        
        /// <remarks/>
        public string Percentual {
            get {
                return this.percentualField;
            }
            set {
                this.percentualField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Mensagens {
        
        private string codigoField;
        
        private string mensagemField;
        
        private string detalheField;
        
        /// <remarks/>
        public string Codigo {
            get {
                return this.codigoField;
            }
            set {
                this.codigoField = value;
            }
        }
        
        /// <remarks/>
        public string Mensagem {
            get {
                return this.mensagemField;
            }
            set {
                this.mensagemField = value;
            }
        }
        
        /// <remarks/>
        public string Detalhe {
            get {
                return this.detalheField;
            }
            set {
                this.detalheField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Retorno {
        
        private System.Nullable<int> codigoRetornoField;
        
        private Mensagens[] lista_RetornoField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> CodigoRetorno {
            get {
                return this.codigoRetornoField;
            }
            set {
                this.codigoRetornoField = value;
            }
        }
        
        /// <remarks/>
        public Mensagens[] Lista_Retorno {
            get {
                return this.lista_RetornoField;
            }
            set {
                this.lista_RetornoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")]
    public delegate void IntegracaoRateioCompletedEventHandler(object sender, IntegracaoRateioCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class IntegracaoRateioCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal IntegracaoRateioCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Retorno Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Retorno)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591