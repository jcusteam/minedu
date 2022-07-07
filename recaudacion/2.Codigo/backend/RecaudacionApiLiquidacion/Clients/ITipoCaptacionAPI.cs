using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiLiquidacion.Clients
{
    public interface ITipoCaptacionAPI
    {
        [Get("/api/tipo-captaciones/{id}")]
        Task<StatusResponse<TipoCaptacion>> FindByIdAsync(int id);
    }
}
