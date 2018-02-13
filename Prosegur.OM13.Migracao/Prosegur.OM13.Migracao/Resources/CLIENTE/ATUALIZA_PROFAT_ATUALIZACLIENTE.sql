--ATUALIZA CLIENTE PROFAT
UPDATE FAT_TCADCLI
   SET CODCLICOM = @COD_COMERCIAL,
       datultalt = getdate(),
	   codusu = 1100
WHERE CODCLI = @COD_PROFAT 
  and codclicom is null