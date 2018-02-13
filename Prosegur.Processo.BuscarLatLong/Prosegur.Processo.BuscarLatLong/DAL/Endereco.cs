using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Prosegur.Processo.BuscarLatLong.Comum;

namespace Prosegur.Processo.BuscarLatLong.DAL
{
    public static class Endereco 
    {
        public static DataTable ObterEnderecosComCoordenadasPendentes()
        {
            DataTable dt = new DataTable();
            String strQuery = "select dom.*, ";
            strQuery += "cid.des_ciudad, ";
            strQuery += "est.cod_estado, est.des_estado, ";
            strQuery += "pais.des_pais, ";
            strQuery += "tc.des_tipo_calle, ";
            strQuery += "cp.cod_codigo_postal ";
            strQuery += "from marte.copr_tdomicilio dom ";
            strQuery += "inner join marte.copr_tciudad cid on cid.oid_ciudad = dom.oid_ciudad ";
            strQuery += "left join marte.copr_testado est on est.oid_estado = cid.oid_estado ";
            strQuery += "left join marte.copr_tpais pais on pais.oid_pais = est.oid_pais ";
            strQuery += "left join marte.copr_ttipo_calle tc on tc.oid_tipo_calle = dom.oid_tipo_calle ";
            strQuery += "left join marte.copr_tcodigo_postal cp on cp.oid_codigo_postal = dom.oid_codigo_postal ";
            strQuery += "where((dom.num_latitud is null or dom.num_longitud is null) ";
            strQuery += "and fyh_atualizadoLatLong is null) ";
            strQuery += "or(dom.fyh_mdate >= nvl(dom.fyh_atualizadolatlong, dom.fyh_mdate)) ";

            var cmd = DbHelper.AcessoDados.CriarComando(Constantes.IdConexaoSistema);
            cmd.CommandText = strQuery;

            try
            {
                Log.AddLog("Buscando endereços com atualização de latitude e longitude pendentes.", Enum.ETipoLog.Info);
                dt = DbHelper.AcessoDados.ExecutarDataTable(Constantes.IdConexaoSistema,cmd);
            }
            catch (Exception ex)
            {
                Log.AddLog($"Ocorreu um erro ao tentar executar a query {strQuery}", Enum.ETipoLog.Erro);
                Log.AddLog($"Detalhes da exceção: {ex.ToString()}", Enum.ETipoLog.Erro);
                throw ex;
            }

            // caso não tenha retornado nenhuma linha, retorna uma nova instância de datatable
            return dt ?? new DataTable();
        }
    }
}
