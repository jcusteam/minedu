using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiDepositoBanco.Clients
{
    public interface IBancoAPI
    {
        [Get("/api/bancos/{id}")]
        Task<StatusResponse<Banco>> FindByIdAsync(int id);
    }
}
