using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiLiquidacion.Clients
{
    public interface IClasificadorIngresoAPI
    {
        [Get("/api/clasificador-ingresos/{id}")]
        Task<StatusResponse<ClasificadorIngreso>> FindByIdAsync(int id);
    }
}
