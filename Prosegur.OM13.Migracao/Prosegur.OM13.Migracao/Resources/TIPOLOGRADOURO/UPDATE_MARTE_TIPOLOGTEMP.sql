UPDATE MARTE.COPR_TTIPO_CALLE 
   SET COD_TIPO_CALLE = COD_TIPO_CALLE || '_'
WHERE  COD_TIPO_CALLE IN (SELECT COD_MARTE FROM MARTE.VABR_TDE_PARA_GERAL V WHERE V.COD_PARAM_TAB = '6')