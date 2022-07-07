using System;
using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Application.Command;
using RecaudacionApiLiquidacion.Application.Command.Dtos;
using RecaudacionApiLiquidacion.Application.Query;
using RecaudacionApiLiquidacion.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiLiquidacion.Helpers;

namespace RecaudacionApiLiquidacion.Controllers
{

    [Produces("application/json")]
    [Route("api/liquidaciones")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class LiquidacionesController : ControllerBase
    {
        private IMediator _mediator;

        public LiquidacionesController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/liquidaciones
        [HttpGet]
        public async Task<ActionResult<FindAllLiquidacionHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllLiquidacionHandler.Query());
        }

        // Get: api/liquidaciones
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageLiquidacionHandler.StatusPageResponse>> FindAll([FromQuery] LiquidacionFilterDto parms)
        {
            var query = new PageLiquidacionHandler.Query();
            query.LiquidacionFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/liquidaciones/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdLiquidacionHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdLiquidacionHandler.Query { Id = id });
        }

        // POST: api/liquidaciones/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddLiquidacionHandler.StatusAddResponse>> Add([FromBody] LiquidacionFormDto requet)
        {
            var command = new AddLiquidacionHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/liquidaciones/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateLiquidacionHandler.StatusUpdateResponse>> Update(int id, [FromBody] LiquidacionFormDto requet)
        {
            var command = new UpdateLiquidacionHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/liquidaciones/estado/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoLiquidacionHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] LiquidacionEstadoFormDto requet)
        {
            var command = new UpdateEstadoLiquidacionHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/liquidaciones/1
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteLiquidacionHandler.StatusDeleteResponse>> Delete(int id)
        {
            return await _mediator.Send(new DeleteLiquidacionHandler.Command { Id = id });
        }
    }
}
