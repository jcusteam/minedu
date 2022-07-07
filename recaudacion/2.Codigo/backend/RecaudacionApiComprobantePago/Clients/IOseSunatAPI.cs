using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobantePago.Clients
{
    public interface IOseSunatAPI
    {
        [Post("/api/ose-sunat/comprobantes")]
        Task<StatusResponse<Resultado>> SendAsync(Documento documento);
    }
}
