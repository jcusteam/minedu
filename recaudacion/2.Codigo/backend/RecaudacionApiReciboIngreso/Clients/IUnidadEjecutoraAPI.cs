using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiReciboIngreso.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
