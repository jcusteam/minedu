using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiRegistroLinea.Clients
{
    public interface ITipoReciboIngresoAPI
    {
        [Get("/api/tipos-recibos-ingresos/{id}")]
        Task<StatusResponse<TipoReciboIngreso>> FindByIdAsync(int id);
    }
}
