using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiLiquidacion.Clients
{
    public interface IFuenteFinanciamientoAPI
    {
        [Get("/api/fuentes-financiamiento/{id}")]
        Task<StatusResponse<FuenteFinanciamiento>> FindByIdAsync(int id);
    }
}
