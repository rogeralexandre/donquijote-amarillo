SELECT CNP, COUNT(CNP) QTD
FROM DBO.FAT_TCADCLI 
GROUP BY CNP
HAVING COUNT(CNP) > 1