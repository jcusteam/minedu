using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiIngresoPecosa.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiIngresoPecosa.Clients
{
    public interface IEstadoAPI
    {
        [Get("/api/estados/consulta")]
        Task<StatusResponse<Estado>> FindByTipoDocAndNumeroAsync([FromQuery] int tipoDocumentoId, [FromQuery] int numero);
    }
}
