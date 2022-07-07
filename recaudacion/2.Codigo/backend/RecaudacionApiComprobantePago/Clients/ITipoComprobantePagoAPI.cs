using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobantePago.Clients
{
    public interface ITipoComprobantePagoAPI
    {
        [Get("/api/tipos-comprobantes-pago")]
        Task<StatusResponse<List<TipoComprobantePago>>> FindAllAsync();
        [Get("/api/tipos-comprobantes-pago/{id}")]
        Task<StatusResponse<TipoComprobantePago>> FindByIdAsync(int id);
    }
}
