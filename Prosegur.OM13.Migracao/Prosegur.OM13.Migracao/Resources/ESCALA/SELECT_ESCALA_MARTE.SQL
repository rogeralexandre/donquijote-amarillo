select e.oid_escala, e.cod_escala, e.des_escala, dp.cod_profat
from marte.copr_tescala e
left join marte.vabr_tde_para_geral dp on e.oid_escala = dp.cod_marte
where dp.cod_param_tab = 12