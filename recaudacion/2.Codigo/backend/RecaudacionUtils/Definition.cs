using System;

namespace RecaudacionUtils
{
    public static class Definition
    {
        // Estato
        public const bool ACTIVO = true;
        public const bool INACTIVO = false;

        // Pagination
        public const int PAGE_NUMBER = 1;
        public const int PAGE_SIZE_5 = 5;
        public const int PAGE_SIZE_10 = 10;
        public const string ASC = "asc";
        public const string DESC = "desc";
        // Operacion
        public const int INSERT = 1;
        public const int UPDATE = 2;

        // Tipo de Mensage
        public const string MESSAGE_TYPE_SUCCESS = "success";
        public const string MESSAGE_TYPE_INFO = "info";
        public const string MESSAGE_TYPE_WARNING = "warning";
        public const string MESSAGE_TYPE_ERROR = "error";

        // Tipo Documento Identidad
        public const int TIPO_DOCUMENTO_IDENTIDAD_DNI = 1;
        public const int TIPO_DOCUMENTO_IDENTIDAD_CE = 2;
        public const int TIPO_DOCUMENTO_IDENTIDAD_RUC = 3;

        // Tipo Documento módulo
        public const int TIPO_DOCUMENTO_FACTURA = 1;
        public const int TIPO_DOCUMENTO_BOLETA = 2;
        public const int TIPO_DOCUMENTO_NOTA_CREDITO = 3;
        public const int TIPO_DOCUMENTO_NOTA_DEBITO = 4;
        public const int TIPO_DOCUMENTO_COMPROBANTE_RETENCION = 5;
        public const int TIPO_DOCUMENTO_LIQUIDACION = 6;
        public const int TIPO_DOCUMENTO_DEPOSITO_BANCO = 7;
        public const int TIPO_DOCUMENTO_REGISTRO_LINEA = 8;
        public const int TIPO_DOCUMENTO_RECIBO_INGRESO = 9;
        public const int TIPO_DOCUMENTO_PAPELETA_DEPOSITO = 10;
        public const int TIPO_DOCUMENTO_INGRESO_PECOSA = 11;
        public const int TIPO_DOCUMENTO_GUIA_SALIDA_BIEN = 12;

        //Estado Tipo Documento Comprobamte
        public const int COMPROBANTE_PAGO_ESTADO_EMITIDO = 1;
        public const int COMPROBANTE_PAGO_ESTADO_ACEPTADO = 2;
        public const int COMPROBANTE_PAGO_ESTADO_ACEPTADO_OBS = 3;
        public const int COMPROBANTE_PAGO_ESTADO_RECHAZADO = 4;

        //Estado Tipo Documento Comprobante Retencion
        public const int COMPROBANTE_RETENCION_ESTADO_EMITIDO = 1;
        public const int COMPROBANTE_RETENCION_ESTADO_ACEPTADO = 2;
        public const int COMPROBANTE_RETENCION_ESTADO_ACEPTADO_OBS = 3;
        public const int COMPROBANTE_RETENCION_ESTADO_RECHAZADO = 4;

        //Estado Tipo Documento Liquidación
        public const int LIQUIDACION_ESTADO_EMITIDO = 1;
        public const int LIQUIDACION_ESTADO_PROCESADO = 2;
        public const int LIQUIDACION_ESTADO_EMITIR_RI = 3;

        //Estado Tipo Documento Depósito Banco
        public const int DEPOSITO_BANCO_ESTADO_EMITIDO = 1;
        public const int DEPOSITO_BANCO_ESTADO_PROCESADO = 2;

        //Estado Tipo Documento Registro Linea
        public const int REGISTRO_LINEA_ESTADO_EMITIDO = 1;
        public const int REGISTRO_LINEA_ESTADO_EN_PROCESO = 2;
        public const int REGISTRO_LINEA_ESTADO_DERIVADO = 3;
        public const int REGISTRO_LINEA_ESTADO_DESESTIMADO = 4;
        public const int REGISTRO_LINEA_ESTADO_OBSERVADO = 5;
        public const int REGISTRO_LINEA_ESTADO_AUTORIZAR = 6;
        public const int REGISTRO_LINEA_ESTADO_EMITIR_RI = 7;

        //Estado Tipo Documento Recibo Ingreso
        public const int RECIBO_INGRESO_ESTADO_EMITIDO = 1;
        public const int RECIBO_INGRESO_ESTADO_PROCESADO = 2;
        public const int RECIBO_INGRESO_ESTADO_CONFIRMADO = 3;
        public const int RECIBO_INGRESO_ESTADO_ENVIADO_SIAF = 4;
        public const int RECIBO_INGRESO_ESTADO_TRANSMITIDO = 5;
        public const int RECIBO_INGRESO_ESTADO_RECHAZADO = 6;
        public const int RECIBO_INGRESO_ESTADO_ANULADO = 7;
        public const int RECIBO_INGRESO_ESTADO_ANULACION_POSTERIOR = 8;

        //Estado Tipo Documento Papeleta Depósito
        public const int PAPELETA_DEPOSITO_ESTADO_EMITIDO = 1;
        public const int PAPELETA_DEPOSITO_ESTADO_PROCESADO = 2;


        //Estado Tipo Documento Ingreso pecosa
        public const int INGRESO_PECOSA_ESTADO_EMITIDO = 1;
        public const int INGRESO_PECOSA_ESTADO_PROCESADO = 2;

        //Estado Tipo Documento Guia Salida Bien
        public const int GUIA_SALIDA_BIEN_ESTADO_EMITIDO = 1;
        public const int GUIA_SALIDA_BIEN_ESTADO_PROCESADO = 2;


        //Estado Depóstio Banco detalle
        public const bool DEPOSITO_BANCO_DETALLE_UTILIZADO_SI = true;
        public const bool DEPOSITO_BANCO_DETALLE_UTILIZADO_NO = false;

        // Tipo Comprobante Pago
        public const int TIPO_COMPROBANTE_FACTURA = 1;
        public const int TIPO_COMPROBANTE_BOLETA = 2;
        public const int TIPO_COMPROBANTE_NOTA_CREDITO = 3;
        public const int TIPO_COMPROBANTE_NOTA_DEBITO = 4;
        public const int TIPO_COMPROBANTE_RETENCION = 5;

        // Tipo Comprobante Pago SUNAT
        public const string SUNAT_TIPO_COMPROBANTE_FACTURA = "01";
        public const string SUNAT_TIPO_COMPROBANTE_BOLETA = "03";
        public const string SUNAT_TIPO_COMPROBANTE_NOTA_CREDITO = "07";
        public const string SUNAT_TIPO_COMPROBANTE_NOTA_DEBITO = "08";
        public const string SUNAT_TIPO_COMPROBANTE_RETENCION = "20";

        // Codigo Pais 
        public const string SUNAT_CODIGO_PAIS = "PE";

        // Codigo Tipo Moneda 
        public const string SUNAT_CODIGO_TIPO_MONEDA_SOL = "PEN";

        // Tipo Documento Identidad SUNAT
        public const string TIPO_DOCUMENTO_SUNAT_DNI = "1";
        public const string TIPO_DOCUMENTO_SUNAT_CARNE_EXT = "4";
        public const string TIPO_DOCUMENTO_SUNAT_RUC = "6";

        // Condición de Pago
        public const string CONDICION_PAGO_EFECTIVO = "EFECTIVO";
        public const string CONDICION_PAGO_CREDITO = "CREDITO";

        public const int TIPO_CONDICION_PAGO_EFECTIVO = 1;
        public const int TIPO_CONDICION_PAGO_CREDITO = 2;

        // Tipo recibo de ingreso
        public const int TIPO_RECIBO_INGRESO_CAPTACION_VENTANILLA = 1;
        public const int TIPO_RECIBO_INGRESO_HABILITACION_HURBANA = 2;
        public const int TIPO_RECIBO_INGRESO_PENALIDAD = 3;
        public const int TIPO_RECIBO_INGRESO_RETENCION_GARANTIA = 4;
        public const int TIPO_RECIBO_INGRESO_SANCION_ADMINISTRATIVA = 5;
        public const int TIPO_RECIBO_INGRESO_EJECUCION_CARTA_FIANAZA = 6;
        public const int TIPO_RECIBO_INGRESO_DEVOLUCION_SALGO_VIATICO = 7;
        public const int TIPO_RECIBO_INGRESO_DEPOSITO_INDEBIDO = 8;
        public const int TIPO_RECIBO_INGRESO_COSTA_PROCESAL = 9;
        public const int TIPO_RECIBO_INGRESO_SENTENCIA_JUDICIAL = 10;
        public const int TIPO_RECIBO_INGRESO_RETENCION_IGV = 11;

        // Tipo de Captación
        public const int TIPO_CAPTACION_EFECTIVO = 1;
        public const int TIPO_CAPTACION_DEPOSITO_CUENTA = 2;
        public const int TIPO_CAPTACION_CHEQUE = 3;
        public const int TIPO_CAPTACION_VARIOS = 4;

        // Fuente Financiamiento
        public const int FUENTE_FINANCIAMIENTO_RECURSO_ORDINARIO = 1;
        public const int FUENTE_FINANCIAMIENTO_RECURSO_DIREC_RECAUDADO = 2;

        // Id Unidad Ejecutora
        public const int ID_UE_024 = 1;
        public const int ID_UE_026 = 2;
        public const int ID_UE_116 = 3;

        // Codigo Unidad Ejecutora
        public const string CODIGO_UE_024 = "024";
        public const string CODIGO_UE_026 = "026";
        public const string CODIGO_UE_116 = "116";

        // Validar Deposito
        public const string VALIDAR_DEPOSITO_PENDIENTE = "1";
        public const string VALIDAR_DEPOSITO_SI = "2";
        public const string VALIDAR_DEPOSITO_NO = "3";

        // Tipo Fuente
        public const int TIPO_FUENTE_FACTURA = 1;
        public const int TIPO_FUENTE_BOLETA = 2;

        // Validar Documento Fuente
        public const string VALIDAR_FUENTE_PENDIENTE = "1";
        public const string VALIDAR_FUENTE_SI = "2";
        public const string VALIDAR_FUENTE_NO = "3";

        // Fuente Origen
        public const string FUENTE_ORIGEN_INTERNO = "1";
        public const string FUENTE_ORIGEN_EXTERNO = "2";

        // Tip Adquicion
        public const int TIPO_ADQUISICION_SERVICIO = 1;
        public const int TIPO_ADQUISICION_BIEN = 2;

        // Tipo IGV
        public const int TIPO_IGV_GRABADA = 10;
        public const int TIPO_IGV_EXONERADA = 20;
        public const int TIPO_IGV_GRATUITA = 21;
        public const int TIPO_IGV_INAFECTA = 30;


        // Tipo IGV
        public const int IGV_PORCETAJE = 18;
        public const decimal IGV_VALOR = 0.18M;

        // Código Tipo Operación
        public const string TIPO_OPERACION_VENTA_INTERNA = "01";
        public const string TIPO_OPERACION_EXPORTACION = "02";

        public const string TIPO_PRECIO_UNITARIO = "01";
        public const string TIPO_PRECIO_REFERENCIAL = "02";

        // Código Tipo Regimen Retencion
        public const string TIPO_REGIMEN_RETENCION_01 = "01";
        public const string TIPO_REGIMEN_RETENCION_02 = "02";
        // Descripcion Tipo Regimen Retencion
        public const string TIPO_REGIMEN_RETENCION_TASA_3 = "Tasa 3%";
        public const string TIPO_REGIMEN_RETENCION_TASA_6 = "Tasa 6%";

        public const decimal TIPO_REGIMEN_RETENCION_TASA_PORCENTAJE_3 = 3;
        public const decimal TIPO_REGIMEN_RETENCION_TASA_PORCENTAJE_6 = 6;

        // UNIDAD MEDIDA
        public const int ID_UNIDAD_MEDIDA_UNIDAD = 1;
        // UNIDAD MEDIDA
        public const int ID_CLASIFICADOR_INGRESO_OTRO_PRODUCTO = 1;

        // Roles
        public const string ROLE_OT_JEFE = "ROLE_OT_JEFE";
        public const string ROLE_VENTANILLA = "ROLE_VENTANILLA";
        public const string ROLE_TEC_ADMIN = "ROLE_TEC_ADMIN";
        public const string ROLE_REGISTRO_SIAF = "ROLE_REGISTRO_SIAF";
        public const string ROLE_COORDINADOR = "ROLE_COORDINADOR";
        public const string ROLE_GIRO_PAGO = "ROLE_GIRO_PAGO";

        // Env OSE
        public const string ENV_PROD = "Prod";
        public const string ENV_DEV = "Dev";

        // Send OSE
        public const string SEND_OSE_NO = "0";
        public const string SEND_OSE_SI = "1";

        // Código QR
        public const string CODE_QR_NO = "0";
        public const string CODE_QR_SI = "1";
    }
}
