SELECT DESABRTIPLOG
FROM FAT_TTABTIPLOG
GROUP BY DESABRTIPLOG
HAVING COUNT(DESABRTIPLOG) >1