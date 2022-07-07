using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiPapeletaDeposito.Clients
{
    public interface ITipoCaptacionAPI
    {
        [Get("/api/tipo-captaciones/{id}")]
        Task<StatusResponse<TipoCaptacion>> FindByIdAsync(int id);
    }
}
