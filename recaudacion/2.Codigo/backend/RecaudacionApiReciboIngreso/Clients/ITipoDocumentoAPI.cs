using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiReciboIngreso.Clients
{
    public interface ITipoDocumentoAPI
    {
        [Get("/api/tipo-documentos/{id}")]
        Task<StatusResponse<TipoDocumento>> FindByIdAsync(int id);
    }
}
