using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiRegistroLinea.Clients
{
    public interface IClasificadorIngresoAPI
    {
        [Get("/api/clasificador-ingresos/{id}")]
        Task<StatusResponse<ClasificadorIngreso>> FindByIdAsync(int id);
    }
}
