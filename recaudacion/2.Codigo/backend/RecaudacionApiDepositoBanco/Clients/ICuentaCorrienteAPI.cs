using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiDepositoBanco.Clients
{
    public interface ICuentaCorrienteAPI
    {
        [Get("/api/cuentas-corrientes/{id}")]
        Task<StatusResponse<CuentaCorriente>> FindByIdAsync(int id);
    }
}
