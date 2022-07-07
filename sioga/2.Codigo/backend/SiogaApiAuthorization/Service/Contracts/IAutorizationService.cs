using System.Collections.Generic;
using System.Threading.Tasks;
using SiogaApiAuthorization.Domain;
using SiogaUtils;

namespace SiogaApiAuthorization.Service.Contracts
{
    public interface IAutorizationService
    {
        Task<StatusApiResponse<Usuario>> GetAllUsuario(string codigoModulo, string headerAuth);
        Task<StatusApiResponse<Usuario>> GetUsuario(string headerAuth);
        Task<StatusApiResponse<List<Modulo>>> GetModulos(string headerAuth);
        Task<StatusApiResponse<List<Menu>>> GetMenus(string codigoModulo, string headerAuth);
        Task<StatusApiResponse<List<Rol>>> GetRoles(string headerAuth);
        Task<StatusApiResponse<List<Accion>>> GetAcciones(string codigoModulo, string codigoMenu, string headerAuth);
        Task<StatusApiResponse<Sesion>> GetSesion(string headerAuth);
        Task<StatusApiResponse<AppSettings>> GetConfiguracion();
        Task<StatusApiResponse<UsuarioToken>> GetUsuarioToken(string codigoModulo, string headerAuth);
    }
}
