SELECT C.COD_IDENTIFICACION_FISCAL, C.COD_CLIENTE, C.DES_RAZON_SOCIAL, COUNT(C.COD_IDENTIFICACION_FISCAL) QTD
FROM MARTE.COPR_TCLIENTE C INNER JOIN MARTE.VABR_TDE_PARA_GERAL V ON C.COD_CLIENTE = V.COD_MARTE     
WHERE C.BOL_POTENCIAL = 0    
  AND V.COD_PARAM_TAB = '10'
GROUP BY C.COD_IDENTIFICACION_FISCAL, C.COD_CLIENTE, C.DES_RAZON_SOCIAL
HAVING COUNT(C.COD_IDENTIFICACION_FISCAL) > 1  