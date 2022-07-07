using System;
using System.Threading.Tasks;
using RecaudacionApiParametro.Application.Command;
using RecaudacionApiParametro.Application.Command.Dtos;
using RecaudacionApiParametro.Application.Query;
using RecaudacionApiParametro.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiParametro.Helpers;

namespace RecaudacionApiParametro.Controllers
{

    [Produces("application/json")]
    [Route("api/parametros")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class ParametrosController : ControllerBase
    {
        private IMediator _mediator;

        public ParametrosController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/Parametros
        [HttpGet]
        public async Task<ActionResult<FindAllParametroHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllParametroHandler.Query());
        }

        // Get: api/Parametros
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageParametroHandler.StatusPageResponse>> FindAll([FromQuery] ParametroFilterDto parms)
        {
            var query = new PageParametroHandler.Query();
            query.ParametroFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/Parametros/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdParametroHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdParametroHandler.Query { Id = id });
        }

        // Get: api/Parametros/1
        [HttpGet("consulta")]
        public async Task<ActionResult<FindByEjecutoraAndTipoParametroHandler.StatusFindEjecutoraResponse>> FindByEjecutoraAndTipo([FromQuery] int unidadEjecutoraId, [FromQuery] int tipoDocumentoId)
        {
            return await _mediator.Send(new FindByEjecutoraAndTipoParametroHandler.Query { UnidadEjecutoraId = unidadEjecutoraId, TipoDocumentoId = tipoDocumentoId });
        }

        // POST: api/Parametros/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddParametroHandler.StatusAddResponse>> Add([FromBody] ParametroFormDto requet)
        {
            var command = new AddParametroHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/Parametros/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateParametroHandler.StatusUpdateResponse>> Update(int id, [FromBody] ParametroFormDto requet)
        {
            var command = new UpdateParametroHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/Parametros/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<DeleteParametroHandler.StatusDeleteResponse>> Delete(int id)
        //{
        //    return await _mediator.Send(new DeleteParametroHandler.Command { Id = id });
        //}
    }
}
