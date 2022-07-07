using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Refit;
using SiogaApiGateway.Helpers;
using SiogaUtils;

namespace SiogaApiGateway.RestClients
{
    public interface IAuthorizationAPI
    {
        [Post("/api/authorization/usuarios")]
        Task<StatusApiResponse<Usuario>> GetUsuario([FromBody] DataAuth dataAuth, [Header("Authorization")] string headerAuth);
        [Post("/api/authorization/roles")]
        Task<StatusApiResponse<List<Rol>>> GetRoles([FromBody] DataAuth dataAuth, [Header("Authorization")] string headerAuth);
        [Post("/api/authorization/modulos")]
        Task<StatusApiResponse<List<Modulo>>> GetModulos([FromBody] DataAuth dataAuth, [Header("Authorization")] string headerAuth);
    }
}
