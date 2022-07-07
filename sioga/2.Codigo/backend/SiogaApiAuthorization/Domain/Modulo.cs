namespace SiogaApiAuthorization.Domain
{
    public class Modulo
    {
        public int IdModulo { get; set; }
        public string Codigo { get; set; }
        public string NombreModulo { get; set; }
        public string NombreIcono { get; set; }
        public int? Orden { get; set; }
        public string Url { get; set; }
        public int TipoOpcion { get; set; }
    }

    public class ModuloAuth
    {
        public string CODIGO_SISTEMA { get; set; }
        public int? ID_ROL { get; set; }
    }
}