using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiPapeletaDeposito.Clients
{
    public interface IUnidadEjecutoraAPI
    {
        [Get("/api/unidades-ejecutoras/{id}")]
        Task<StatusResponse<UnidadEjecutora>> FindByIdAsync(int id);
    }
}
