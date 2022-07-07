using System.Collections.Generic;

namespace SiogaApiGateway
{
    public class AppSettings
    {
        public string SecretKeyJWT { get; set; }
        public string[] AllowedSiogaOrigins { get; set; }
        public string SecretKeyAES { get; set; }
        public string CodeSistema { get; set; }
        public string TimeAuthMilliseconds { get; set; }
        public string DDoSAttack { get; set; }
        public string[] Modulos { get; set; }
        public RolApp Rol { get; set; }

    }

    public class RolApp
    {
        public List<string> Recaudacion { get; set; }
        public List<string> Copaseju { get; set; }
        public List<string> Subvencion { get; set; }
    }
}
