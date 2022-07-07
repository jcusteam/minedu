using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobantePago.Clients
{
    public interface ITipoDocumentoAPI
    {
        [Get("/api/tipo-documentos/{id}")]
        Task<StatusResponse<TipoDocumento>> FindByIdAsync(int id);
    }
}
