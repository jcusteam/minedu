using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiPapeletaDeposito.Clients
{
    public interface IBancoAPI
    {
        [Get("/api/bancos/{id}")]
        Task<StatusResponse<Banco>> FindByIdAsync(int id);
    }
}
