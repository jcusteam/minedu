using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiReciboIngreso.Clients
{
    public interface IFuenteFinanciamientoAPI
    {
        [Get("/api/fuentes-financiamiento/{id}")]
        Task<StatusResponse<FuenteFinanciamiento>> FindByIdAsync(int id);
    }
}
