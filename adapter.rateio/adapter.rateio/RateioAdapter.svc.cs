using System;
using System.Collections.Generic;
using System.Globalization;
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
            string strEmpresa;
            string strClienteComercial;
            string strObjetivoComercial;
            List<WsIntegracaoRateioOT.DadosListaRateio> ListadoRateo = new List<WsIntegracaoRateioOT.DadosListaRateio>();
            //List<tempuri.org.Mensagens> ListaMensagens = new List<tempuri.org.Mensagens>();

            string nomeCaminho = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            string nomeArquivo = "LOG_" + DateTime.Now.ToString().Replace("/", "").Replace(":", "") + ".txt";

            // WEB SERVICE - declarações
            WsIntegracaoRateioOT.DadosContratoXmlRateio WsRequest = new WsIntegracaoRateioOT.DadosContratoXmlRateio();
            WsIntegracaoRateioOT.Retorno WsResponse = new WsIntegracaoRateioOT.Retorno();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            try
            {
                GravarLog("Iniciando serviço", nomeArquivo, nomeCaminho);
                GravarLog("Lendo request", nomeArquivo, nomeCaminho);

                strEmpresa = request.Body.Rateio.Empresa;
                strClienteComercial = request.Body.Rateio.ClienteComercial;
                strObjetivoComercial = request.Body.Rateio.ObjetivoComercial;

                GravarLog("Empresa = " + strEmpresa, nomeArquivo, nomeCaminho);
                GravarLog("ClienteComercial = " + strClienteComercial, nomeArquivo, nomeCaminho);
                GravarLog("ObjetivoComercial = " + strObjetivoComercial, nomeArquivo, nomeCaminho);

                GravarLog("Lendo o listado rateio" , nomeArquivo, nomeCaminho);
                for (int i = 0; i < request.Body.Rateio.ListaDoRateio.Length; i++)
                {
                    ListadoRateo.Add(new WsIntegracaoRateioOT.DadosListaRateio(request.Body.Rateio.ListaDoRateio));
                    ListadoRateo[i].ClienteCobranca = request.Body.Rateio.ListaDoRateio[i].ClienteCobranca;
                    ListadoRateo[i].ObjetivoCobranca = request.Body.Rateio.ListaDoRateio[i].ObjetivoCobranca;
                    ListadoRateo[i].ClienteFaturar = request.Body.Rateio.ListaDoRateio[i].ClienteFaturar;
                    ListadoRateo[i].ObjetivoFaturar = request.Body.Rateio.ListaDoRateio[i].ObjetivoFaturar;
                    ListadoRateo[i].Percentual = request.Body.Rateio.ListaDoRateio[i].Percentual.Replace(",",".");

                    GravarLog("Listado rateio Item[" + i.ToString() + "]", nomeArquivo, nomeCaminho);
                    GravarLog("  Cliente Cobranca=" + ListadoRateo[i].ClienteCobranca.ToString() , nomeArquivo, nomeCaminho);
                    GravarLog("  Objetivo Cobranca=" + ListadoRateo[i].ObjetivoCobranca.ToString(), nomeArquivo, nomeCaminho);
                    GravarLog("  Cliente Faturar=" + ListadoRateo[i].ClienteFaturar.ToString(), nomeArquivo, nomeCaminho);
                    GravarLog("  Objetivo Faturar=" + ListadoRateo[i].ObjetivoFaturar.ToString(), nomeArquivo, nomeCaminho);
                    GravarLog("  Percentual=" + ListadoRateo[i].Percentual.ToString(), nomeArquivo, nomeCaminho);
                }

                // Chamada ao WS do PROFAT.
                //WsIntegracaoRateioOT.RateioOT WSProfat = new WsIntegracaoRateioOT.RateioOT();
                using (var WSProfat = new WsIntegracaoRateioOT.RateioOT())
                {
                    
                    // TODO: Fazer a busca no Web.config.
                    //WSProfat.Url = "http://10.80.48.91/Prosegur.Profat.WS_Versao13/IntegracaoRateioOT.asmx";
                    GravarLog("Preparando chamada ao WSPROFAT localizado em :" + WSProfat.Url, nomeArquivo, nomeCaminho);

                    // Preparação para o request
                    WsRequest.Empresa = strEmpresa;
                    WsRequest.ClienteComercial = strClienteComercial;
                    WsRequest.ObjetivoComercial = strObjetivoComercial;
                    WsRequest.ListaDoRateio = ListadoRateo.ToArray();
                    
                    GravarLog("Executando WSProfat", nomeArquivo, nomeCaminho);
                    WsResponse = WSProfat.IntegracaoRateio(WsRequest);
                    GravarLog("WSProfat foi executado com sucesso!", nomeArquivo, nomeCaminho);
                }
                    
                // obtendo o retorno
                GravarLog("Gerando o retorno", nomeArquivo, nomeCaminho);
                var r = new tempuri.org.Retorno();
                r.Lista_Retorno = new tempuri.org.Mensagens[WsResponse.Lista_Retorno.Length];
                
                r.CodigoRetorno = WsResponse.CodigoRetorno;
                GravarLog("CodigoRetorno recebido do PROFAT = " + r.CodigoRetorno.ToString(), nomeArquivo, nomeCaminho);
                alterarCodigoRetorno = false;

                for (int i = 0; i < WsResponse.Lista_Retorno.Length; i++)
                {
                    r.Lista_Retorno[i] = new tempuri.org.Mensagens();
                    r.Lista_Retorno[i].Codigo = WsResponse.Lista_Retorno[i].Codigo;
                    r.Lista_Retorno[i].Detalhe = WsResponse.Lista_Retorno[i].Detalhe;
                    r.Lista_Retorno[i].Mensagem = WsResponse.Lista_Retorno[i].Mensagem;

                    if (r.Lista_Retorno[i].Codigo != null)
                    {
                        GravarLog("Código da mensagem do retorno linha-" + i.ToString() + " = " + r.Lista_Retorno[i].Codigo.ToString(), nomeArquivo, nomeCaminho);
                        if (r.Lista_Retorno[i].Codigo == "ID008" || r.Lista_Retorno[i].Codigo == "ID009" || r.Lista_Retorno[i].Codigo == "ID011" || r.Lista_Retorno[i].Codigo == "ID012" || r.Lista_Retorno[i].Codigo == "ID013" || r.Lista_Retorno[i].Codigo == "ID014") 
                        {
                            alterarCodigoRetorno = true;
                        }
                    }

                    if (r.Lista_Retorno[i].Detalhe != null)
                    {
                        GravarLog("Detalhe da mensagem do retorno linha-" + i.ToString() + " = " + r.Lista_Retorno[i].Detalhe.ToString(), nomeArquivo, nomeCaminho);
                    }

                    if (r.Lista_Retorno[i].Mensagem != null)
                    {
                        GravarLog("Mensagem da mensagem do retorno linha-" + i.ToString() + " = " + r.Lista_Retorno[i].Mensagem.ToString(), nomeArquivo, nomeCaminho);
                    }
                    
                }

                if (alterarCodigoRetorno)
                {
                    GravarLog("Alterando o código retornado pelo PROFAT", nomeArquivo, nomeCaminho);
                    // alterando o código para OK/SUCESSO.
                    r.CodigoRetorno = 1;
                    GravarLog("CodigoRetorno alterado para " + r.CodigoRetorno.ToString(), nomeArquivo, nomeCaminho);
                }
                
                // Retornos do WS
                var retornoCorpo = new IntegracaoRateioResponseBody(r);
                var retorno = new IntegracaoRateioResponse(retornoCorpo);
                                
                GravarLog("Finalizando o serviço.", nomeArquivo, nomeCaminho);
                return retorno;
            }
            catch (Exception ex)
            {
                //GravarLog("Ocorreu um erro:", nomeArquivo, nomeCaminho);
                GravarLog("Mensagem de erro:" + ex.Message.ToString(), nomeArquivo, nomeCaminho);
                GravarLog("StackTrace: " + ex.StackTrace.ToString(), nomeArquivo, nomeCaminho);
                throw;
            }


        }

        private void GravarLog(String pMensagem, String pNomeArquivo, String nomeCaminho)
        {

            //String nomeCaminho =  "C:\\Log";
            nomeCaminho = nomeCaminho + "Log\\";
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
