using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prosegur.EmailHelper;
using Prosegur.Framework.Dicionario;
using Prosegur.Processo.BuscarLatLong.Comum;
using Prosegur.Processo.BuscarLatLong.DAL;

namespace Prosegur.Processo.BuscarLatLong
{
    public class Processo : Prosegur.Duo.HelperProcesso.Processo
    {
        protected override void ExecutarProcesso()
        {
            Log.AddLog("-----------------------------------------------", Enum.ETipoLog.Info);
            Log.AddLog($"Iniciando processo DUO BuscarLatLong às {DateTime.Now.ToString()}", Enum.ETipoLog.Info);

            try
            {
                using (DataTable dt = Endereco.ObterEnderecosComCoordenadasPendentes())
                {
                    string strEndereco;
                    int contador = 0;
                    Log.AddLog($"Retornaram {dt.Rows.Count.ToString()} endereços para atualizar as coordenadas.", Enum.ETipoLog.Info);
                    foreach (DataRow dr in dt.Rows)
                    {
                        strEndereco = Funcoes.MontarTextoEndereco(dr);

                        //chamar a API e pegar as coordenadas.

                        //atualizar a tabela de domicilios do MARTE
                        // usar o OID_DOMICILIO



                        //Abortar se bater o limite da API (Caso a Prosegur não pague pelo uso)
                        contador++;
                        Log.AddLog($"Contador: {contador.ToString()}", Enum.ETipoLog.Info);
                        if (contador >= Constantes.LimiteProcessamento)
                        {
                            Log.AddLog($"Antigiu o limite de processamento, parando a execução. {contador.ToString()} repetições foram feitas", Enum.ETipoLog.Info);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.AddLog("Ocorreu um erro na execução do processo DUO BuscarLatLong", Enum.ETipoLog.Erro);
                Log.AddLog($"Detalhes da exceção: {ex.ToString()}", Enum.ETipoLog.Erro);
                throw ex;
            }

            Log.AddLog($"Finalizando processo DUO BuscarLatLong às {DateTime.Now.ToString()}", Enum.ETipoLog.Info);
        }
    }
}
