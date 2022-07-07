using System;
using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Application.Command;
using RecaudacionApiComprobantePago.Application.Command.Dtos;
using RecaudacionApiComprobantePago.Application.Query;
using RecaudacionApiComprobantePago.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiComprobantePago.Helpers;

namespace RecaudacionApiComprobantePago.Controllers
{
    [Produces("application/json")]
    [Route("api/comprobante-pagos")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class ComprobantePagosController : ControllerBase
    {
        private IMediator _mediator;

        public ComprobantePagosController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/comprobante-pagos
        [HttpGet]
        public async Task<ActionResult<FindAllComprobantePagoHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllComprobantePagoHandler.Query());
        }

        // Get: api/comprobante-pagos
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageComprobantePagoHandler.StatusPageResponse>> FindAll([FromQuery] ComprobantePagoFilterDto parms)
        {
            var query = new PageComprobantePagoHandler.Query();
            query.ComprobantePagoFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/comprobante-pagos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdComprobantePagoHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdComprobantePagoHandler.Query { Id = id });
        }

        // Get: api/comprobante-pagos/fuente/1
        [HttpGet("fuente")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<FindByFuenteComprobantePagoHandler.StatusFindFuenteResponse>> FindFuente(
            [FromQuery] ComprobantePagoFuenteDto fuenteDto)
        {
            return await _mediator.Send(new FindByFuenteComprobantePagoHandler.Query { FuenteDto = fuenteDto });
        }

        // Get: api/comprobante-pagos/chart/1
        [HttpGet("chart/tipo-comprobante")]
        public async Task<ActionResult<FindByTipoChartComprobantePagoHandler.StatusChartResponse>> FindChartByTipo(
            [FromQuery] int ejecutoraId, [FromQuery] int anio)
        {
            return await _mediator.Send(new FindByTipoChartComprobantePagoHandler.Query { EjecutoraId = ejecutoraId, Anio = anio });
        }

        // POST: api/comprobante-pagos/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddComprobantePagoHandler.StatusAddResponse>> Add([FromBody] ComprobantePagoFormDto requet)
        {
            var command = new AddComprobantePagoHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/comprobante-pagos/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateComprobantePagoHandler.StatusUpdateResponse>> Update(int id, [FromBody] ComprobantePagoFormDto requet)
        {
            var command = new UpdateComprobantePagoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/comprobante-pagos/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoComprobantePagoHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] ComprobantePagoEstadoFormDto requet)
        {
            var command = new UpdateEstadoComprobantePagoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/comprobante-pagos/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<DeleteComprobantePagoHandler.StatusDeleteResponse>> Delete(int id)
        //{
        //    return await _mediator.Send(new DeleteComprobantePagoHandler.Command { Id = id });
        //}
    }
}
