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
                ListadoRateo.Add(new WsIntegracaoRateioOT.DadosListaRateio(request.Body.Rateio.ListaDoRateio));
                                
                GravarLog("Empresa = " + strEmpresa, nomeArquivo);
                GravarLog("ClienteComercial = " + strClienteComercial, nomeArquivo);
                GravarLog("ObjetivoComercial = " + strObjetivoComercial, nomeArquivo);

                // Chamada ao WS do PROFAT.
                WsIntegracaoRateioOT.RateioOT WSProfat = new WsIntegracaoRateioOT.RateioOT();
                // TODO: Fazer a busca no Web.config.
                WSProfat.Url = "http://10.80.48.91/Prosegur.Profat.WS_Versao13/IntegracaoRateioOT.asmx";
                
                // Preparação para o request
                WsRequest.Empresa = strEmpresa;
                WsRequest.ClienteComercial = strClienteComercial;
                WsRequest.ObjetivoComercial = strObjetivoComercial;
                WsRequest.ListaDoRateio = ListadoRateo.ToArray();

                WsResponse = WSProfat.IntegracaoRateio(WsRequest);

                // obtendo o retorno
                GravarLog("Gerando o retorno", nomeArquivo);
                var r = new tempuri.org.Retorno();
                r.Lista_Retorno = new tempuri.org.Mensagens[WsResponse.Lista_Retorno.Length];
                
                r.CodigoRetorno = WsResponse.CodigoRetorno;

                for (int i = 0; i < WsResponse.Lista_Retorno.Length; i++)
                {
                    r.Lista_Retorno[i] = new tempuri.org.Mensagens();
                    r.Lista_Retorno[i].Codigo = WsResponse.Lista_Retorno[i].Codigo;
                    r.Lista_Retorno[i].Detalhe = WsResponse.Lista_Retorno[i].Detalhe;
                    r.Lista_Retorno[i].Mensagem = WsResponse.Lista_Retorno[i].Mensagem;
                }             

                GravarLog("CodigoRetorno = " + r.CodigoRetorno.ToString(), nomeArquivo);

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
