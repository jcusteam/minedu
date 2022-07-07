using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobantePago.Clients
{
    public interface ITarifarioAPI
    {
        [Get("/api/tarifario/{id}")]
        Task<StatusResponse<Tarifario>> FindByIdAsync(int id);
    }
}
