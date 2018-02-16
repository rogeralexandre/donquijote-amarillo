select e.cod_empresa_erp,
       c.cod_cliente,
       s.cod_subcliente,
       pot.cod_puesto,
       pot.fec_inicio_servicio,
       pot.hor_inicio_servicio,
       pot.fec_fin_servicio,
       pot.hor_fin_servicio,
       pot.hor_inicio_1,
       pot.hor_fin_1,
       pot.num_horas_dia_vig HorasDiaXVigilante,
       pot.num_horas_almuerzo AlmocoHoras,
       ca.des_cobertura_almuerzo,
       pot.bol_dia_1 segunda,
       pot.bol_dia_2 terça,
       pot.bol_dia_3 quarta,
       pot.bol_dia_4 quinta,
       pot.bol_dia_5 sexta,
       pot.bol_dia_6 sabado,
       pot.bol_dia_7 domingo,
       pot.oid_tipo_jornada,
       tmp1.fec_baixa,
       tmp1.hor_baixa
  from marte.copr_tot ot 
 inner join marte.copr_tcliente    c   on c.oid_cliente = ot.oid_cliente
 inner join marte.copr_tsubcliente s   on s.oid_subcliente = ot.oid_subcliente
 inner join marte.copr_tempresa    e   on e.oid_empresa = ot.oid_empresa
 inner join marte.copr_tpuestosxot pot on pot.oid_ot = ot.oid_ot
 inner join marte.copr_tcobertura_almuerzo ca on ca.oid_cobertura_almuerzo = pot.oid_cobertura_almuerzo
 left join (
           select e1.cod_empresa_erp,
                  c1.cod_cliente,
                  s1.cod_subcliente,
                  pot1.cod_puesto,
                  pot1.fec_inicio_servicio fec_baixa, 
                  pot1.hor_inicio_servicio hor_baixa
             from marte.copr_tot ot1 
             inner join marte.copr_tcliente c1    on c1.oid_cliente = ot1.oid_cliente
             inner join marte.copr_tsubcliente s1 on s1.oid_subcliente = ot1.oid_subcliente
             inner join marte.copr_tempresa e1    on e1.oid_empresa = ot1.oid_empresa
             inner join marte.copr_tpuestosxot pot1 on pot1.oid_ot = ot1.oid_ot
             where pot1.des_condicion = 'B'
           ) tmp1 on  tmp1.cod_empresa_erp = e.cod_empresa_erp
                  and tmp1.cod_cliente     = c.cod_cliente
                  and tmp1.cod_subcliente  = s.cod_subcliente
                  and tmp1.cod_puesto      = pot.cod_puesto
 where pot.des_condicion = 'A'
   and {0}
