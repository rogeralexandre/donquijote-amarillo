﻿update marte.BRINT_COMPLETAR_DADOS
   set M_CODUNICOPOSTO = :pCODUNICOPOSTO,
       M_FECHAINICIO   = :pFECHAINICIO,
       M_HORAFECHAINICIO = :pHORAFECHAINICIO,
       M_FECHAFIN = :pFECHAFIN,
       M_HORAFECHAFIN = :pHORAFECHAFIN,
       M_DIASTRABAJO = :pDIASTRABAJO,
       M_TIPODIA = :pTIPODIA,
       M_HORARIOS = :pHORARIOS,
       M_NUMEROHORAS = :pNUMEROHORAS,
       M_INTERVALOALMUERZO = :pINTERVALOALMUERZO,
       M_TRABAJAALMUERZO = :pTRABAJAALMUERZO,
	   M_DATAEXPORTACAO = sysdate,
	   M_QtdePostos = :pQtdePostos,
	   M_Cantidad = :pQtdePostosMARTE
 where oidpuemar = :pOIDPUEMAR
