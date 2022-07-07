using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiRegistroLinea.Clients
{
    public interface IReciboIngresoAPI
    {
        [Post("/api/recibo-ingresos")]
        Task<StatusResponse<ReciboIngreso>> AddAsync(ReciboIngreso reciboIngreso);
    }
}
