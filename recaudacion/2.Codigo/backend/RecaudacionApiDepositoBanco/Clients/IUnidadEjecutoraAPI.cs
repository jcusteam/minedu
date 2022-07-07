using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiDepositoBanco.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
