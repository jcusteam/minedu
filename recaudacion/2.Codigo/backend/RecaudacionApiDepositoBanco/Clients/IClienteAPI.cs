using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiDepositoBanco.Clients
{
    public interface IClienteAPI
    {
        [Get("/api/clientes/{id}")]
        Task<StatusResponse<Cliente>> FindByIdAsync(int id);
        [Get("/api/clientes/consulta")]
        Task<StatusResponse<Cliente>> FindByTipoNroDocAsync([FromQuery] int tipoDocumentoIdentidadId, [FromQuery] string numeroDocumento);
        [Post("/api/clientes")]
        Task<StatusResponse<Cliente>> AddAsync(Cliente cliente);
    }
}
