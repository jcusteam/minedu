using System;
using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Application.Command;
using RecaudacionApiPapeletaDeposito.Application.Command.Dtos;
using RecaudacionApiPapeletaDeposito.Application.Query;
using RecaudacionApiPapeletaDeposito.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiPapeletaDeposito.Helpers;
using RecaudacionApiPapeletaDeposito.Clients.Dtos;

namespace RecaudacionApiPapeletaDeposito.Controllers
{

    [Produces("application/json")]
    [Route("api/papeleta-depositos")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class PapeletaDepositosController : ControllerBase
    {
        private IMediator _mediator;

        public PapeletaDepositosController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/papeleta-depositos
        [HttpGet]
        public async Task<ActionResult<FindAllPapeletaDepositoHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllPapeletaDepositoHandler.Query());
        }

        // Get: api/papeleta-depositos
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PagePapeletaDepositoHandler.StatusPageResponse>> FindAll([FromQuery] PapeletaDepositoFilterDto parms)
        {
            var query = new PagePapeletaDepositoHandler.Query();
            query.PapeletaDepositoFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/papeleta-depositos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdPapeletaDepositoHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdPapeletaDepositoHandler.Query { Id = id });
        }

        // Get: api/papeleta-depositos/recibo-ingreso/consulta
        [HttpGet("recibo-ingreso/consulta")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<FindByNroEjecutoraReciboIngresoHandler.StatusFindNroEjecutoraResponse>> FindByReciboIngeso([FromQuery] ReciboIngresoConsultaDto consultaDto)
        {
            return await _mediator.Send(new FindByNroEjecutoraReciboIngresoHandler.Query { ConsultaDto = consultaDto });
        }

        // POST: api/papeleta-depositos/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddPapeletaDepositoHandler.StatusAddResponse>> Add([FromBody] PapeletaDepositoFormDto requet)
        {
            var command = new AddPapeletaDepositoHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/papeleta-depositos/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdatePapeletaDepositoHandler.StatusUpdateResponse>> Update(int id, [FromBody] PapeletaDepositoFormDto requet)
        {
            var command = new UpdatePapeletaDepositoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/papeleta-depositos/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoPapeletaDepositoHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] PapeletaDepositoEstadoFormDto requet)
        {
            var command = new UpdateEstadoPapeletaDepositoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/papeleta-depositos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletePapeletaDepositoHandler.StatusDeleteResponse>> Delete(int id)
        {
            return await _mediator.Send(new DeletePapeletaDepositoHandler.Command { Id = id });
        }
    }
}
