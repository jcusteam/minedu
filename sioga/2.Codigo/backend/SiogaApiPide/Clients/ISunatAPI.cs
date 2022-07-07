using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Refit;
using SiogaApiPide.Clients.Request;
using SiogaApiPide.Clients.Response;

namespace SiogaApiPide.Clients
{
    public interface ISunatAPI
    {
        [Post("/api/auth/login")]
        Task<SunatResponse> Login([FromBody] LoginSunatRequest request);
        [Post("/api/v1/datos-principales")]
        Task<SunatResponse> FindByNroRuc([FromBody] SunatRequest request);
        [Post("/api/v1/representantes-legales")]
        Task<SunatResponse> FindRegresetanteByNroDoc([FromBody] SunatRequest request);
    }
}
