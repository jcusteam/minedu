using System.Threading.Tasks;
using RecaudacionApiParametro.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiParametro.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
