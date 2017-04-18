DECLARE @codesc_n integer = 0
If exists(SELECT codesc FROM FAT_TCADESC WHERE codesc = PAR_CODESC)
 begin
	update FAT_TCADESC
	   set DATULTALT = GETDATE(), 
	       CODUSU = 1100 , 
	       CODMARTE = 'PAR_CODMARTE'
	 where CODESC = PAR_CODESC
	 select 0 as retorno
 end
ELSE
 begin
    SELECT @codesc_n = MAX(CODESC)+1 from FAT_TCADESC
	INSERT INTO FAT_TCADESC 
	            (CODESC   , DESESC , CODUSU, DATULTALT, CODMARTE)
	     VALUES
			    (@codesc_n, 'PAR_DESESC', 1100  , getdate(), 'PAR_CODMARTE')
	SELECT @codesc_n as retorno
 end
