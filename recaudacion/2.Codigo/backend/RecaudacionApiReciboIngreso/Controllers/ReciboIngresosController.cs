using System;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Application.Command;
using RecaudacionApiReciboIngreso.Application.Command.Dtos;
using RecaudacionApiReciboIngreso.Application.Query;
using RecaudacionApiReciboIngreso.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiReciboIngreso.Helpers;

namespace RecaudacionApiReciboIngreso.Controllers
{

    [Produces("application/json")]
    [Route("api/recibo-ingresos")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class ReciboIngresosController : ControllerBase
    {
        private IMediator _mediator;

        public ReciboIngresosController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/ReciboIngresos
        [HttpGet]
        public async Task<ActionResult<FindAllHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllHandler.Query());
        }

        // Get: api/recibo-ingresos
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageHandler.StatusPageResponse>> FindAll([FromQuery] ReciboIngresoFilterDto parms)
        {
            var query = new PageHandler.Query();
            query.ReciboIngresoFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/recibo-ingresos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdHandler.Query { Id = id });
        }

        // Get: api/recibo-ingresos/1
        [HttpGet("consulta")]
        public async Task<ActionResult<FindByNumeroEjecutoraCuentaHandler.StatusFindNumeroEjecutoraResponse>> FindByNumeroAndEjecutora(
            [FromQuery] string numero, [FromQuery] int unidadEjecutoraId, [FromQuery] int cuentaCorrienteId
        )
        {
            return await _mediator.Send(new FindByNumeroEjecutoraCuentaHandler.Query
            {
                Numero = numero,
                UnidadEjecutoraId = unidadEjecutoraId,
                CuentaCorrienteId = cuentaCorrienteId
            });
        }

        // Get: api/charts/tipo-recibo-ingreso
        [HttpGet("charts/tipo-recibo-ingreso")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<FindByTipoReciboChartHandler.StatusChartTipoReciboResponse>> FindChartTipoRecibo([FromQuery] ChartFilterDto filterDto)
        {
            return await _mediator.Send(new FindByTipoReciboChartHandler.Query { FilterDto = filterDto });
        }

        // POST: api/recibo-ingresos/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddHandler.StatusAddResponse>> Add([FromBody] ReciboIngresoFormDto requet)
        {
            var command = new AddHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/recibo-ingresos/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateHandler.StatusUpdateResponse>> Update(int id, [FromBody] ReciboIngresoFormDto requet)
        {
            var command = new UpdateHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/recibo-ingresos/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] ReciboIngresoEstadoFormDto requet)
        {
            var command = new UpdateEstadoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/recibo-ingresos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteHandler.StatusDeleteResponse>> Delete(int id)
        {
            return await _mediator.Send(new DeleteHandler.Command { Id = id });
        }
    }
}
