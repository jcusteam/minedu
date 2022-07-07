using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiGuiaSalidaBien.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
