using SiogaApiPide.Domain;
using SiogaUtils;
using System.Threading.Tasks;

namespace SiogaApiPide.Service.Contracts
{
    public interface IReniecService
    {
        Task<StatusApiResponse<Reniec>> FindByDni(string dni, bool menorEdad);
    }
}
