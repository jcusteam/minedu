using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiIngresoPecosa.Clients
{
    public interface ITipoDocumentoAPI
    {
        [Get("/api/tipo-documentos/{id}")]
        Task<StatusResponse<TipoDocumento>> FindByIdAsync(int id);
    }
}
