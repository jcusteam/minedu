using Microsoft.AspNetCore.Mvc;
using Refit;
using SiogaApiAuthorization.Domain;
using SiogaUtils;
using System.Threading.Tasks;

namespace SiogaApiAuthorization.Clients
{
    public interface IUsuarioInstitucionAPI
    {
        [Get("/api/usuario-instituciones/consulta")]
        Task<StatusApiResponse<UsuarioInstitucion>> FindByNroDocAsync([FromQuery] string numeroDocumento, [Header("Authorization")] string headerAuth);
    }
}
