using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiLiquidacion.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
