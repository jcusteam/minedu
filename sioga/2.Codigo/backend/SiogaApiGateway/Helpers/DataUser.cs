using System.Collections.Generic;

namespace SiogaApiGateway.Helpers
{
    public class DataUser
    {
        public string NumeroDocumento { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Modulos { get; set; }

        public DataUser()
        {
            Roles = new List<string>();
            Modulos = new List<string>();
        }
    }

    public class Usuario
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NumeroDocumento { get; set; }
        public List<Rol> Roles { get; set; }
        public List<Modulo> Modulos { get; set; }

        public Usuario()
        {
            Roles = new List<Rol>();
            Modulos = new List<Modulo>();
        }
    }

    public class Modulo
    {
        public int IdModulo { get; set; }
        public string Codigo { get; set; }
        public string NombreModulo { get; set; }
    }

    public class Rol
    {
        public int IdRol { get; set; }
        public string Codigo { get; set; }
        public string NombreRol { get; set; }
    }
}
