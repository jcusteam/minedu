using System;
using System.Collections.Generic;

namespace SiogaApiAuthorization.Domain
{
    public class Usuario
    {
        public string IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public int IdTipoDocumento { get; set; }
        public string NombreTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Correo { get; set; }
        public string NombreCompleto { get; set; }
        public Sesion Sesion { get; set; }
        public List<Rol> Roles { get; set; }
        public List<Menu> Menus { get; set; }
        public List<Modulo> Modulos { get; set; }

        public Usuario()
        {
            Roles = new List<Rol>();
            Menus = new List<Menu>();
            Modulos = new List<Modulo>();
        }
    }

    public class UsuarioToken
    {
        public string Token { get; set; }
        public Usuario Usuario { get; set; }
    }

    public class UsuarioAuth
    {
        public string NOMBRES_USUARIO { get; set; }
        public string APELLIDO_PATERNO { get; set; }
        public string APELLIDO_MATERNO { get; set; }
        public int ID_TIPO_DOCUMENTO_ENUM { get; set; }
        public string TIPO_DOCUMENTO_ENUM { get; set; }
        public string NUMERO_DOCUMENTO { get; set; }
        public DateTime FECHA_NACIMIENTO { get; set; }
        public string CORREO_USUARIO { get; set; }
        public string CODIGO_SISTEMA { get; set; }
    }
}
