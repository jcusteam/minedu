using System.Threading.Tasks;
using Refit;
using SiogaApiPublic.Domain;
using SiogaUtils;

namespace SiogaApiPublic.Clients
{
    public interface IBancoAPI
    {
        [Get("/api/bancos")]
        Task<StatusApiResponse<Banco[]>> FindAllAsync();
    }
}
