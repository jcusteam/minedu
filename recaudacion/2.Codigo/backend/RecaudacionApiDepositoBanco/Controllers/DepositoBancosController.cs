using System;
using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Application.Command;
using RecaudacionApiDepositoBanco.Application.Command.Dtos;
using RecaudacionApiDepositoBanco.Application.Query;
using RecaudacionApiDepositoBanco.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiDepositoBanco.Helpers;

namespace RecaudacionApiDepositoBanco.Controllers
{

    [Produces("application/json")]
    [Route("api/deposito-bancos")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class DepositoBancosController : ControllerBase
    {
        private IMediator _mediator;

        public DepositoBancosController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/deposito-bancos
        [HttpGet]
        public async Task<ActionResult<FindAllDepositoBancoHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllDepositoBancoHandler.Query());
        }

        // Get: api/deposito-bancos
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageDepositoBancoHandler.StatusPageResponse>> FindAll([FromQuery] DepositoBancoFilterDto parms)
        {
            var query = new PageDepositoBancoHandler.Query();
            query.DepositoBancoFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/deposito-bancos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdDepositoBancoHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdDepositoBancoHandler.Query { Id = id });
        }

        // POST: api/deposito-bancos/1
        [HttpPost("files")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<FileDepositoBancoHandler.StatusFileResponse>> findFile(DepositoBancoFileDto fileDto)
        {
            return await _mediator.Send(new FileDepositoBancoHandler.Command { FileDto = fileDto });
        }

        // POST: api/deposito-bancos
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddDepositoBancoHandler.StatusAddResponse>> Add([FromBody] DepositoBancoFormDto requet)
        {
            var command = new AddDepositoBancoHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/deposito-bancos/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateDepositoBancoHandler.StatusUpdateResponse>> Update(int id, [FromBody] DepositoBancoFormDto requet)
        {
            var command = new UpdateDepositoBancoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/deposito-bancos/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoDepositoBancoHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] DepositoBancoEstadoFormDto requet)
        {
            var command = new UpdateEstadoDepositoBancoHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/deposito-bancos/1
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteDepositoBancoHandler.StatusDeleteResponse>> Delete(int id)
        {
            return await _mediator.Send(new DeleteDepositoBancoHandler.Command { Id = id });
        }

        // Detalle de recibo de ingreso

        // Get: api/deposito-bancos/detalle/consulta
        [HttpGet("detalle/consulta")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<FindByNumeroDepositoBancoDetalleHandler.StatusFindNumeroDetalleResponse>> FindByNumeroAndFecha(
            [FromQuery] DepositoBancoDetalleFilterDto filterDto)
        {
            var query = new FindByNumeroDepositoBancoDetalleHandler.Query();
            query.NumerDeposito = filterDto.NumeroDeposito;
            query.FechaDeposito = filterDto.FechaDeposito;
            query.CuentaCorrienteId = filterDto.CuentaCorrienteId;
            query.ClienteId = filterDto.ClienteId;
            return await _mediator.Send(query);
        }

        // Get: api/deposito-bancos/detalle/1
        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<FindByIdDepositoBancoDetalleHandler.StatusFindDetalleResponse>> FindDetalleById(int id)
        {
            return await _mediator.Send(new FindByIdDepositoBancoDetalleHandler.Query { Id = id });
        }

        // PUT: api/deposito-bancos/detalle/1
        [HttpPut("detalle/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateDepositoBancoDetalleHandler.StatusUpdateDetalleResponse>> UpdateDetalle(int id, [FromBody] DepositoBancoDetalleFormDto requet)
        {
            var command = new UpdateDepositoBancoDetalleHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

    }
}
