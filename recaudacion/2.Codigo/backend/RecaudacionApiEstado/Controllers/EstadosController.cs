using System;
using System.Threading.Tasks;
using RecaudacionApiEstado.Application.Command;
using RecaudacionApiEstado.Application.Command.Dtos;
using RecaudacionApiEstado.Application.Query;
using RecaudacionApiEstado.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiEstado.Helpers;

namespace RecaudacionApiEstado.Controllers
{

    [Produces("application/json")]
    [Route("api/estados")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class EstadosController : ControllerBase
    {
        private IMediator _mediator;

        public EstadosController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/estados
        [HttpGet]
        public async Task<ActionResult<FindAllEstadoHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllEstadoHandler.Query());
        }

        // Get: api/estados
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageEstadoHandler.StatusPageResponse>> FindAll([FromQuery] EstadoFilterDto parms)
        {
            var query = new PageEstadoHandler.Query();
            query.EstadoFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/estados/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdEstadoHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdEstadoHandler.Query { Id = id });
        }

        // Get: api/estados/tipo-documento/1
        [HttpGet("tipo-documento/{id}")]
        public async Task<ActionResult<FindByTipoDocEstadoHandler.StatusFindTipoDocResponse>> FindByTipoDocumento(int id)
        {
            return await _mediator.Send(new FindByTipoDocEstadoHandler.Query { TipoDocumentoId = id });
        }

        // Get: api/estados/tipo-documento/1
        [HttpGet("consulta")]
        public async Task<ActionResult<FindByTipoDocNumeroEstadoHandler.StatusFindTipoDocNumeroResponse>> FindByTipoDocumentoAndNumero([FromQuery] int tipoDocumentoId, [FromQuery] int numero)
        {
            return await _mediator.Send(new FindByTipoDocNumeroEstadoHandler.Query { TipoDocumentoId = tipoDocumentoId, Numero = numero });
        }

        // POST: api/estados/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddEstadoHandler.StatusAddResponse>> Add([FromBody] EstadoFormDto requet)
        {
            var command = new AddEstadoHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/estados/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoHandler.StatusUpdateResponse>> Update(int id, [FromBody] EstadoFormDto requet)
        {
            var command = new UpdateEstadoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/Estados/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteEstadoHandler.StatusDeleteResponse>> Delete(int id)
        {
            return await _mediator.Send(new DeleteEstadoHandler.Command { Id = id });
        }
    }
}
