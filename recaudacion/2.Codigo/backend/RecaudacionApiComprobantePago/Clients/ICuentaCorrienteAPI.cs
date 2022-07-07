using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;
namespace RecaudacionApiComprobantePago.Clients
{
    public interface ICuentaCorrienteAPI
    {
        [Get("/api/cuentas-corrientes/{id}")]
        Task<StatusResponse<CuentaCorriente>> FindByIdAsync(int id);

        [Get("/api/cuentas-corrientes/consulta")]
        Task<StatusResponse<CuentaCorriente>> FindByNumeroAsync([FromQuery] string numero);
    }
}
