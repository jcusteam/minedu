using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Refit;
using SiogaApiPide.Clients.Request;
using SiogaApiPide.Clients.Response;

namespace SiogaApiPide.Clients
{
    public interface IReniecAPI
    {
        [Post("/por-dni")]
        Task<ReniecResponse> FindByDni([FromBody] ReniecRequest request);
    }
}
