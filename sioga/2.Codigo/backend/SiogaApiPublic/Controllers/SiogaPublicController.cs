using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SiogaApiPublic.Application.Command;
using SiogaApiPublic.Application.Query;
using SiogaApiPublic.Domain;

namespace SiogaApiPublic.Controllers
{
    [Produces("application/json")]
    [Route("api/public")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class SiogaPublicController : ControllerBase
    {

        private IMediator _mediator;

        public SiogaPublicController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        // Get: api/public/bancos
        [HttpGet("bancos")]
        public async Task<ActionResult<FindAllBancoHandler.StatusFindAllBancoResponse>> FindAllBancos()
        {
            return await _mediator.Send(new FindAllBancoHandler.Query());
        }
        // Get: api/public/clasificador-ingresos
        [HttpGet("clasificador-ingresos")]
        public async Task<ActionResult<FindAllClasificadorIngresoHandler.StatusFindAllClasificadorIngresoResponse>> FindAllClasificadorIngresos()
        {
            return await _mediator.Send(new FindAllClasificadorIngresoHandler.Query());
        }
        // Get: api/public/cuentas-corrientes
        [HttpGet("cuentas-corrientes")]
        public async Task<ActionResult<FindAllCuentaCorrienteHandler.StatusFindAllCuentaCorrienteResponse>> FindAllCuentaCorrientes()
        {
            return await _mediator.Send(new FindAllCuentaCorrienteHandler.Query());
        }
        // Get: api/public/bancos
        [HttpGet("tipo-documento-identidad")]
        public async Task<ActionResult<FindAllTipoDocIdentidadHandler.StatusFindAllTipoDocIdentidadResponse>> FindAllTipoDocIdentidades()
        {
            return await _mediator.Send(new FindAllTipoDocIdentidadHandler.Query());
        }
        // Get: api/public/tipos-recibos-ingresos
        [HttpGet("tipos-recibos-ingresos")]
        public async Task<ActionResult<FindAllTipoReciboIngresoHandler.StatusFindAllTipoReciboIngresoResponse>> FindAllTipoReciboIngresos()
        {
            return await _mediator.Send(new FindAllTipoReciboIngresoHandler.Query());
        }

        // Post: api/public/registo-lineas
        [HttpPost("registro-lineas")]
        public async Task<ActionResult<AddRegistroLineaHandler.StatusAddRegistroLineaResponse>> AddRegistroLinea(DataModel FormDto)
        {
            return await _mediator.Send(new AddRegistroLineaHandler.Command { FormDto = FormDto });
        }

    }
}
