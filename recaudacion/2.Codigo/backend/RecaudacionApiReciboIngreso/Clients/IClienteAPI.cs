using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiReciboIngreso.Clients
{
    public interface IClienteAPI
    {
        [Get("/api/clientes/{id}")]
        Task<StatusResponse<Cliente>> FindByIdAsync(int id);
        [Get("/api/clientes/consulta")]
        Task<StatusResponse<Cliente>> FindByNroDocAsync([FromQuery] string nroDocumento);
        [Post("/api/clientes")]
        Task<StatusResponse<Cliente>> AddAsync(Cliente cliente);
    }
}
