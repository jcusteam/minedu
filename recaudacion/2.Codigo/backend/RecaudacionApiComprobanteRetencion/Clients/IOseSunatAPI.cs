using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobanteRetencion.Clients
{
    public interface IOseSunatAPI
    {
        [Post("/api/ose-sunat/retenciones")]
        Task<StatusResponse<Resultado>> SendAsync(Retencion retencion);
    }
}
