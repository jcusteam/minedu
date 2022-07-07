using System;
using System.Collections.Generic;
using System.Text;

namespace SiogaUtils
{
    public static class Message
    {
        public const string SUCCESS_INSERT = "La información ha sido guardada correctamente";
        public const string SUCCESS_UPDATE = "El registro ha sido actualizado correctamente";
        public const string SUCCESS_DELETE = "El registro ha sido eliminado correctamente";
        public const string SUCCESS_UPDATE_ACTIVO = "El registro ha sido activado correctamente";
        public const string SUCCESS_UPDATE_INACTIVO = "El registro ha sido inactivado correctamente";
        public const string INFO_EXISTS_DATA = "Ya se encuentra registrado con los datos ingresados";
        public const string INFO_NOT_EXISTS_DATA = "No se ha podido obtener el registro";
        public const string INFO_NOT_EXISTS_DATA_ITEMS = "No se ha podido obtener los registros";
        public const string INFO_NOT_EXISTS_DATA_PROCESS = "El registro no existe para realizar esta operacion";
        public const string ERROR_INSERT_DB = "No se ha realizado la inserción a la base de datos";
        public const string ERROR_UPDATE_DB = "No se ha realizado la actualización en la base de datos";
        public const string ERROR_SERVER = "Ocurrío un error interno en el servidor";
        public const string ERROR_SERVICE = "Ocurrío un error interno en el servicio";
        public const string ERROR_SERVICE_REFIT = "Ocurrío un error interno en la conexion del servicio";
        public const string ERROR_SERVICE_GATEWAY = "Ocurrío un error interno en el servicio de Api Gateway";
        public const string ERROR_SERVICE_AUTH = "Ocurrío un error en el servicio de Autorización";
        public const string ERROR_SERVICE_RENIEC = "Ocurrío un error en el servicio de la RENIEC";
        public const string ERROR_SERVICE_RENIEC_WCF = "Ocurrío un error en el servicio de la RENIEC WCF";
        public const string ERROR_SERVICE_MIGRACION = "Ocurrío un error en el servicio de Migraciones";
        public const string ERROR_SERVICE_SUNAT = "Ocurrío un error en el servicio de la SUNAT";
        public const string ERROR_UNAUTHORIZED = "unauthorized";
        public const string ERROR_NOT_EXIST_SESSION = "La sesión no existe.";
        public const string ERROR_SESSION_EXPIRED = "La sesión ha caducado.";
        public const string ERROR_USER_NOT_UNAUTHORIZED = "Usuario no autorizado";
        public const string TIME_EXPIRED = "El tiempo de la petición a expirado";

    }
}
