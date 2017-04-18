select e.oid_escala, e.cod_escala, e.des_escala, nvl(dp.cod_profat,0) cod_profat
from marte.copr_tescala e
left join marte.vabr_tde_para_geral dp on trim(e.oid_escala) = trim(dp.cod_marte) and dp.cod_param_tab = 12
