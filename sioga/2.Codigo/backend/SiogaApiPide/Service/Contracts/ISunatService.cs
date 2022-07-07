using SiogaApiPide.Domain;
using SiogaUtils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiogaApiPide.Service.Contracts
{
    public interface ISunatService
    {
        Task<StatusApiResponse<string>> Login(string idServicio);
        Task<StatusApiResponse<Sunat>> FindByNroRuc(string numeroRuc);
        Task<StatusApiResponse<List<MultiRef>>> FindRegresetanteByNroDoc(string numeroRuc);

    }
}
