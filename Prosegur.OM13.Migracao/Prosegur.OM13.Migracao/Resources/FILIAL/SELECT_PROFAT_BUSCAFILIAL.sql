SELECT CODFIL, NOMFIL, CNP, CODEMP
FROM FAT_TCADFIL
where CODEMP IN (1,2)
  AND CODFIL = @CODFIL
ORDER BY 1