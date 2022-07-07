using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiIngresoPecosa.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiIngresoPecosa.Clients
{
    public interface IPedidoPecosaAPI
    {
        [Get("/api/pedido-pecosa/ejecutora/consulta")]
        Task<StatusResponse<PedidoPecosa>> FindByEjecutoraAsync([FromQuery] string ejecutora, [FromQuery] int anioEje, [FromQuery] int numeroPecosa);
        [Get("/api/pedido-pecosa/ejecutora/consulta")]
        Task<StatusResponse<List<PedidoPecosa>>> FindAllAsync([FromQuery] string ejecutora, [FromQuery] int anioEje, [FromQuery] int numeroPecosa);
    }
}
