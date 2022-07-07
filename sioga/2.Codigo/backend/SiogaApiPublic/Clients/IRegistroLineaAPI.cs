using System.Threading.Tasks;
using Refit;
using SiogaApiPublic.Domain;
using SiogaUtils;

namespace SiogaApiPublic.Clients
{
    public interface IRegistroLineaAPI
    {
        [Post("/api/registro-lineas")]
        Task<StatusApiResponse<RegistroLinea>> AddAsync(RegistroLinea registroLinea);
    }
}
