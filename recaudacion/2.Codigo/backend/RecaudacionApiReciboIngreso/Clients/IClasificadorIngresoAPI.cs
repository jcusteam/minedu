using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiReciboIngreso.Clients
{
    public interface IClasificadorIngresoAPI
    {
        [Get("/api/clasificador-ingresos/{id}")]
        Task<StatusResponse<ClasificadorIngreso>> FindByIdAsync(int id);
    }
}
