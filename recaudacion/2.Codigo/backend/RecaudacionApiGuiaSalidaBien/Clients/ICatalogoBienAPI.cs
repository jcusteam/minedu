using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiGuiaSalidaBien.Clients
{
    public interface ICatalogoBienAPI
    {
        [Get("/api/catalogo-bienes/{id}")]
        Task<StatusResponse<CatalogoBien>> FindByIdAsync(int id);
    }
}
