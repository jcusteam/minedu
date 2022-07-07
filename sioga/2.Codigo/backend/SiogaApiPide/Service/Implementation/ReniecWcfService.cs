using ReniecProxy;
using SiogaApiPide.Service.Contracts;
using System.Threading.Tasks;

namespace SiogaApiPide.Service.Implementation
{
    public class ReniecWcfService : IReniecWcfService
    {
        public async Task<buscarDNICascadaResponse> GetReniec(string endpoint, string usuario, string clave, string ipsistema, string dni)
        {
            ReniecProxy.ReniecWSClient consultareniec = new ReniecProxy.ReniecWSClient(
            ReniecProxy.ReniecWSClient.EndpointConfiguration.ReniecWSPort, endpoint);

            var resultReniec = await consultareniec.buscarDNICascadaAsync(new ReniecProxy.buscarDNICascada()
            {
                dni = dni,
                ipsistema = ipsistema,
                clave = clave,
                usuario = usuario,
            });

            return resultReniec;
        }
    }
}
