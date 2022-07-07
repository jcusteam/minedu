using SiogaApiAuthorization.Domain;

namespace SiogaApiAuthorization.Application.Query.Dtos
{
    public class UsuarioInstitucionTokenDto
    {
        public string Token { get; set; }
        public Usuario Usuario { get; set; }
        public Institucion Institucion { get; set; }
    }
}
