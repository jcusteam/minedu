export const MESSAGES = {
    API: {
        ERROR_SERVICE: 'Ocurrío un error al conectarse al servicio',
        ERROR_SERVER: 'Ocurrío un error interno en el servidor',
        ERROR_SESSION: 'Sesión expirada, por favor vuelva a iniciar',
        ERROR_COMPONENT: 'Error interno en el servidor, no se encuentra el servicio',
        ERROR_LIMIT: 'Se superó el límite de petición de API',
        ERROR_VALIDATION_MODEL: "Se produjeron uno o más errores de validación",
        ERROR_NOT_FOUND: "Ruta no encontrado",
        ERROR_FORBIDDEN: "La solicitud ha sido prohibida por el servidor",
        ERROR: "Se produjo un error al momento de ejecución. Contacte a un administrador para un mejor soporte."
    },
    FORM: {
        CONFIRM_REGISTER: "¿Está seguro de que desea guardar el registro?",
        CONFIRM_UPDATE: "¿Está seguro de que desea actualizar el registro?",
        CONFIRM_ACTIVAR: "¿Está seguro de que desea activar el registro?",
        CONFIRM_INACTIVAR: "¿Está seguro de que desea inactivar el registro?",
        CONFIRM_ELIMINAR: "¿Esta seguro de que desea eliminar el registro?",
        CONFIRM_PROCESAR: "¿Está seguro de que desea procesar el registro?",
        CONFIRM_ANULAR: "¿Está seguro de que desea anular el registro?",
        CLOSE_FORM: "¿Esta seguro de cerrar el registro?",
        ERROR_VALIDATION: "Ingrese todos los campos requeridos.",
        REGISTRO_LINEA: {
            SUCCESS_REGISTER: "La información ha sido registrado correctamente",
            ERROR_CAPTCHA: "Seleccione el CAPTCHA correctamente",
        }
    }
}

export const TYPE_MESSAGE = {
    SUCCESS: "success",
    WARNING: "warning",
    INFO: "info",
    ERROR: "error"
}