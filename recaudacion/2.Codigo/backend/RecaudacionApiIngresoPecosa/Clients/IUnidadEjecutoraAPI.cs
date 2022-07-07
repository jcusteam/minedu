using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiIngresoPecosa.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
