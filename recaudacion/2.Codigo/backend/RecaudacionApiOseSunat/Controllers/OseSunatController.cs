using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiOseSunat.Applicacion.Command;
using RecaudacionApiOseSunat.Domain;
using RecaudacionApiOseSunat.Helpers;

namespace RecaudacionApiOseSunat.Controllers
{
    [Produces("application/json")]
    [Route("api/ose-sunat")]
    [EnableCors("MineduPolicy")]
    public class OseSunatController : ControllerBase
    {
        private IMediator _mediator;

        public OseSunatController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // POST: api/ose-sunat/comprobantes
        [HttpPost("comprobantes")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<SendComprobanteHandler.StatusSendResponse>> SendComprobante([FromBody] Documento formDto)
        {
            var command = new SendComprobanteHandler.Command();
            command.FormDto = formDto;
            return await _mediator.Send(command);
        }

        // POST: api/ose-sunat/retenciones
        [HttpPost("retenciones")]
        [InjectionHtmlAtribute]
        public async Task<ActionResult<SendRetencionHandler.StatusSendRetencionResponse>> SendRetencion([FromBody] Retencion formDto)
        {
            var command = new SendRetencionHandler.Command();
            command.FormDto = formDto;
            return await _mediator.Send(command);
        }
    }
}
