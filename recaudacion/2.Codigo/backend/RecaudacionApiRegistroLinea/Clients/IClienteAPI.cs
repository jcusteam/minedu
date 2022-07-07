using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiRegistroLinea.Clients
{
    public interface IClienteAPI
    {
        [Get("/api/clientes/{id}")]
        Task<StatusResponse<Cliente>> FindByIdAsync(int id);
        [Get("/api/clientes/consulta")]
        Task<StatusResponse<Cliente>> FindByNroDocAsync([FromQuery] int tipoDocumentoIdentidadId, [FromQuery] string numeroDocumento);
        [Post("/api/clientes")]
        Task<StatusResponse<Cliente>> AddAsync(Cliente cliente);
        [Put("/api/clientes/{id}")]
        Task<StatusResponse<Cliente>> UpdateAsync(Cliente cliente, int id);
    }
}
