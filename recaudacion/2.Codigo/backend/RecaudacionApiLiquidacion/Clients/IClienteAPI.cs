using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiLiquidacion.Clients
{
    public interface IClienteAPI
    {
        [Get("/api/clientes/{id}")]
        Task<StatusResponse<Cliente>> FindByIdAsync(int id);
    }
}
