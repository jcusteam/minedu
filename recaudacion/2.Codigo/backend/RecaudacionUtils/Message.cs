using System;

namespace RecaudacionUtils
{
    public static class Message
    {

        public const string SUCCESS_INSERT = "La información ha sido guardada correctamente";
        public const string SUCCESS_UPDATE = "El registro ha sido actualizado correctamente";
        public const string SUCCESS_DELETE = "El registro ha sido eliminado correctamente";
        public const string SUCCESS_UPLOAD = "El archivo se ha subido correctamente";
        public const string SUCCESS_DOWNLOAD = "El archivo se ha descargado correctamente";
        public const string SUCCESS_UPDATE_ACTIVO = "El registro ha sido activado correctamente";
        public const string SUCCESS_UPDATE_INACTIVO = "El registro ha sido inactivado correctamente";
        public const string INFO_EXISTS_DATA = "Ya se encuentra registrado con los datos ingresados";
        public const string INFO_NOT_EXISTS_DATA = "No se ha podido obtener el registro";
        public const string INFO_NOT_EXISTS_DATA_ITEMS = "No se ha podido obtener los registros";
        public const string INFO_NOT_EXISTS_DATA_PROCESS = "El registro no existe para realizar esta operacion";
        public const string WARNING_UPDATE_ESTADO = "No se puede actualizar el estado del Registro";
        public const string WARNING_DELETE = "No se puede eliminar eliminar el registro";
        public const string ERROR_INSERT_DB = "No se ha realizado la inserción a la base de datos";
        public const string ERROR_UPDATE_DB = "No se ha realizado la actualización en la base de datos";
        public const string ERROR_DELETE_DB = "No se ha realizado la eliminación en la base de datos";
        public const string ERROR_SERVER = "Ocurrío un error interno en el servidor";
        public const string ERROR_SERVICE = "Ocurrío un error interno en el servicio";
        public const string ERROR_SERVICE_OSE_SUNAT = "Ocurrío un error interno en el servicio de OSE SUNAT";
        public const string ERROR_SERVICE_REFIT = "Ocurrío un error interno en la conexion del servicio";
        public const string ERROR_SERVICE_CONEXION = "No se puede establecer una conexión ya que el equipo de destino denegó expresamente dicha conexión";
        public const string ERROR_VALIDATION_MODEL = "Se produjeron uno o más errores de validación";
        public const string ERROR_NOT_FOUND = "Ruta de API no encontrado";

        // Comprobante
        public const string COMPROBANTE_INFO_EXISTS_DATA_DOCUMENTO_FUENTE = "El documento fuente, ha sido utilizado para una nota crédito ó débito";
        public const string COMPROBANTE_INFO_NOT_EXISTS_DATA_DOCUMENTO_FUENTE = "No se ha podido obtener el documento fuente";
        public const string COMPROBANTE_SUCCESS_INSERT = "El comprobante de pago ha sido generado correctamente";
        public const string COMPROBANTE_RETENCION_SUCCESS_INSERT = "El comprobante de retención ha sido generado correctamente";

        // Pedido Pecosa
        public const string PEDIDO_PECOSA_INFO_NOT_EXISTS_DATA_DOCUMENTO_FUENTE = "No se ha podido encontrar registros con el número de pecosa";


    }
}
