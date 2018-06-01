using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace Prosegur.Processo.BuscarLatLong.Comum
{
    public static class Funcoes
    {
        public static string MontarTextoEndereco(System.Data.DataRow pLinha)
        {
            //Montar a string de endereço.
            Log.AddLog("Montando string de endereço.", Enum.ETipoLog.Info);

            StringBuilder sbEndereco = new StringBuilder();
            sbEndereco.Append(pLinha["DES_TIPO_CALLE"].ToString() + " ");
            sbEndereco.Append(pLinha["DES_CALLE"].ToString() + " ");
            sbEndereco.Append(pLinha["NUM_NRO"].ToString().Trim() + " ");
            //sbEndereco.Append(pLinha["DES_BARRIO"].ToString() + " ");
            sbEndereco.Append(pLinha["COD_CODIGO_POSTAL"].ToString() + " ");
            sbEndereco.Append(pLinha["DES_CIUDAD"].ToString() + " ");
            sbEndereco.Append(pLinha["COD_ESTADO"].ToString() + " ");
            sbEndereco.Append(pLinha["DES_PAIS"].ToString() + " ");

            Log.AddLog($"Endereço: {sbEndereco.ToString()}", Enum.ETipoLog.Info);

            return sbEndereco.ToString();
        }

        public static void GravarJSON(string pCaminho, ChamadasAPI pChamadas)
        {
            String nomeArquivo;
            nomeArquivo = pCaminho + Constantes.NomeJSONControleExecucoes;

            // serialize JSON to a string and then write string to a file
            //File.WriteAllText(nomeArquivo, JsonConvert.SerializeObject(pChamadas));

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(nomeArquivo))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, pChamadas);
            }
        }
        
        public static ChamadasAPI LerJSON(string pCaminho)
        {
            ChamadasAPI Chamadas;
            String nomeArquivo;
            nomeArquivo = pCaminho + Constantes.NomeJSONControleExecucoes;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(nomeArquivo))
            {
                JsonSerializer serializador = new JsonSerializer();
                Chamadas = (ChamadasAPI)serializador.Deserialize(file, typeof(ChamadasAPI));
            }
            return Chamadas;
        }
    }
}
