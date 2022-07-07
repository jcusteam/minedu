using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiDepositoBanco.Clients
{
    public interface ITipoDocIdentidadAPI
    {
        [Get("/api/tipo-documento-identidad/{id}")]
        Task<StatusResponse<TipoDocumentoIdentidad>> FindByIdAsync(int id);
    }
}
