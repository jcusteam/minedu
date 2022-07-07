using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;
using Refit;
namespace RecaudacionApiPapeletaDeposito.Clients
{
    public interface ICuentaCorrienteAPI
    {
        [Get("/api/cuentas-corrientes/{id}")]
        Task<StatusResponse<CuentaCorriente>> FindByIdAsync(int id);
    }
}
