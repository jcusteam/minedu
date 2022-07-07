using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobantePago.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
