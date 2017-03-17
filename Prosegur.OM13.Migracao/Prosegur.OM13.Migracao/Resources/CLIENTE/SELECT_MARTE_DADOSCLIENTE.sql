--BUSCA INFORMA��ES CLIENTE MARTE
SELECT C.COD_CLIENTE
       ,V.COD_NOVI
       ,V.COD_PROFAT
       ,C.OID_TIPO_ID_FISCAL
       ,C.COD_IDENTIFICACION_FISCAL
       ,C.DES_NOMBRE_FANTASIA
       ,C.DES_RAZON_SOCIAL
       ,C.FEC_INICIO
       ,C.FEC_FIN
       ,D.COD_CEP
       ,D.DES_CALLE
       ,D.NUM_NRO
       ,D.DES_COMPL_DIRECCION
       ,L.COD_TIPO_CALLE
       ,L.DES_TIPO_CALLE
       ,D.DES_BARRIO
       ,CI.DES_CIUDAD
	   ,TRIM(CI.OID_ESTADO) as ESTADO
       ,C.BOL_ACTIVO
FROM MARTE.COPR_TCLIENTE C
INNER JOIN MARTE.VABR_TDE_PARA_GERAL V ON C.COD_CLIENTE = V.COD_MARTE
INNER JOIN DDLVIG.OCLTE O ON O.OCLTE_COD_CLIENTE = V.COD_NOVI
INNER JOIN MARTE.COPR_TDOMICILIOXCLIENTE DC ON C.OID_CLIENTE = DC.OID_CLIENTE
INNER JOIN MARTE.COPR_TDOMICILIO D ON DC.OID_DOMICILIO = D.OID_DOMICILIO
INNER JOIN MARTE.COPR_TTIPO_CALLE L ON D.OID_TIPO_CALLE = L.OID_TIPO_CALLE
INNER JOIN MARTE.COPR_TCIUDAD CI ON D.OID_CIUDAD = CI.OID_CIUDAD
WHERE V.COD_PARAM_TAB = '10'
  AND C.BOL_POTENCIAL = 0
ORDER BY 3 