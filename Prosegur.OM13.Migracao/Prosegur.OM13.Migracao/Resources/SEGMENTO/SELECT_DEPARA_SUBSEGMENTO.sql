SELECT A.COD_MARTE OID_SUBSEGMENTO, A.COD_PROFAT, S.COD_SUBSEGMENTO, S.OID_SEGMENTO
FROM MARTE.VABR_TDE_PARA_GERAL A INNER JOIN MARTE.COPR_TSUBSEGMENTO S ON A.COD_MARTE = S.OID_SUBSEGMENTO
WHERE A.COD_PARAM_TAB = '2'
{0}