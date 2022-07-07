using System;
using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Application.Command;
using RecaudacionApiGuiaSalidaBien.Application.Command.Dtos;
using RecaudacionApiGuiaSalidaBien.Application.Query;
using RecaudacionApiGuiaSalidaBien.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiGuiaSalidaBien.Helpers;

namespace RecaudacionApiGuiaSalidaBien.Controllers
{

    [Produces("application/json")]
    [Route("api/guia-salida-bienes")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class GuiaSalidaBienesController : ControllerBase
    {
        private IMediator _mediator;

        public GuiaSalidaBienesController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/guia-salida-bienes
        [HttpGet]
        public async Task<ActionResult<FindAllGuiaSalidaBienHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllGuiaSalidaBienHandler.Query());
        }

        // Get: api/guia-salida-bienes
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageGuiaSalidaBienHandler.StatusPageResponse>> FindAll([FromQuery] GuiaSalidaBienFilterDto parms)
        {
            var query = new PageGuiaSalidaBienHandler.Query();
            query.GuiaSalidaBienFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/guia-salida-bienes/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdGuiaSalidaBienHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdGuiaSalidaBienHandler.Query { Id = id });
        }

        // POST: api/guia-salida-bienes/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddGuiaSalidaBienHandler.StatusAddResponse>> Add([FromBody] GuiaSalidaBienFormDto requet)
        {
            var command = new AddGuiaSalidaBienHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/guia-salida-bienes/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateGuiaSalidaBienHandler.StatusUpdateResponse>> Update(int id, [FromBody] GuiaSalidaBienFormDto requet)
        {
            var command = new UpdateGuiaSalidaBienHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/guia-salida-bienes/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoGuiaSalidaBienHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] GuiaSalidaBienEstadoFormDto requet)
        {
            var command = new UpdateEstadoGuiaSalidaBienHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/guia-salida-bienes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteGuiaSalidaBienHandler.StatusDeleteResponse>> Delete(int id)
        {
            return await _mediator.Send(new DeleteGuiaSalidaBienHandler.Command { Id = id });
        }
    }
}
