using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace adapter.rateio
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RateioAdapter" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RateioAdapter.svc or RateioAdapter.svc.cs at the Solution Explorer and start debugging.
    public class RateioAdapter : RateioOTSoap
    {
        
        public IntegracaoRateioResponse IntegracaoRateio(IntegracaoRateioRequest request) 
        {
            bool alterarCodigoRetorno;
            String strEmpresa;
            String strClienteComercial;
            String strObjetivoComercial;
            List<WsIntegracaoRateioOT.DadosListaRateio> ListadoRateo = new List<WsIntegracaoRateioOT.DadosListaRateio>();
            //List<tempuri.org.Mensagens> ListaMensagens = new List<tempuri.org.Mensagens>();

            String nomeArquivo = "LOG_" + DateTime.Now.ToString().Replace("/", "").Replace(":", "") + ".txt";

            // WEB SERVICE - declarações
            WsIntegracaoRateioOT.DadosContratoXmlRateio WsRequest = new WsIntegracaoRateioOT.DadosContratoXmlRateio();
            WsIntegracaoRateioOT.Retorno WsResponse = new WsIntegracaoRateioOT.Retorno();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            try
            {
                GravarLog("Iniciando serviço", nomeArquivo);
                GravarLog("Lendo request", nomeArquivo);

                strEmpresa = request.Body.Rateio.Empresa;
                strClienteComercial = request.Body.Rateio.ClienteComercial;
                strObjetivoComercial = request.Body.Rateio.ObjetivoComercial;

                GravarLog("Empresa = " + strEmpresa, nomeArquivo);
                GravarLog("ClienteComercial = " + strClienteComercial, nomeArquivo);
                GravarLog("ObjetivoComercial = " + strObjetivoComercial, nomeArquivo);

                GravarLog("Lendo o listado rateio" , nomeArquivo);
                for (int i = 0; i < request.Body.Rateio.ListaDoRateio.Length; i++)
                {
                    ListadoRateo.Add(new WsIntegracaoRateioOT.DadosListaRateio(request.Body.Rateio.ListaDoRateio));
                    ListadoRateo[i].ClienteCobranca = request.Body.Rateio.ListaDoRateio[i].ClienteCobranca;
                    ListadoRateo[i].ObjetivoCobranca = request.Body.Rateio.ListaDoRateio[i].ObjetivoCobranca;
                    ListadoRateo[i].ClienteFaturar = request.Body.Rateio.ListaDoRateio[i].ClienteFaturar;
                    ListadoRateo[i].ObjetivoFaturar = request.Body.Rateio.ListaDoRateio[i].ObjetivoFaturar;
                    ListadoRateo[i].Percentual = request.Body.Rateio.ListaDoRateio[i].Percentual;

                    GravarLog("Listado rateio Item[" + i.ToString() + "]", nomeArquivo);
                    GravarLog("  Cliente Cobranca=" + ListadoRateo[i].ClienteCobranca.ToString() , nomeArquivo);
                    GravarLog("  Objetivo Cobranca=" + ListadoRateo[i].ObjetivoCobranca.ToString(), nomeArquivo);
                    GravarLog("  Cliente Faturar=" + ListadoRateo[i].ClienteFaturar.ToString(), nomeArquivo);
                    GravarLog("  Objetivo Faturar=" + ListadoRateo[i].ObjetivoFaturar.ToString(), nomeArquivo);
                    GravarLog("  Percentual=" + ListadoRateo[i].Percentual.ToString(), nomeArquivo);
                }

                // Chamada ao WS do PROFAT.
                //WsIntegracaoRateioOT.RateioOT WSProfat = new WsIntegracaoRateioOT.RateioOT();
                using (var WSProfat = new WsIntegracaoRateioOT.RateioOT())
                {
                    // TODO: Fazer a busca no Web.config.
                    //WSProfat.Url = "http://10.80.48.91/Prosegur.Profat.WS_Versao13/IntegracaoRateioOT.asmx";
                    GravarLog("Preparando chamada ao WSPROFAT localizado em :" + WSProfat.Url, nomeArquivo);

                    // Preparação para o request
                    WsRequest.Empresa = strEmpresa;
                    WsRequest.ClienteComercial = strClienteComercial;
                    WsRequest.ObjetivoComercial = strObjetivoComercial;
                    WsRequest.ListaDoRateio = ListadoRateo.ToArray();
                    
                    GravarLog("Executando WSProfat", nomeArquivo);
                    WsResponse = WSProfat.IntegracaoRateio(WsRequest);
                    GravarLog("WSProfat foi executado com sucesso!", nomeArquivo);
                }
                    
                // obtendo o retorno
                GravarLog("Gerando o retorno", nomeArquivo);
                var r = new tempuri.org.Retorno();
                r.Lista_Retorno = new tempuri.org.Mensagens[WsResponse.Lista_Retorno.Length];
                
                r.CodigoRetorno = WsResponse.CodigoRetorno;
                GravarLog("CodigoRetorno recebido do PROFAT = " + r.CodigoRetorno.ToString(), nomeArquivo);
                alterarCodigoRetorno = false;

                for (int i = 0; i < WsResponse.Lista_Retorno.Length; i++)
                {
                    r.Lista_Retorno[i] = new tempuri.org.Mensagens();
                    r.Lista_Retorno[i].Codigo = WsResponse.Lista_Retorno[i].Codigo;
                    r.Lista_Retorno[i].Detalhe = WsResponse.Lista_Retorno[i].Detalhe;
                    r.Lista_Retorno[i].Mensagem = WsResponse.Lista_Retorno[i].Mensagem;

                    if (r.Lista_Retorno[i].Codigo != null)
                    {
                        GravarLog("Código da mensagem do retorno linha-" + i.ToString() + " = " + r.Lista_Retorno[i].Codigo.ToString(), nomeArquivo);
                        if (r.Lista_Retorno[i].Codigo == "ID008" || r.Lista_Retorno[i].Codigo == "ID009" || r.Lista_Retorno[i].Codigo == "ID011" || r.Lista_Retorno[i].Codigo == "ID012" || r.Lista_Retorno[i].Codigo == "ID013" || r.Lista_Retorno[i].Codigo == "ID014") 
                        {
                            alterarCodigoRetorno = true;
                        }
                    }

                    if (r.Lista_Retorno[i].Detalhe != null)
                    {
                        GravarLog("Detalhe da mensagem do retorno linha-" + i.ToString() + " = " + r.Lista_Retorno[i].Detalhe.ToString(), nomeArquivo);
                    }

                    if (r.Lista_Retorno[i].Mensagem != null)
                    {
                        GravarLog("Mensagem da mensagem do retorno linha-" + i.ToString() + " = " + r.Lista_Retorno[i].Mensagem.ToString(), nomeArquivo);
                    }
                    
                }

                if (alterarCodigoRetorno)
                {
                    GravarLog("Alterando o código retornado pelo PROFAT", nomeArquivo);
                    // alterando o código para OK/SUCESSO.
                    r.CodigoRetorno = 1;
                    GravarLog("CodigoRetorno alterado para " + r.CodigoRetorno.ToString(), nomeArquivo);
                }
                
                // Retornos do WS
                var retornoCorpo = new IntegracaoRateioResponseBody(r);
                var retorno = new IntegracaoRateioResponse(retornoCorpo);
                                
                GravarLog("Finalizando o serviço.", nomeArquivo);
                return retorno;
            }
            catch (Exception ex)
            {
                GravarLog("Ocorreu um erro:", nomeArquivo);
                GravarLog("Mensagem de erro:" + ex.Message.ToString(), nomeArquivo);
                throw;
            }


        }

        private void GravarLog(String pMensagem, String pNomeArquivo)
        {
            //String nomeCaminho = System.IO.Directory.GetCurrentDirectory().ToString() + "\\Log";
            String nomeCaminho =  "C:\\Log";
            String destinoArquivo;

            if (!System.IO.Directory.Exists(nomeCaminho))
            {
                System.IO.Directory.CreateDirectory(nomeCaminho);
            }

            destinoArquivo = System.IO.Path.Combine(nomeCaminho, pNomeArquivo);

            using (System.IO.StreamWriter str = new System.IO.StreamWriter(destinoArquivo, true))
            {
                str.WriteLine(DateTime.Now.ToString() + " - " + pMensagem);
                str.Close();
            }

        }
    

    }
}
