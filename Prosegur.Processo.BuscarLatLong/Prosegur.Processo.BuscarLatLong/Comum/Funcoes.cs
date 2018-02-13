using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
