--ATUALIZA CLIENTE PROFAT
UPDATE FAT_TCADCLI
   SET CODCLICOM = @COD_COMERCIAL
WHERE CODCLI = @COD_PROFAT 