using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ESinadApiExpediente.Application.Query;

namespace ESinadApiExpediente.Controllers
{
    [Produces("application/json")]
    [Route("api/esinad/expedientes")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class ExpedientesController : ControllerBase
    {
        private IMediator _mediator;

        public ExpedientesController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/esinad/consulta/expediente
        [HttpGet("consulta")]
        public async Task<ActionResult<FindByExpedienteNumeroHandler.StatusFindExpedienteResponse>> FindByNumeroExpediente([FromQuery] string numeroExpediente)
        {
            return await _mediator.Send(new FindByExpedienteNumeroHandler.Query { NumeroExpediente = numeroExpediente });
        }
    }
}
