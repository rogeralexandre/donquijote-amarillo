--ATUALIZA SUB CLIENTE PROFAT
UPDATE FAT_TCADPONATE
   SET CODPONATECOM = @COD_COMERCIAL,
	   datultalt = getdate(),
	   codusu = 1100
WHERE CODPONATE = @COD_PROFAT 
  and CODPONATECOM is null