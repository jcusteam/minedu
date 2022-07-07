using SiogaApiPide.Domain;
using SiogaUtils;
using System.Threading.Tasks;

namespace SiogaApiPide.Service.Contracts
{
    public interface IMigracionService
    {
        Task<StatusApiResponse<Migracion>> FindByNroDoc(string numero);
        Task<StatusApiResponse<string>> Login();
    }
}
