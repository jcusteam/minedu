using System.Threading.Tasks;
using RecaudacionApiEstado.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiEstado.Clients
{
    public interface ITipoDocumentoAPI
    {
        [Get("/api/tipo-documentos/{id}")]
        Task<StatusResponse<TipoDocumento>> FindByIdAsync(int id);
    }
}
