DELETE FROM DBO.FAT_TCADCLI 
WHERE CPF = :CPF
  AND DATULTALT < (SELECT MAX(DATULTALT) FROM DBO.FAT_TCADCLI WHERE CPF = :CPF)