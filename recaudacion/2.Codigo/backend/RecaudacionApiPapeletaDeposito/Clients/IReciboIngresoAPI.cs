using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiPapeletaDeposito.Clients.Dtos;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiPapeletaDeposito.Clients
{
    public interface IReciboIngresoAPI
    {
        [Get("/api/recibo-ingresos/{id}")]
        Task<StatusResponse<ReciboIngreso>> FindByIdAsync(int id);

        [Get("/api/recibo-ingresos/consulta")]
        Task<StatusResponse<ReciboIngreso>> FindByNroEjecutoraAsync([FromQuery] ReciboIngresoConsultaDto consultaDto);
    }
}
