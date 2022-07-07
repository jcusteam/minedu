using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiPapeletaDeposito.Clients
{
    public interface ITipoReciboIngresoAPI
    {
        [Get("/api/tipos-recibos-ingresos/{id}")]
        Task<StatusResponse<TipoReciboIngreso>> FindByIdAsync(int id);
    }
}
