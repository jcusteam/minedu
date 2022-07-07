using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobanteRetencion.Clients
{
    public interface IComprobanteEmisorAPI
    {
        [Get("/api/comprobantes-emisor/unidad-ejecutora/{id}")]
        Task<StatusResponse<ComprobanteEmisor>> FindByIdEjecutoraAsync(int id);
    }
}
