using System.Collections.Generic;

namespace SiogaApiAuthorization
{
    public class AppSettings
    {
        public JwtConfig JwtConfig { get; set; }
        public string CodigoSistema { get; set; }
        public ModuloApp Modulo { get; set; }

    }

    public class JwtConfig
    {
        public string SecretKey { get; set; }
        public string AudienceToken { get; set; }
        public string IssuerToken { get; set; }
        public string ExpireMinutes { get; set; }
    }

    public class ModuloApp
    {
        public SubvecionApp Subvencion { get; set; }
    }

    public class SubvecionApp
    {
        public string Codigo { get; set; }
        public SubvecionRol Rol { get; set; }
    }

    public class SubvecionRol
    {
        public string Sub01 { get; set; }
        public string Sub02 { get; set; }
        public string Sub03 { get; set; }
        public string Sub04 { get; set; }
    }
}
