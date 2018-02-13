using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Prosegur.Processo.BuscarLatLong.Comum
{
    public static class Constantes
    {
        #region AppConfig

        /// <summary>
        ///     Retorna o id da conexão com a base de dados do sistema.
        /// </summary>
        /// <returns>String com o id da conexão.</returns>
        /// <remarks>
        ///     O id de conexão com o sistema é utilizado para recuperar os dados de
        ///     conexão existente dentro do arquivo conexoes.dbc.
        ///     
        ///     [Criado 23.12.2016] Bruno Abreu - brx0000510
        /// </remarks>
        public static string IdConexaoSistema
        {
            get
            {
                var idConexaoSistema = ConfigurationManager.AppSettings["ID_CONEXAO_SISTEMA"];
                if (string.IsNullOrEmpty(idConexaoSistema))
                    return string.Empty;

                return idConexaoSistema;
            }
        }

        public static int LimiteProcessamento
        {
            get
            {
                int intLimiteProcessamento;
                var strLimiteProcessamento = ConfigurationManager.AppSettings["LIMITE_PROCESSAR"];
                if (int.TryParse(strLimiteProcessamento, out intLimiteProcessamento))
                {
                    return intLimiteProcessamento;
                }
                else
                {
                    //coloquei como padrão fazer uma vez só.
                    return 1;
                }
            }
        }
        #endregion
    }
}
