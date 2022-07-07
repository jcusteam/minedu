using System;
using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Application.Command;
using RecaudacionApiRegistroLinea.Application.Command.Dtos;
using RecaudacionApiRegistroLinea.Application.Query;
using RecaudacionApiRegistroLinea.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiRegistroLinea.Helpers;

namespace RecaudacionApiRegistroLinea.Controllers
{

    [Produces("application/json")]
    [Route("api/registro-lineas")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class RegistroLineasController : ControllerBase
    {
        private IMediator _mediator;

        public RegistroLineasController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/registro-lineas
        [HttpGet]
        public async Task<ActionResult<FindAllRegistroLineaHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllRegistroLineaHandler.Query());
        }

        // Get: api/registro-lineas
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageRegistroLineaHandler.StatusPageResponse>> FindAll([FromQuery] RegistroLineaFilterDto parms)
        {
            var query = new PageRegistroLineaHandler.Query();
            query.RegistroLineaFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/registro-lineas/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdRegistroLineaHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdRegistroLineaHandler.Query { Id = id });
        }

        // POST: api/registro-lineas
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddRegistroLineaHandler.StatusAddResponse>> Add([FromBody] RegistroLineaFormDto requet)
        {
            var command = new AddRegistroLineaHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/registro-lineas/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateRegistroLineaHandler.StatusUpdateResponse>> Update(int id, [FromBody] RegistroLineaFormDto requet)
        {
            var command = new UpdateRegistroLineaHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }


        // PUT: api/registro-lineas/estados/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoRegistroLineaHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] RegistroLineaEstadoFormDto requet)
        {
            var command = new UpdateEstadoRegistroLineaHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/registro-lineas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteRegistroLineaHandler.StatusDeleteResponse>> Delete(int id)
        {
            return await _mediator.Send(new DeleteRegistroLineaHandler.Command { Id = id });
        }
    }
}
