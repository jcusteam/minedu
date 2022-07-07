using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiPapeletaDeposito.Clients
{
    public interface IEstadoAPI
    {
        [Get("/api/estados/consulta")]
        Task<StatusResponse<Estado>> FindByTipoDocAndNumeroAsync([FromQuery] int tipoDocumentoId, [FromQuery] int numero);
    }
}
