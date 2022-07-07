using System.Threading.Tasks;
using Refit;
using SiogaApiPublic.Domain;
using SiogaUtils;

namespace SiogaApiPublic.Clients
{
    public interface ITipoDocumentoIdentidadAPI
    {
        [Get("/api/tipo-documento-identidad")]
        Task<StatusApiResponse<TipoDocumentoIdentidad[]>> FindAllAsync();
    }
}
