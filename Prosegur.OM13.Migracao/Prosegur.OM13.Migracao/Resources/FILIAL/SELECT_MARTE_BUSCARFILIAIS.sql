SELECT DISTINCT F.COD_FILIAL COMERCIAL, O.ODELE_COD_DELEG NOVI      
FROM MARTE.COPR_TFILIAL F
INNER JOIN MARTE.VABR_TDE_PARA_GERAL V ON F.COD_FILIAL = V.COD_MARTE
INNER JOIN DDLVIG.ODELE O ON O.ODELE_COD_DELEG = V.COD_NOVI
WHERE V.COD_PARAM_TAB IN('4','13')
ORDER BY F.COD_FILIAL