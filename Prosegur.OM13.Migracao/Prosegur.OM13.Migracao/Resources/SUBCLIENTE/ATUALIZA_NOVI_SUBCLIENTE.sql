--ATUALIZA SUB CLIENTE NOVI
UPDATE DDLVIG.OSBCL
   SET --OSBCL_NOC_SUBCLI = :OSBCL_NOC_SUBCLI, 
	   --OSBCL_NOL_SUBCLI = :OSBCL_NOL_SUBCLI, 
       OSBCL_USU_ULTACTU = 'OM13',
	   OSBCL_FEC_ULTACTU = TO_CHAR(SYSDATE, 'YYYYMMDD'),
	   OSBCL_HOR_ULTACTU = TO_CHAR(SYSDATE, 'HH24MISS'), 
	   --OSBCL_COD_VIA = :OSBCL_COD_VIA, 
	   COD_SUBCLI_COMERCIAL = :COD_COMERCIAL
WHERE OSBCL_COD_CLIENTE = :COD_CLIE
  AND OSBCL_COD_SUBCLI = :COD_SUBCLIE
  and cod_subcli_comercial is null