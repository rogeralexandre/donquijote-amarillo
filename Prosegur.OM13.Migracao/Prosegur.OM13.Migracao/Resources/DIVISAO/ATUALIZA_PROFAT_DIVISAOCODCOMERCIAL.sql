--ATUALIZA O ITEM DE COBRAN�A COM O CODIGO COMERCIAL
UPDATE FAT_TCADITECOB
   SET CODSEGCOM = @CODSEGCOM
WHERE CODITECOB = @CODITECOB