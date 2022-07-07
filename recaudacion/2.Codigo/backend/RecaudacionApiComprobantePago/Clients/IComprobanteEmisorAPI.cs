using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobantePago.Clients
{
    public interface IComprobanteEmisorAPI
    {
        [Get("/api/comprobantes-emisor/unidad-ejecutora/{id}")]
        Task<StatusResponse<ComprobanteEmisor>> FindByIdEjecutoraAsync(int id);
    }
}
