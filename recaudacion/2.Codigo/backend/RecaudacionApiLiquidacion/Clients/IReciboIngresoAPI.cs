using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiLiquidacion.Clients
{
    public interface IReciboIngresoAPI
    {
        [Post("/api/recibo-ingresos")]
        Task<StatusResponse<ReciboIngreso>> AddAsync(ReciboIngreso reciboIngreso);
    }
}
