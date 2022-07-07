using System;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Application.Command;
using RecaudacionApiComprobanteRetencion.Application.Command.Dtos;
using RecaudacionApiComprobanteRetencion.Application.Query;
using RecaudacionApiComprobanteRetencion.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiComprobanteRetencion.Helpers;

namespace RecaudacionApiComprobanteRetencion.Controllers
{

    [Produces("application/json")]
    [Route("api/comprobante-retenciones")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class ComprobanteRetencionesController : ControllerBase
    {
        private IMediator _mediator;

        public ComprobanteRetencionesController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/comprobante-retenciones
        [HttpGet]
        public async Task<ActionResult<FindAllComprobanteRetencionHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllComprobanteRetencionHandler.Query());
        }

        // Get: api/comprobante-retenciones
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageComprobanteRetencionHandler.StatusPageResponse>> FindAll([FromQuery] ComprobanteRetencionFilterDto parms)
        {
            var query = new PageComprobanteRetencionHandler.Query();
            query.ComprobanteRetencionFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/ComprobanteRetencions/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdComprobanteRetencionHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdComprobanteRetencionHandler.Query { Id = id });
        }

        // POST: api/comprobante-retenciones/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddComprobanteRetencionHandler.StatusAddResponse>> Add([FromBody] ComprobanteRetencionFormDto requet)
        {
            var command = new AddComprobanteRetencionHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/comprobante-retenciones/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateComprobanteRetencionHandler.StatusUpdateResponse>> Update(int id, [FromBody] ComprobanteRetencionFormDto requet)
        {
            var command = new UpdateComprobanteRetencionHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/comprobante-retenciones/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoComprobanteRetencionHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] ComprobanteRetencionEstadoFormDto requet)
        {
            var command = new UpdateEstadoComprobanteRetencionHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        //DELETE: api/comprobante-retenciones/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<DeleteComprobanteRetencionHandler.StatusDeleteResponse>> Delete(int id)
        //{
        //    return await _mediator.Send(new DeleteComprobanteRetencionHandler.Command { Id = id });
        //}
    }
}
