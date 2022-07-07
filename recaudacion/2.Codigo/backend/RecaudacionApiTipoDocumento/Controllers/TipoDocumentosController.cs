using System;
using System.Threading.Tasks;
using RecaudacionApiTipoDocumento.Application.Command;
using RecaudacionApiTipoDocumento.Application.Command.Dtos;
using RecaudacionApiTipoDocumento.Application.Query;
using RecaudacionApiTipoDocumento.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiTipoDocumento.Helpers;

namespace RecaudacionApiTipoDocumento.Controllers
{

    [Produces("application/json")]
    [Route("api/tipo-documentos")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class TipoDocumentosController : ControllerBase
    {
        private IMediator _mediator;

        public TipoDocumentosController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/tipo-documentos
        [HttpGet]
        public async Task<ActionResult<FindAllTipoDocumentoHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllTipoDocumentoHandler.Query());
        }

        // Get: api/tipo-documentos/paginar
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageTipoDocumentoHandler.StatusPageResponse>> FindAll([FromQuery] TipoDocumentoFilterDto parms)
        {
            var query = new PageTipoDocumentoHandler.Query();
            query.TipoDocumentoFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/tipo-documentos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdTipoDocumentoHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdTipoDocumentoHandler.Query { Id = id });
        }

        // POST: api/tipo-documentos/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddTipoDocumentoHandler.StatusAddResponse>> Add([FromBody] TipoDocumentoFormDto requet)
        {
            var command = new AddTipoDocumentoHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/tipo-documentos/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateTipoDocumentoHandler.StatusUpdateResponse>> Update(int id, [FromBody] TipoDocumentoFormDto requet)
        {
            var command = new UpdateTipoDocumentoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/tipo-documentos/estado/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoTipoDocumentoHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] TipoDocumentoEstadoFormDto requet)
        {
            var command = new UpdateEstadoTipoDocumentoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/tipo-documentos/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<DeleteTipoDocumentoHandler.StatusDeleteResponse>> Delete(int id)
        //{
        //    return await _mediator.Send(new DeleteTipoDocumentoHandler.Command { Id = id });
        //}
    }
}
