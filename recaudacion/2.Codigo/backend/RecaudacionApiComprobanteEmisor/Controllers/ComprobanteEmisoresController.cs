using System;
using System.Threading.Tasks;
using RecaudacionApiComprobanteEmisor.Application.Command;
using RecaudacionApiComprobanteEmisor.Application.Command.Dtos;
using RecaudacionApiComprobanteEmisor.Application.Query;
using RecaudacionApiComprobanteEmisor.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiComprobanteEmisor.Helpers;
using RecaudacionApiComprobanteEmisor.Application.Command.Request;
using RecaudacionApiComprobanteEmisor.Application.Command.Response;

namespace RecaudacionApiComprobanteEmisor.Controllers
{

    [Produces("application/json")]
    [Route("api/comprobantes-emisor")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class ComprobanteEmisoresController : ControllerBase
    {
        private IMediator _mediator;

        public ComprobanteEmisoresController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/comprobantes-emisor
        [HttpGet]
        public async Task<ActionResult<FindAllComprobanteEmisorHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllComprobanteEmisorHandler.Query());
        }

        // Get: api/comprobantes-emisor
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageComprobanteEmisorHandler.StatusPageResponse>> FindAll([FromQuery] ComprobanteEmisorFilterDto parms)
        {
            var query = new PageComprobanteEmisorHandler.Query();
            query.ComprobanteEmisorFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/comprobantes-emisor/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdComprobanteEmisorHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdComprobanteEmisorHandler.Query { Id = id });
        }

        // Get: api/comprobantes-emisor/unidad-ejecutora/1
        [HttpGet("unidad-ejecutora/{id}")]
        public async Task<ActionResult<FindByUnidadEjecutoraComprobanteEmisorHandler.StatusFindUnidadEjecutoraResponse>> FindByUnidadEjecutoraId(int id)
        {
            return await _mediator.Send(new FindByUnidadEjecutoraComprobanteEmisorHandler.Query { UnidadEjecutoraId = id });
        }

        // POST: api/comprobantes-emisor/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<StatusAddResponse>> Add([FromBody] ComprobanteEmisorFormDto requet)
        {
            return await _mediator.Send(new CommandAdd { FormDto = requet });
        }

        // PUT: api/comprobantes-emisor/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<StatusUpdateResponse>> Update(int id, [FromBody] ComprobanteEmisorFormDto requet)
        {
            return await _mediator.Send(new CommandUpdate { Id = id, FormDto = requet });
        }

        // PUT: api/comprobantes-emisor/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] ComprobanteEmisorEstadoFormDto requet)
        {
            return await _mediator.Send(new CommandUpdateEstado { Id = id, FormDto = requet });
        }

        // DELETE: api/comprobantes-emisor/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<DeleteComprobanteEmisorHandler.StatusDeleteResponse>> Delete(int id)
        //{
        //    return await _mediator.Send(new DeleteComprobanteEmisorHandler.Command { Id = id });
        //}
    }
}
