UPDATE COPR_TDELEGACION
SET COD_DELEGACION = :COD_PROFAT,
	DES_DELEGACION = :DES_PROFAT
WHERE DES_CORTA_DELEGACION = :UF_PROFAT
  AND BOL_ACTIVO = 1