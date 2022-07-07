using System;
using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Application.Command;
using RecaudacionApiIngresoPecosa.Application.Command.Dtos;
using RecaudacionApiIngresoPecosa.Application.Query;
using RecaudacionApiIngresoPecosa.Application.Query.Dtos;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiIngresoPecosa.Helpers;

namespace RecaudacionApiIngresoPecosa.Controllers
{

    [Produces("application/json")]
    [Route("api/ingreso-pecosa")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class IngresoPecosasController : ControllerBase
    {
        private IMediator _mediator;

        public IngresoPecosasController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/ingreso-pecosa
        [HttpGet]
        public async Task<ActionResult<FindAllIngresoPecosaHandler.StatusFindAllResponse>> FindAll()
        {
            return await _mediator.Send(new FindAllIngresoPecosaHandler.Query());
        }

        // Get: api/ingreso-pecosa
        [HttpGet("paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageIngresoPecosaHandler.StatusPageResponse>> FindAll([FromQuery] IngresoPecosaFilterDto parms)
        {
            var query = new PageIngresoPecosaHandler.Query();
            query.IngresoPecosaFilterDto = parms;
            return await _mediator.Send(query);

        }

        // Get: api/ingreso-pecosa/1
        [HttpGet("{id}")]
        public async Task<ActionResult<FindByIdIngresoPecosaHandler.StatusFindResponse>> FindById(int id)
        {
            return await _mediator.Send(new FindByIdIngresoPecosaHandler.Query { Id = id });
        }
        // Get: api/ingreso-pecosa/detalle/1
        [HttpGet("detalle/{id}")]
        public async Task<ActionResult<FindByIdIngresoPecosaDetalleHandler.StatusFindDetalleResponse>> FindDetalleById(int id)
        {
            return await _mediator.Send(new FindByIdIngresoPecosaDetalleHandler.Query { Id = id });
        }

        // Get: api/ingreso-pecosa/detalle/catalogo-bien/1
        [HttpGet("detalle/catalogo-bien/{id}")]
        public async Task<ActionResult<FindByCatalogoIngresoPecosaDetalleHandler.StatusSaldoResponse>> FindDetalleByCatalogoBien(int id)
        {
            return await _mediator.Send(new FindByCatalogoIngresoPecosaDetalleHandler.Query { CatalogoBienId = id });
        }

        // Get: api/ingreso-pecosa/saldos/paginar
        [HttpGet("saldos/paginar")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<PageCatalogoBienIngresoPecosaDetalleHandler.StatusPageCatalogoBienResponse>> FindCatalogoSaldos([FromQuery] CatalogoBienFilterDto catalogoParams)
        {
            return await _mediator.Send(new PageCatalogoBienIngresoPecosaDetalleHandler.Query { CatalogoBienFilterDto = catalogoParams });
        }

        // Get: api/ingreso-pecosa/saldos
        [HttpGet("saldos")]
        public async Task<ActionResult<FindAllCatalogoBienIngresoPecosaDetalleHandler.StatusFindAllCatalogoBienResponse>> FindCatalogoSaldos()
        {
            return await _mediator.Send(new FindAllCatalogoBienIngresoPecosaDetalleHandler.Query { });
        }

        // POST: api/ingreso-pecosa/1
        [HttpPost]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<AddIngresoPecosaHandler.StatusAddResponse>> Add([FromBody] IngresoPecosaFormDto requet)
        {
            var command = new AddIngresoPecosaHandler.Command();
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/ingreso-pecosa/1
        [HttpPut("{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateIngresoPecosaHandler.StatusUpdateResponse>> Update(int id, [FromBody] IngresoPecosaFormDto requet)
        {
            var command = new UpdateIngresoPecosaHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/ingreso-pecosa/estado/1
        [HttpPut("estado/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateEstadoIngresoPecosaHandler.StatusUpdateEstadoResponse>> UpdateEstado(int id, [FromBody] IngresoPecosaEstadoFormDto requet)
        {
            var command = new UpdateEstadoIngresoPecosaHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // PUT: api/ingreso-pecosa/detalle/1
        [HttpPut("detalle/{id}")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<UpdateIngresoPecosaDetalleHandler.StatusUpdateDetalleResponse>> UpdateDetalle(int id, [FromBody] IngresoPecosaDetalleFormDto requet)
        {
            var command = new UpdateIngresoPecosaDetalleHandler.Command();
            command.Id = id;
            command.FormDto = requet;
            return await _mediator.Send(command);
        }

        // DELETE: api/ingreso-pecosa/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteIngresoPecosaHandler.StatusDeleteResponse>> Delete(int id)
        {
            return await _mediator.Send(new DeleteIngresoPecosaHandler.Command { Id = id });
        }
    }
}
