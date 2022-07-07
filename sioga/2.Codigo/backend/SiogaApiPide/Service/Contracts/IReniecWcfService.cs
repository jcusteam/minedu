
using ReniecProxy;
using System.Threading.Tasks;

namespace SiogaApiPide.Service.Contracts
{
    public interface IReniecWcfService
    {
        Task<buscarDNICascadaResponse> GetReniec(string endpoint, string usuario, string clave, string ipsistema, string dni);
    }
}
