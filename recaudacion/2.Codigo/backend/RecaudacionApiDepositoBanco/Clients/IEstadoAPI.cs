using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiDepositoBanco.Clients
{
    public interface IEstadoAPI
    {
        [Get("/api/estados/consulta")]
        Task<StatusResponse<Estado>> FindByTipoDocAndNumeroAsync([FromQuery] int tipoDocumentoId, [FromQuery] int numero);
    }
}
