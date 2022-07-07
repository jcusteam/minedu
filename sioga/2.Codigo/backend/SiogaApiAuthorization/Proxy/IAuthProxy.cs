using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using SiogaApiAuthorization.Domain;
using SiogaUtils;

namespace SiogaApiAuthorization.Proxy
{
    public interface IAuthProxy
    {
        Task<StatusApiResponse<Boot>> GetBoot();
        Task<StatusApiResponse<Usuario>> GetUsuario(string codigoSistema, string headerAuth);
        Task<StatusApiResponse<List<Modulo>>> GetModulos(string codigoSistema, string headerAuth);
        Task<StatusApiResponse<List<Menu>>> GetMenus(string codigoSistema, string codigoModulo, string headerAuth);
        Task<StatusApiResponse<List<Rol>>> GetRoles(string codigoSistema, string headerAuth);
        Task<StatusApiResponse<List<Accion>>> GetAcciones(string codigoSistema, int idRol, int idMenu, string headerAuth);
        Task<StatusApiResponse<Sesion>> GetSesion(string codigoSistema, string headerAuth);
    }
}
