using System.Threading.Tasks;
using RecaudacionApiParametro.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiParametro.Clients
{
    public interface ITipoDocumentoAPI
    {
        [Get("/api/tipo-documentos/{id}")]
        Task<StatusResponse<TipoDocumento>> FindByIdAsync(int id);
    }
}
