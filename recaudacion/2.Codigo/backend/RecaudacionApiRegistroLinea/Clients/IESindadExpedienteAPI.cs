using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiRegistroLinea.Clients
{
    public interface IESindadExpedienteAPI
    {
        [Get("/api/esinad/expedientes/consulta")]
        Task<StatusResponse<Expediente>> FindByNumeroExpediente([FromQuery] string numeroExpediente);
    }
}
