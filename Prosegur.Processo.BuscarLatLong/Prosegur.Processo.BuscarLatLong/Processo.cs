using System;
using System.Data;
using Prosegur.Processo.BuscarLatLong.Comum;
using Prosegur.Processo.BuscarLatLong.DAL;


namespace Prosegur.Processo.BuscarLatLong
{
    public class Processo : Prosegur.Duo.HelperProcesso.Processo
    {
        protected override void ExecutarProcesso()
        {
            Log.AddLog("----------------------------------------------- :) #) :o) ------------------------------", Enum.ETipoLog.Info);
            Log.AddLog($"Iniciando processo DUO BuscarLatLong às {DateTime.Now.ToString()}", Enum.ETipoLog.Info);

            try
            {
                //TODO: Armazenar o controle LimiteProcessamento de maneira que possa recuperar depois, porque o limite é mensal.
                //Teria que ser data que bateu o limite e comparar com a constante LimiteProcessamento
                String caminho = this.GetType().Assembly.Location;
                caminho = caminho.Substring(0, caminho.LastIndexOf("\\") + 1);
                Log.AddLog($"Obtendo path do Assembly: {caminho}", Enum.ETipoLog.Info);

                Log.AddLog("Iniciar leitura do JSON.", Enum.ETipoLog.Info);
                ChamadasAPI limiteGeoCodificacao = Funcoes.LerJSON(caminho);

                using (DataTable dt = Endereco.ObterEnderecosComCoordenadasPendentes(Constantes.LimiteProcessamento))
                {
                    string strEndereco;
                    int contador = limiteGeoCodificacao.QtdeExecucoes;
                    //double[] coordenadas = { -23.5108126, -46.7064927 };  // coordenadas da Prosegur Ermano Marchetti.
                    double[] coordenadas;

                    Log.AddLog($"Retornaram {dt.Rows.Count.ToString()} endereços para atualizar as coordenadas.", Enum.ETipoLog.Info);

                    foreach (DataRow dr in dt.Rows)
                    {
                        Log.AddLog($"******** ITERAÇÃO: {contador.ToString()} ********", Enum.ETipoLog.Info);
                        strEndereco = Funcoes.MontarTextoEndereco(dr);
                        Log.AddLog($"Endereço a ser geolocalizado é: {strEndereco}", Enum.ETipoLog.Info);

                        //chamar a API e pegar as coordenadas.
                        Log.AddLog("Iniciar busca da Geolocalização.", Enum.ETipoLog.Info);
                        coordenadas = Mapas.ObterGeoCoordenadas(strEndereco);

                        //atualizar a tabela de domicilios do MARTE
                        Endereco.AtualizarCoordenadas(dr["OID_DOMICILIO"].ToString(), coordenadas);

                        contador++;
                    }

                    if (Constantes.LimiteProcessamento > 0 && contador >= Constantes.LimiteProcessamento)
                    {
                        Log.AddLog($"Atingiu o limite de processamento, parando a execução. {contador.ToString()} repetições foram feitas", Enum.ETipoLog.Info);
                    }
                    else
                    {
                        Log.AddLog($"Foram executadas {contador.ToString()} buscas de geolocalizações.", Enum.ETipoLog.Info);
                    }
                    
                    limiteGeoCodificacao.DataBase = DateTime.Now;
                    limiteGeoCodificacao.QtdeExecucoes = contador;
                    
                    Funcoes.GravarJSON(caminho, limiteGeoCodificacao);
                }
            }
            catch (Exception ex)
            {
                Log.AddLog("Ocorreu um erro na execução do processo DUO BuscarLatLong", Enum.ETipoLog.Erro);
                Log.AddLog($"Detalhes da exceção:", ex);
                throw ex;
            }

            Log.AddLog($"Finalizando processo DUO BuscarLatLong às {DateTime.Now.ToString()}", Enum.ETipoLog.Info);
        }       
    }
}
