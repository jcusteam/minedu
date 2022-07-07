using System;

namespace SiogaApiAuthorization.Domain
{
    public class UsuarioInstitucion
    {
        public Institucion Institucion { get; set; }
    }

    public class Institucion
    {
        public int InstitucionId { get; set; }
        public string Nombre { get; set; }
    }
}