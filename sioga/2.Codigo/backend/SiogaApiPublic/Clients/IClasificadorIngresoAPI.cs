using System.Threading.Tasks;
using Refit;
using SiogaApiPublic.Domain;
using SiogaUtils;

namespace SiogaApiPublic.Clients
{
    public interface IClasificadorIngresoAPI
    {
        [Get("/api/clasificador-ingresos")]
        Task<StatusApiResponse<ClasificadorIngreso[]>> FindAllAsync();
    }
}
