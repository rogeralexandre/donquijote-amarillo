using System;
using Prosegur.Processo.BuscarLatLong.Enum;
using log4net;

namespace Prosegur.Processo.BuscarLatLong.Comum
{
    public static class Log
    {
        #region Atributos

        // Gerar instância da dll para geração dos logs.
        private static readonly ILog _log4Net = LogManager.GetLogger("PROCESSOLOG_ALL");

        #endregion

        #region Métodos

        /// <summary>
        ///     Adiciona um novo log de acordo com o seu tipo.
        /// </summary>
        /// <param name="mensagem">Mensagem do log.</param>
        /// <param name="tipoLog">Tipo de log a ser inserido.</param>
        /// <remarks>
        ///     Os tipos de log possíveis são: Erro, Informação e Aviso.
        ///     
        ///     [Criado 23.12.2016] Bruno Abreu - brx0000510
        /// </remarks>
        public static void AddLog(string mensagem, ETipoLog tipoLog)
        {
            switch (tipoLog)
            {
                case ETipoLog.Erro:
                    _log4Net.Error(mensagem);
                    break;
                case ETipoLog.Info:
                    _log4Net.Info(mensagem);
                    break;
                case ETipoLog.Aviso:
                    _log4Net.Warn(mensagem);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///     Adiciona um novo log de acordo a exceção que levou ao erro.
        /// </summary>
        /// <param name="mensagem">Mensagem do log.</param>
        /// <param name="ex">Exceção que levou ao erro.</param>
        /// <remarks>
        ///     Assumindo que toda exceção é um erro, este método sempre adicionar uma
        ///     nova linha de log do tipo 'Error'. Adicionado além de uma mensagem customizada
        ///     também os dados da exceção lançada.
        ///     
        ///     [Criado 23.12.2016] Bruno Abreu - brx0000510
        /// </remarks>
        public static void AddLog(string mensagem, Exception ex)
        {
            _log4Net.Error(mensagem, ex);
        }

        #endregion

    }
}
