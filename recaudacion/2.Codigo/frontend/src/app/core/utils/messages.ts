export const MESSAGES = {
    API: {
        ERR_SERVICE: 'Ocurrío un error al conectarse al servicio',
        ERR_SERVER: 'Ocurrío un error interno en el servidor',
        ERR_SESSION: 'Sesión expirada, por favor vuelva a iniciar',
        ERR_COMPONENT: 'Error interno en el servidor, no se encuentra el servicio',
        ERR_LIMIT: 'Se superó el límite de petición de API',
        ERR_VALIDATION_MODEL: "Se produjeron uno o más errores de validación",
        ERR_NOT_FOUND: "Ruta no encontrado",
        ERR_FORBIDDEN: "La solicitud ha sido prohibida por el servidor",
        ERR: "Se produjo un error al momento de ejecución. Contacte a un administrador para un mejor soporte."
    },
    FORM: {
        CONFIRM_REGISTER: "¿Está seguro de que desea guardar el registro?",
        CONFIRM_UPDATE: "¿Está seguro de que desea actualizar el registro?",
        CONFIRM_ACTIVAR: "¿Está seguro de que desea activar el registro?",
        CONFIRM_INACTIVAR: "¿Está seguro de que desea inactivar el registro?",
        CONFIRM_ELIMINAR: "¿Esta seguro de que desea eliminar el registro?",
        CONFIRM_PROCESAR: "¿Está seguro de que desea procesar el registro?",
        CONFIRM_ANULAR: "¿Está seguro de que desea anular el registro?",
        ERROR_VALIDATION: "Ingrese todos los campos requeridos.",
        CLOSE_FORM: "¿Esta seguro de cerrar el registro?",

        INGRESO_PECOSA: {
            CONFIRM_IMPORT: "¿Está seguro de que desea importar la pecosa?",
        },
        DEPOSITO_BANCO: {
            CONFIRM_ACTIVAR: "¿Está seguro de que desea activar el registro?",
            CONFIRM_INACTIVAR: "¿Está seguro de que desea inactivar el registro?",
            CONFIRM_ELIMINAR: "¿Esta seguro de que desea eliminar el registro?",
            CONFIRM_PROCESAR: "¿Esta seguro de que desea procesar el registro?",
            CONFIRM_ANULAR: "¿Esta seguro de que desea anular el registro?"
        },
        REGISTRO_LINEA: {
            CONFIRM_INICIAR_PROCESO: "¿Está seguro de que desea iniciar el proceso?",
            CONFIRM_DERIVAR: "¿Está seguro de que desea derivar el registro?",
            CONFIRM_DESESTIMAR: "¿Está seguro de que desea desestimar el registro?",
            CONFIRM_OBSERVAR: "¿Está seguro de que desea observar el registro?",
            CONFIRM_AUTORIZAR: "¿Está seguro de que desea autorizar el registro?. Una vez autorizado, el registro pasará a ser Emitido el recibo de ingreso.",
            CONFIRM_TRANSMITIR: "¿Está seguro de que desea transmitir el registro?",
            CONFIRM_EMITIR_RECIBO_INGRESO: "¿Esta seguro de que desea emitir el recibo de ingreso?",
            WARNING_VALIDA_DEPOSTO: "Para derivar el registro, debe validar el depósito",
        },
        RECIBO_INGRESO: {
            CONFIRM_PROCESS: "¿Está seguro de que desea procesar el registro?",
            CONFIRM: "¿Está seguro de que desea confirmar el registro?",
            CONFIRM_RECHAZAR: "¿Está seguro de que desea rechazar el registro?",
            CONFIRM_ENVIO_SIAF: "¿Está seguro de que desea enviar a SIAF el registro?",
            CONFIRM_TRANSMITIR: "¿Está seguro de que desea transmitir el registro?",
            CONFIRM_ANULAR: "¿Está seguro de que desea anular el registro?",
            ONFIRM_ANULAR_POSTERIOR: "¿Está seguro de que desea anular posteriormente el registro?",
        },
        LIQUIDACION: {
            CONFIRM_EMITIR_RECIBO_INGRESO: "¿Esta seguro de que desea emitir el recibo de ingreso?"
        },
        PAPELETA_DEPOSITO: {
            CONFIRM_EMITIR_RECIBO_INGRESO: "¿Esta seguro de que desea emitir el recibo de ingreso?"
        },
        COMPROBANTE_PAGO:{
            GENERAR:"¿Está seguro de que desea generar el comprobante de pago?"
        },
        COMPROBANTE_RETENCION:{
            GENERAR:"¿Está seguro de que desea generar el comprobante de retención?"
        }
    }
}

export const TYPE_MESSAGE = {
    SUCCESS: "success",
    WARNING: "warning",
    INFO: "info",
    ERROR: "error"
}