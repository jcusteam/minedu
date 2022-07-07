using System.Threading.Tasks;
using Refit;
using SiogaApiPublic.Domain;
using SiogaUtils;

namespace SiogaApiPublic.Clients
{
    public interface ITipoReciboIngresoAPI
    {
        [Get("/api/tipos-recibos-ingresos")]
        Task<StatusApiResponse<TipoReciboIngreso[]>> FindAllAsync();
    }
}
