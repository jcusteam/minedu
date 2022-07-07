using System;
using System.Threading.Tasks;
using RecaudacionApiPedidoPecosa.Application.Query;
using RecaudacionApiPedidoPecosa.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiPedidoPecosa.Helpers;

namespace RecaudacionApiPedidoPecosa.Controllers
{

    [Produces("application/json")]
    [Route("api/pedido-pecosa")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class PedidoPecosasController : ControllerBase
    {
        private IMediator _mediator;

        public PedidoPecosasController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/pedido-pecosa
        [HttpGet("consulta")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<FindAllPedidoPecosaHandler.StatusFindAllResponse>> FindAll([FromQuery] ConsultaDto consultaDto)
        {
            return await _mediator.Send(new FindAllPedidoPecosaHandler.Query { ConsultaDto = consultaDto });
        }

        // Get: api/pedido-pecosa
        [HttpGet("ejecutora/consulta")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<FindByEjecutoraPedidoPecosaHandler.StatusFindByEjecutoraResponse>> FindByEjecutora([FromQuery] ConsultaDto consultaDto)
        {
            return await _mediator.Send(new FindByEjecutoraPedidoPecosaHandler.Query { ConsultaDto = consultaDto });
        }
    }
}
