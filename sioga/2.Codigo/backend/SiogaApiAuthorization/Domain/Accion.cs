using System;

namespace SiogaApiAuthorization.Domain
{
    public class Accion
    {
        public int IdPermiso { get; set; }
        public string NombrePermiso { get; set; }

    }


    public class AccionAuth
    {

        public int ID_ROL { get; set; }
        public int ID_MENU { get; set; }
        public int ID_PERMISO { get; set; }
        public string NOMBRE_PERMISO { get; set; }
        public string CODIGO_SISTEMA { get; set; }
    }
}