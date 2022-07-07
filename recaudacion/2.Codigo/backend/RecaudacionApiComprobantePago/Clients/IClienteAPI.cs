using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;
namespace RecaudacionApiComprobantePago.Clients
{
    public interface IClienteAPI
    {
        [Get("/api/clientes/{id}")]
        Task<StatusResponse<Cliente>> FindByIdAsync(int id);
    }
}
