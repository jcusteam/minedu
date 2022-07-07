using System.Threading.Tasks;
using Refit;
using SiogaApiPublic.Domain;
using SiogaUtils;

namespace SiogaApiPublic.Clients
{
    public interface ICuentaCorrienteAPI
    {
        [Get("/api/cuentas-corrientes")]
        Task<StatusApiResponse<CuentaCorriente[]>> FindAllAsync();
    }
}
