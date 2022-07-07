export enum RoleEnum {
    ROLE_OT_JEFE = "RECAUDA001",
    ROLE_VENTANILLA = "RECAUDA002",
    ROLE_TEC_ADMIN = "RECAUDA003",
    ROLE_REGISTRO_SIAF = "RECAUDA004",
    ROLE_COORDINADOR = "RECAUDA005",
    ROLE_GIRO_PAGO = "RECAUDA006"
}

export enum MenuEnum {
    INICIO = "01010000", // Inicio
    TESORERIA = "01020000", // Tesoreria
    RECIBO_INGRESO = "01020100", // Recibo de ingresos
    DEPOSITO_BANCO = "01020200", // Deposito en cuenta corriente
    REGISTRO_LINEA = "01020300", // Registro en Linea
    LIQUIDACION_INGRESO = "01020400", // Liquidacion de ingreso
    PAPELETA_DEPOSITO = "01020500", // Papeleta de deposito
    COMPROBANTE = "01030000", // Comprobante
    FACTURA = "01030100", // Factura
    BOLETA = "01030200", // Boletas de Ventas
    NOTA_CREDITO = "01030300", // Notas de Credito
    NOTA_DEBITO = "01030400", // Notas de Debito
    RETENCION = "01030500", // Comprobantes de retencion
    ALMACEN = "01040000", // Almacenes
    INGRESO_PECOSA = "01040100", // Ingreso de pecosas
    GUIA_SALIDA = "01040200", // Guia de Salida de Bienes
    KARDEX = "01040300", // Kardex de Almacen 
    SALDO_ALMACEN = "01040400", // Saldos de Almacen
    MAESTRA = "01050000", // Maestras
    TARIFARIO = "01050100", // Tarifario Institucional
    CLASIFICADOR_INGRESO = "01050200", // Clasificador de Ingresos
    CUENTA_CORRIENTE = "01050300", // Cuentas Corrientes
    CATALOGO_BIEN = "01050400", // Catalogo de Bienes
    GRUPO_RECAUDACION = "01050500", // Grupo de Recaudacion
    UNIDA_EJECUTORA = "01050600", // Undades Ejecutoras
    BANCO = "01050700", // Bancos
    CUENTA_CONTABLE = "01050800", // Cuentas Contables
    TIPO_RECIBO_INGRESO = "01050900", // Tipo de Recibo de Ingresos
    FUENTE_FINANCIAMIENTO = "01051000", // Fuentes de Financiamiento
    CLIENTE = "01051100", // Clientes
    UIT = "01051200", // UIT
    UNIDAD_MEDIDA = "01051300", // Unidad de Medida
    TIPO_COMPROBANTE = "01051400", // Tipo de Comprobante de Pago
    TIPO_CAPTACION = "01051500", // Tipo de Captacion
    TIPO_DOC_IDENTIDAD = "01051600", // Tipo de Documento de Identidad
    CONFIGURACION = "01060000", // Configuraciones
    COMPROBANTE_EMISOR = "01060100", // Comprobante Emisor
    TIPO_DOCUMENTO = "01060200", // Tipo de Documentos
    CUADROS_ESTADISTICO = "01070000", // Cuadros estadísticos
}

export enum AccionEnum {
    ACCEDER = "acceder",
    ACTIVAR = "activar",
    AGREGAR = "agregar",
    ANULAR = "anular",
    APROBAR = "aprobar",
    ASIGNAR = "asignar",
    CONFIRMAR = "confirmar",
    CONSULTAR = "consultar",
    DERIVAR = "derivar",
    DESESTIMAR = "desestimar",
    ELIMINAR = "eliminar",
    EXPORTAR = "exportar",
    IMPORTAR = "importar",
    IMPRIMIR = "imprimir",
    INACTIVAR = "inactivar",
    MODIFICAR = "modificar",
    OBSERVAR = "observar",
    REMITIR = "remitir",
  }
  
export enum FileServerEnum {
    COMPROBANTE = "comprobantes",
}

export enum TipoDocEnum {
    FACTURA = 1,
    BOLETA_VENTA = 2,
    NOTA_CREDITO = 3,
    NOTA_DEBITO = 4,
    COMPROBANTE_RETENCION = 5,
    LIGUIDACION_RECAUDACION = 6,
    DEPOSITO_BANCO = 7,
    REGISTRO_LINEA = 8,
    RECIBO_INGRESO = 9,
    PAPELETA_DEPOSITO = 10,
    INGRESO_PECOSA = 11,
    GUIA_SALIDA_BIEN = 12,
}

export enum TipoComprobanteEnum {
    FACTURA = 1,
    BOLETA_VENTA = 2,
    NOTA_CREDITO = 3,
    NOTA_DEBITO = 4,
    COMPROBANTE_RETENCION = 5
}

export enum UnidadEjecturaEnum {
    UE_024 = 1,
    UE_026 = 2,
    UE_116 = 3,
}

export enum TipoDocIdentidadEnum {
    DNI = 1,
    CE = 2,
    RUC = 3,
}
export enum TipoDocIdentidadSunatEnum {
    DNI = "1",
    CE = "4",
    RUC = "6",
}

export enum EstadoEnum {
    ACTIVO = "Activo",
    INACTIVO = "Inactivo",
    ELIMINADO = "Eliminado"
}

export enum PrecioVariableEnum {
    SI = "Si",
    NO = "No"
}

export enum CondicionPagoCodEnum {
    EFECTIVO = "1",
    CREDITO = "2"
}

export enum PeriodoUitEnum {
    ANIO_INICIO = 2010,
}

export enum PeriodoPecosaEnum {
    ANIO_INICIO = 2018,
}

export enum TipoRegimenRetencionEnum {
    TASA_03 = "01",
    TASA_06 = "02",
}

export enum ValorRegimenRetencionEnum {
    TASA_03 = 3.0,
    TASA_06 = 6.0,
}

export enum ValidaDepositoEnum {
    PENDIENTE = "1",
    SI = "2",
    NO = "3"
}

export enum EstadoLiquidacionEnum {
    EMITIDO = 1,
    PROCESADO = 2,
    EMITIDO_RI = 3,
}

export enum EstadoDepositoBancoEnum {
    EMITIDO = 1,
    PROCESADO = 2,
}

export enum EstadoRegistroLineaEnum {
    EMITIDO = 1,
    EN_PROCESO = 2,
    DERIVADO = 3,
    DESESTIMADO = 4,
    OBSERVADO = 5,
    AUTORIZAR = 6,
    EMITIR_RI = 7
}

export enum EstadoReciboIngresoEnum {
    EMITIDO = 1,
    PROCESADO = 2,
    CONFIRMADO = 3,
    ENVIO_SIAF = 4,
    TRANSMITIDO = 5,
    RECHAZADO = 6,
    ANULADO = 7,
    ANULADO_POSTERIOR = 8
}

export enum EstadoPapeletaDepositoEnum {
    EMITIDO = 1,
    PROCESADO = 2
}

export enum EstadoIngresoPecosaEnum {
    EMITIDO = 1,
    PROCESADO = 2,
}

export enum EstadoGuiaSalidaBienEnum {
    EMITIDO = 1,
    PROCESADO = 2,
}

export enum EstadoComprobantePagoEnum {
    EMITIDO = 1,
    ACEPTADO = 2,
    ACEPTADO_OBS = 3,
    RECHAZADA = 4
}

export enum EstadoComprobanteRetencionEnum {
    EMITIDO = 1,
    ACEPTADO = 2,
    ACEPTADO_OBS = 3,
    RECHAZADA = 4
}

export enum TipoAdquisicionEnum {
    SERVICIO = 1,
    BIEN = 2
}

export enum TipoIGVEnum {
    GRAVADA = 10,
    EXONERADA = 20,
    INAFECTA = 30,
}

export enum TipoPrecioVentaEnum {
    PRECIO_UNITARIO = "01",
    VALOR_REFERENCIAL = "02"
}

export enum TipoCaptacionEnum {
    EFECTIVO = 1,
    DEPOSITO_CUENTA_CORRRIENTE = 2,
    CHEQUE = 3,
    VARIOS = 4
}

export enum FuenteOrigenEnum {
    INTERNO = "1",
    EXTERNO = "2"
}

export enum FuenteValidaEnum {
    PENDIENTE = "1",
    SI = "2",
    NO = "3"
}

export enum TipoOperacionEnum {
    VENTA_INTERNA = "01"
}

export enum IGVEnum {
    PORCENTAJE = 18,
    VALOR = 0.18,
}

export enum TipoMonedaEnum {
    NUEVO_SOL = "PEN",
    DOLAR = "USD"
}

export enum CodigoPaisEnum {
    PE = "PE",
}

export enum EstadoDepositoBancoDetalleEnum {
    ACTIVO = "1",
    UTILIZADO = "2",
}

export enum TipoReciboIngresoEnum {
    CAPTACION_VENTANILLA = 1,
    HABILITACION_HURBANA = 2,
    PENALIDAD = 3,
    RETENCION_GARANTIA = 4,
    SANCION_ADMINISTRATIVA = 5,
    EJECUCION_CARTA_FIANAZA = 6,
    DEVOLUCION_SALGO_VIATICO = 7,
    DEPOSITO_INDEBIDO = 8,
    COSTA_PROCESAL = 9,
    SENTENCIA_JUDICIAL = 10,
    RETENCION_IGV = 11
}

export enum RoleRecaudacionEnum {
    ROLE_OT_JEFE = "ROLE_OT_JEFE",
    ROLE_VENTANILLA = "ROLE_VENTANILLA",
    ROLE_TEC_ADMIN = "ROLE_TEC_ADMIN",
    ROLE_REGISTRO_SIAF = "ROLE_REGISTRO_SIAF",
    ROLE_COORDINADOR = "ROLE_COORDINADOR",
    ROLE_GIRO_PAGO = "ROLE_GIRO_PAGO",
}