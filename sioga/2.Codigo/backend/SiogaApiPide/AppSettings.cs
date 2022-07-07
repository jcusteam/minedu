namespace SiogaApiPide
{
    public class AppSettings
    {
        public SunatApp Sunat { get; set; }
        public MigracionApp Migracion { get; set; }
        public ReniecApp Reniec { get; set; }

    }

    public class SunatApp
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IdServicio { get; set; }
        public string IdSistema { get; set; }
        public string EmailSistema { get; set; }
        public string UsuarioSistema { get; set; }
        public string TipoDocumento { get; set; }
        public string IpUsuarioSistema { get; set; }
        public string SecretKey { get; set; }
        public SunatEmpresaApp Empresa { get; set; }
        public SunatRepresentanteApp Representante { get; set; }
    }

    public class SunatEmpresaApp
    {
        public string IdServicio { get; set; }
    }

    public class SunatRepresentanteApp
    {
        public string IdServicio { get; set; }
    }

    public class MigracionApp
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IdServicio { get; set; }
        public string IdSistema { get; set; }
        public string EmailSistema { get; set; }
        public string UsuarioSistema { get; set; }
        public string TipoDocumento { get; set; }
        public string IpUsuarioSistema { get; set; }
        public string SecretKey { get; set; }
    }
    public class ReniecApp
    {
        public string Servicio { get; set; }
        public ReniecWcfApp Wcf { get; set; }
        public ReniecApiApp Api { get; set; }
    }

    public class ReniecWcfApp
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string IpRemota { get; set; }
        public string Endpoint { get; set; }
    }

    public class ReniecApiApp
    {
        public string DniConsultante { get; set; }
        public string SubConsulta { get; set; }
        public string FormatoFirma { get; set; }
        public string SecretKey { get; set; }
        public string IdSistema { get; set; }
    }
}
