﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://tempuri.org/", ClrNamespace="tempuri.org")]

namespace tempuri.org
{
    using System.Runtime.Serialization;
    using adapter.rateio.WsIntegracaoRateioOT;

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DadosContratoXmlRateio", Namespace="http://tempuri.org/")]
    public partial class DadosContratoXmlRateio : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string EmpresaField;
        
        private string ClienteComercialField;
        
        private string ObjetivoComercialField;
        
        private tempuri.org.DadosListaRateio[] ListaDoRateioField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string Empresa
        {
            get
            {
                return this.EmpresaField;
            }
            set
            {
                this.EmpresaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string ClienteComercial
        {
            get
            {
                return this.ClienteComercialField;
            }
            set
            {
                this.ClienteComercialField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string ObjetivoComercial
        {
            get
            {
                return this.ObjetivoComercialField;
            }
            set
            {
                this.ObjetivoComercialField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public tempuri.org.DadosListaRateio[] ListaDoRateio
        {
            get
            {
                return this.ListaDoRateioField;
            }
            set
            {
                this.ListaDoRateioField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DadosListaRateio", Namespace="http://tempuri.org/")]
    public partial class DadosListaRateio : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string ClienteFaturarField;
        
        private string ObjetivoFaturarField;
        
        private string ClienteCobrancaField;
        
        private string ObjetivoCobrancaField;
        
        private string PercentualField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string ClienteFaturar
        {
            get
            {
                return this.ClienteFaturarField;
            }
            set
            {
                this.ClienteFaturarField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string ObjetivoFaturar
        {
            get
            {
                return this.ObjetivoFaturarField;
            }
            set
            {
                this.ObjetivoFaturarField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string ClienteCobranca
        {
            get
            {
                return this.ClienteCobrancaField;
            }
            set
            {
                this.ClienteCobrancaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string ObjetivoCobranca
        {
            get
            {
                return this.ObjetivoCobrancaField;
            }
            set
            {
                this.ObjetivoCobrancaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string Percentual
        {
            get
            {
                return this.PercentualField;
            }
            set
            {
                this.PercentualField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Retorno", Namespace="http://tempuri.org/")]
    public partial class Retorno : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private System.Nullable<int> CodigoRetornoField;
        
        private tempuri.org.Mensagens[] Lista_RetornoField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Nullable<int> CodigoRetorno
        {
            get
            {
                return this.CodigoRetornoField;
            }
            set
            {
                this.CodigoRetornoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public tempuri.org.Mensagens[] Lista_Retorno
        {
            get
            {
                return this.Lista_RetornoField;
            }
            set
            {
                this.Lista_RetornoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Mensagens", Namespace="http://tempuri.org/")]
    public partial class Mensagens : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string CodigoField;
        
        private string MensagemField;
        
        private string DetalheField;
        private adapter.rateio.WsIntegracaoRateioOT.Mensagens[] lista_Retorno;

        //public Mensagens(adapter.rateio.WsIntegracaoRateioOT.Mensagens[] lista_Retorno)
        //{
        //    this.lista_Retorno = lista_Retorno;
        //}

        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string Codigo
        {
            get
            {
                return this.CodigoField;
            }
            set
            {
                this.CodigoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string Mensagem
        {
            get
            {
                return this.MensagemField;
            }
            set
            {
                this.MensagemField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string Detalhe
        {
            get
            {
                return this.DetalheField;
            }
            set
            {
                this.DetalheField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="http://prosegur.com.br/adapter/marte/rateio/v1",
                                              ConfigurationName="RateioOTSoap")]
public interface RateioOTSoap
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IntegracaoRateio", ReplyAction="*")]
    IntegracaoRateioResponse IntegracaoRateio(IntegracaoRateioRequest request);
    
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class IntegracaoRateioRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="IntegracaoRateio", Namespace="http://tempuri.org/", Order=0)]
    public IntegracaoRateioRequestBody Body;
    
    public IntegracaoRateioRequest()
    {
    }
    
    public IntegracaoRateioRequest(IntegracaoRateioRequestBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class IntegracaoRateioRequestBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public tempuri.org.DadosContratoXmlRateio Rateio;
    
    public IntegracaoRateioRequestBody()
    {
    }
    
    public IntegracaoRateioRequestBody(tempuri.org.DadosContratoXmlRateio Rateio)
    {
        this.Rateio = Rateio;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class IntegracaoRateioResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="IntegracaoRateioResponse", Namespace="http://tempuri.org/", Order=0)]
    public IntegracaoRateioResponseBody Body;
    
    public IntegracaoRateioResponse()
    {
    }
    
    public IntegracaoRateioResponse(IntegracaoRateioResponseBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class IntegracaoRateioResponseBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public tempuri.org.Retorno IntegracaoRateioResult;
    
    public IntegracaoRateioResponseBody()
    {
    }
    
    public IntegracaoRateioResponseBody(tempuri.org.Retorno IntegracaoRateioResult)
    {
        this.IntegracaoRateioResult = IntegracaoRateioResult;
    }
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface RateioOTSoapChannel : RateioOTSoap, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class RateioOTSoapClient : System.ServiceModel.ClientBase<RateioOTSoap>, RateioOTSoap
{
    
    public RateioOTSoapClient()
    {
    }
    
    public RateioOTSoapClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public RateioOTSoapClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public RateioOTSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public RateioOTSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public IntegracaoRateioResponse IntegracaoRateio(IntegracaoRateioRequest request)
    {
        return base.Channel.IntegracaoRateio(request);
    }
    
}
