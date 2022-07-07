using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiGuiaSalidaBien.Clients
{
    public interface ITipoDocumentoAPI
    {
        [Get("/api/tipo-documentos/{id}")]
        Task<StatusResponse<TipoDocumento>> FindByIdAsync(int id);
    }
}
