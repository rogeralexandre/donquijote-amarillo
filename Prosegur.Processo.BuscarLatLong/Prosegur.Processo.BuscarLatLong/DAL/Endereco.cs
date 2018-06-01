using System;
using System.Data;
using Prosegur.Processo.BuscarLatLong.Comum;

namespace Prosegur.Processo.BuscarLatLong.DAL
{
    public static class Endereco 
    {
        public static DataTable ObterEnderecosComCoordenadasPendentes(int pLimiteLinhas)
        {
            DataTable dt = new DataTable();
            //String strQuery = "select dom.*, ";
            //strQuery += "cid.des_ciudad, ";
            //strQuery += "est.cod_estado, est.des_estado, ";
            //strQuery += "pais.des_pais, ";
            //strQuery += "tc.des_tipo_calle, ";
            //strQuery += "cp.cod_codigo_postal ";
            //strQuery += "from marte.copr_tdomicilio dom ";
            //strQuery += "inner join marte.copr_tciudad cid on cid.oid_ciudad = dom.oid_ciudad ";
            //strQuery += "left join marte.copr_testado est on est.oid_estado = cid.oid_estado ";
            //strQuery += "left join marte.copr_tpais pais on pais.oid_pais = est.oid_pais ";
            //strQuery += "left join marte.copr_ttipo_calle tc on tc.oid_tipo_calle = dom.oid_tipo_calle ";
            //strQuery += "left join marte.copr_tcodigo_postal cp on cp.oid_codigo_postal = dom.oid_codigo_postal ";
            //strQuery += "where (";
            //// filtra os que tem a latitude e longitude nulos.
            //strQuery += "      ((dom.num_latitud is null or dom.num_longitud is null) ";
            //strQuery += "        and fyh_atualizadoLatLong is null) ";
            //// filtra os que tem a data de atualização posterior a data da última atualização das coordenadas.
            //strQuery += "   or (dom.fyh_mdate > nvl(dom.fyh_atualizadolatlong, dom.fyh_mdate)) ";
            //strQuery += "      )";

            String strQuery = @"select dom.*, 
                                       cid.des_ciudad, 
                                       est.cod_estado, 
                                       est.des_estado, 
                                       pais.des_pais, 
                                       tc.des_tipo_calle, 
                                       cp.cod_codigo_postal
                                  from marte.copr_tdomicilio dom 
                            inner join marte.copr_tciudad cid on cid.oid_ciudad = dom.oid_ciudad 
                             left join marte.copr_testado est on est.oid_estado = cid.oid_estado 
                             left join marte.copr_tpais pais on pais.oid_pais = est.oid_pais
                             left join marte.copr_ttipo_calle tc on tc.oid_tipo_calle = dom.oid_tipo_calle 
                             left join marte.copr_tcodigo_postal cp on cp.oid_codigo_postal = dom.oid_codigo_postal 
                               where (
                                     ((dom.num_latitud is null or dom.num_longitud is null) 
                                       and fyh_atualizadoLatLong is null) 
                                  or (dom.fyh_mdate > nvl(dom.fyh_atualizadolatlong, dom.fyh_mdate)) 
                                     )";

            if (pLimiteLinhas > 0)
            {
                strQuery += "  and rownum <= " + pLimiteLinhas.ToString();
            }
            
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

        public static void AtualizarCoordenadas(string pOIDDomicilio, double[] pCoordenadas)
        {
            int qtdeLinhasAfetadas;

            using (IDbCommand cmd = DbHelper.AcessoDados.CriarComando(Constantes.IdConexaoSistema))
            {
                string strComando = @"UPDATE MARTE.COPR_TDOMICILIO
                                         SET NUM_LATITUD  = :LATITUDE,
                                             NUM_LONGITUD = :LONGITUDE,
                                             FYH_ATUALIZADOLATLONG = SYSDATE 
                                       WHERE OID_DOMICILIO = :OID_DOMICILIO";
                cmd.CommandText = strComando;

                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(Constantes.IdConexaoSistema, "LATITUDE", DbType.Decimal, pCoordenadas[0]));
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(Constantes.IdConexaoSistema, "LONGITUDE", DbType.Decimal, pCoordenadas[1]));
                cmd.Parameters.Add(DbHelper.AcessoDados.CriarParametro(Constantes.IdConexaoSistema, "OID_DOMICILIO", DbType.String, pOIDDomicilio));

                qtdeLinhasAfetadas = DbHelper.AcessoDados.ExecutarNonQuery(Constantes.IdConexaoSistema, cmd);

                Log.AddLog($"O OID_DOMICILIO '{pOIDDomicilio}' foi atualizado com as coordenadas {pCoordenadas[0].ToString()} e {pCoordenadas[1].ToString()}", Enum.ETipoLog.Info);
                Log.AddLog($"Foram atualizadas {qtdeLinhasAfetadas.ToString()} linhas.", Enum.ETipoLog.Info);

            }
        }
    }
}
