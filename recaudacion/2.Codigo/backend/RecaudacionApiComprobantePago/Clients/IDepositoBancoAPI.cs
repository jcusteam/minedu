using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobantePago.Clients
{
    public interface IDepositoBancoAPI
    {
        [Get("/api/deposito-bancos/detalle/{id}")]
        Task<StatusResponse<DepositoBancoDetalle>> FindDetalleByIdAsync(int id);
        [Put("/api/deposito-bancos/detalle/{id}")]
        Task<StatusResponse<DepositoBancoDetalle>> UpdateDetalleAsync(int id, DepositoBancoDetalle depositoBancoDetalle);
    }
}
