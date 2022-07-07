using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Refit;
using SiogaApiPide.Clients.Request;
using SiogaApiPide.Clients.Response;

namespace SiogaApiPide.Clients
{
    public interface IMigracionAPI
    {
        [Post("/api/auth/login")]
        Task<MigracionResponse> Login([FromBody] LoginMigracionRequest request);
        [Post("/api/v1/consultar")]
        Task<MigracionResponse> FindByNroDoc([FromBody] MigracionRequest request);
    }
}
