SELECT CL.COD_TIPO_CALLE
       ,CL.DES_TIPO_CALLE
       ,CL.DES_CORTA_TIPO_CALLE
       ,V.COD_NOVI
       ,V.COD_PROFAT
FROM MARTE.COPR_TTIPO_CALLE CL
INNER JOIN MARTE.VABR_TDE_PARA_GERAL V ON CL.COD_TIPO_CALLE = V.COD_MARTE
WHERE V.COD_PARAM_TAB = '6'
ORDER BY V.COD_PROFAT