using System.Threading.Tasks;
using RecaudacionApiComprobanteEmisor.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobanteEmisor.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
