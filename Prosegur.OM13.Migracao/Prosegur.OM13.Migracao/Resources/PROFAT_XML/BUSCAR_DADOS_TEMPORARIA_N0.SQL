SELECT DISTINCT
       regexp_substr(T.P_OIDPUEMAR, '[^|]+|[^|]+',1,1) AS P_CODEMPRESA,
       regexp_substr(T.P_OIDPUEMAR, '[^|]+|[^|]+',2,2) AS P_CODCLIENTE,
       regexp_substr(T.P_OIDPUEMAR, '[^|]+|[^|]+',3,3) AS P_CODSUBCLI
  FROM MARTE.TMP_CARGAENVIOOTS T
 ORDER BY P_CODEMPRESA, P_CODCLIENTE, P_CODSUBCLI
