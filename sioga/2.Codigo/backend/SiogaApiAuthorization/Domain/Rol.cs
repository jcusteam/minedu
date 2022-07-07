namespace SiogaApiAuthorization.Domain
{
    public class Rol
    {
        public int IdRol { get; set; }
        public string Codigo { get; set; }
        public string NombreRol { get; set; }
        public int IdSede { get; set; }
        public string NombreSede { get; set; }
        public int IdTipoSede { get; set; }
        public string DescriptionTipoSede { get; set; }
        public bool porDefecto { get; set; }
    }

    public class RolAuth
    {
        public int ID_ROL { get; set; }
        public int ID_MENU { get; set; }
        public string CODIGO_ROL { get; set; }
        public string NOMBRE_ROL { get; set; }
        public int ID_SEDE { get; set; }
        public string NOMBRE_SEDE { get; set; }
        public int ID_TIPO_SEDE { get; set; }
        public string DESCRIPCION_TIPO_SEDE { get; set; }
        public bool POR_DEFECTO { get; set; }
        public string CODIGO_SISTEMA { get; set; }
    }
}