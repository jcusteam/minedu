using Newtonsoft.Json;

namespace SiogaApiAuthorization.Domain
{
    public class Menu
    {
        
        public int IdMenu { get; set; }
        public string Codigo { get; set; }
        public string NombreMenu { get; set; }
        public string NombreIcono { get; set; }
        public int? OrdenMenu { get; set; }
        public int IdMenuPadre { get; set; }
        public string Url { get; set; }
        public int TipoOpcion { get; set; }
        public int TotalChildren { get; set; }
    }

    public class MenuAuth
    {
        public int ID_MENU { get; set; }
        public string CODIGO { get; set; }
        public string NOMBRE_MENU { get; set; }
        public string NOMBRE_ICONO { get; set; }
        public int? ORDEN_MENU { get; set; }
        public int ID_MENU_PADRE { get; set; }
        public string URL { get; set; }
        public int TIPO_OPCION { get; set; }
        public string CODIGO_SISTEMA { get; set; }
        public int? ID_ROL { get; set; }
    }
}