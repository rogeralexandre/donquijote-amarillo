using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingMapsRESTToolkit;
using System.Configuration;

namespace Prosegur.Processo.BuscarLatLong.Comum
{
    public static class Mapas
    {
        public static double[] ObterGeoCoordenadas(string pEndereco)
        {
            double[] GeoCoordenadas;

            var r = ServiceManager.GetResponseAsync(new GeocodeRequest()
            {
                BingMapsKey = ConfigurationManager.AppSettings.Get("BING_MAPS_KEY"),
                Query = pEndereco
            }).GetAwaiter().GetResult();

            if (r != null &&
                r.ResourceSets != null &&
                r.ResourceSets.Length > 0 &&
                r.ResourceSets[0].Resources != null &&
                r.ResourceSets[0].Resources.Length > 0)
            {
                //Pega somente a primeira ocorrência do retorno.
                GeoCoordenadas = (r.ResourceSets[0].Resources[0] as Location).Point.Coordinates;
                Log.AddLog($"Foi encontrada a latitude {GeoCoordenadas[0]} e longitude {GeoCoordenadas[1]} para o endereço {pEndereco}", Enum.ETipoLog.Info);
                return GeoCoordenadas;
            }
            else
            {
                Log.AddLog($"A API não retornou nada para o endereço {pEndereco}", Enum.ETipoLog.Info);
                return new double[0];
            }
        }

    }

    public class ChamadasAPI
    {
        private int vQtdeExcecucoes;

        public DateTime DataBase { get; set; }
        public int QtdeExecucoes
        {
            get
            {
                return vQtdeExcecucoes;
            }

            set
            {
                vQtdeExcecucoes = controlarExecucoes(DataBase, QtdeExecucoes);
            }
        }

        private int controlarExecucoes(DateTime pData, int pQtdeExecucoes)
        {
            if (DateTime.Now.Month > pData.Month)
            {
                // Se virou o mês zera o contador
                // Pendente entender quando que a Microsoft zera os contadores de execução.
                return 0;
            }
            else
            {
                return pQtdeExecucoes;
            }
        }
    }
}
