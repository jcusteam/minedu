using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobanteRetencion.Clients
{
    public interface ITipoDocumentoAPI
    {
        [Get("/api/tipo-documentos/{id}")]
        Task<StatusResponse<TipoDocumento>> FindByIdAsync(int id);
    }
}
