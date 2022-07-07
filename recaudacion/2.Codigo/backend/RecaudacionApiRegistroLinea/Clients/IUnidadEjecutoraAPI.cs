using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiRegistroLinea.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
