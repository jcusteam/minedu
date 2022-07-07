using System;
using System.Collections.Generic;
using System.Text;

namespace SiogaUtils
{
    public static class Definition
    {
        // Estato
        public const bool ACTIVO = true;
        public const bool INACTIVO = false;

        // Operacion
        public const int INSERT = 1;
        public const int UPDATE = 2;

        // Tipo de Mensage
        public const string MESSAGE_TYPE_SUCCESS = "success";
        public const string MESSAGE_TYPE_INFO = "info";
        public const string MESSAGE_TYPE_WARNING = "warning";
        public const string MESSAGE_TYPE_ERROR = "error";

        // Operacion
        public const string MODULO_RECAUDACION = "1";
        public const string MODULO_COMPROBANTE_PAGO = "2";
        public const string MODULO_CONCILIACION_CUENTA = "3";
        public const string MODULO_SUBVENCIONES = "4";

        // tipo Servicio
        public const string TIPO_SERVICIO_API = "1";
        public const string TIPO_SERVICIO_WCF = "2";

        // Acciones de Passport
        public const string ACCEDER = "acceder";
        public const string ACTIVAR = "activar";
        public const string AGREGAR = "agregar";
        public const string ANULAR = "anular";
        public const string APROBAR = "aprobar";
        public const string ASIGNAR = "asignar";
        public const string CONFIRMAR = "confirmar";
        public const string CONSULTAR = "consultar";
        public const string DERIVAR = "derivar";
        public const string DESESTIMAR = "desestimar";
        public const string ELIMINAR = "eliminar";
        public const string EXPORTAR = "exportar";
        public const string IMPORTAR = "importar";
        public const string IMPRIMIR = "imprimir";
        public const string INACTIVAR = "inactivar";
        public const string MODIFICAR = "modificar";
        public const string OBSERVAR = "observar";
        public const string REMITIR = "remitir";

        // JWT Key encrypt anD decrypt user data
        public const string JWT_KEY = "$2a$12$vVqz/ozvy6bWpQg9jFlEgOYzn0I3XWcKKex0e8LxOZRr0QUbjJnjq";

    }
}
