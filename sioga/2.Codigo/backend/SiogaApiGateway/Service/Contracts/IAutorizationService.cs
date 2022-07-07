using SiogaApiGateway.Helpers;
using SiogaUtils;
using System.Threading.Tasks;

namespace SiogaApiGateway.Service.Contracts
{
    public interface IAutorizationService
    {
        Task<StatusApiResponse<DataUser>> GetUsuario(DataAuth dataAuth, string headerAuth);
    }
}
