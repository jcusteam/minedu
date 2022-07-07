using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobanteRetencion.Clients
{
    public interface IClienteAPI
    {
        [Get("/api/clientes/{id}")]
        Task<StatusResponse<Cliente>> FindByIdAsync(int id);
    }
}
