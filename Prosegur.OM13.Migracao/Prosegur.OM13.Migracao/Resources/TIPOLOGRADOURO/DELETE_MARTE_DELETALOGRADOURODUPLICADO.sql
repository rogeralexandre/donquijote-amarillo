DELETE
FROM MARTE.COPR_TTIPO_CALLE
WHERE DES_CORTA_TIPO_CALLE = :DES_CORTA_TIPO_CALLE
  AND TO_NUMBER(COD_TIPO_CALLE) > TO_NUMBER((SELECT MIN(COD_TIPO_CALLE) FROM MARTE.COPR_TTIPO_CALLE WHERE DES_CORTA_TIPO_CALLE = :DES_CORTA_TIPO_CALLE))