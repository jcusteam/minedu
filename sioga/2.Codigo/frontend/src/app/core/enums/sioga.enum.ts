export enum TipoDocEnum {
    REGISTRO_LINEA = 8,
}

export enum TipoDocIdentidadEnum {
    DNI = 1,
    CE = 2,
    RUC = 3,
}

export enum LabelTipDocIdentidadEnum {
    NOMBRE_APELLIDO = "Nombres y Apellidos",
    RAZON_SOCIAL = "Razón Social",
}

export enum EstadoRegistroLineaEnum {
    EMITIDO = 1,
    EN_PROCES0 = 2,
    DERIVADO = 3,
    DESESTIMADO = 4,
    OBSERVADO = 5
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

export enum ValidaDepositoEnum {
    PENDIENTE = "1",
    SI = "2",
    NO = "3"
}
