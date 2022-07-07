using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobantePago.Clients
{
    public interface ICatalogoBienAPI
    {
        [Get("/api/catalogo-bienes/{id}")]
        Task<StatusResponse<CatalogoBien>> FindByIdAsync(int id);
    }
}
