using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobanteRetencion.Clients
{
    public interface ITipoComprobantePagoAPI
    {
        [Get("/api/tipos-comprobantes-pago")]
        Task<StatusResponse<List<TipoComprobantePago>>> FindAllAsync();
        [Get("/api/tipos-comprobantes-pago/{id}")]
        Task<StatusResponse<TipoComprobantePago>> FindByIdAsync(int id);
        [Get("/api/tipos-comprobantes-pago/consulta/{codigo}")]
        Task<StatusResponse<TipoComprobantePago>> FindByCodigoAsync(string codigo);
    }
}
