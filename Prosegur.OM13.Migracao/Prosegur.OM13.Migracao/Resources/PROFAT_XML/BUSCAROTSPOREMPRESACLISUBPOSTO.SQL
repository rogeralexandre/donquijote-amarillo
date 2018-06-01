SELECT DISTINCT
       e.cod_empresa,
       c.cod_cliente,
       s.cod_subcliente, 
       ot.num_nro_ot,
       cot.oid_tipo_servicio,
       co.cod_comprobante,
       OT.Fec_Inicio
FROM marte.copr_tcliente c 
inner join marte.copr_tsubcliente s on c.oid_cliente = s.oid_cliente
inner join marte.copr_tot ot on ot.oid_cliente = c.oid_cliente and ot.oid_subcliente = s.oid_subcliente
inner join marte.copr_tpuestosxot pot on ot.oid_ot = pot.oid_ot
inner join marte.copr_tempresa e on e.oid_empresa = ot.oid_empresa
inner join marte.copr_tclasificacion_ot cot on ot.oid_clasificacion_ot = cot.oid_clasificacion_ot
inner join marte.copr_tcomprobante co on ot.oid_comprobante = co.oid_comprobante

