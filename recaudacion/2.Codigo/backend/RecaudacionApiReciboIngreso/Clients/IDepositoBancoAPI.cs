using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiReciboIngreso.Clients
{
    public interface IDepositoBancoAPI
    {
        [Get("/api/deposito-bancos/detalle/{id}")]
        Task<StatusResponse<DepositoBancoDetalle>> FindDetalleByIdAsync(int id);
        [Put("/api/deposito-bancos/detalle/{id}")]
        Task<StatusResponse<DepositoBancoDetalle>> UpdateDetalleAsync(int id, DepositoBancoDetalle depositoBancoDetalle);
    }
}
