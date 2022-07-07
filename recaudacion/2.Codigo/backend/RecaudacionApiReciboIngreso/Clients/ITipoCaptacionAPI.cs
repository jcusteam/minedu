using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiReciboIngreso.Clients
{
    public interface ITipoCaptacionAPI
    {
        [Get("/api/tipo-captaciones/{id}")]
        Task<StatusResponse<TipoCaptacion>> FindByIdAsync(int id);
    }
}
