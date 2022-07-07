using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiReciboIngreso.Clients
{
    public interface ITipoReciboIngresoAPI
    {
        [Get("/api/tipos-recibos-ingresos/{id}")]
        Task<StatusResponse<TipoReciboIngreso>> FindByIdAsync(int id);

        [Get("/api/tipos-recibos-ingresos")]
        Task<StatusResponse<List<TipoReciboIngreso>>> FindAllAsync();
    }
}
