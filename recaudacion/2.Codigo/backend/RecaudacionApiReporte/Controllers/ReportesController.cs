using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RecaudacionApiReporte.Application.Command.Dtos;
using RecaudacionApiReporte.Application.Command;
using MediatR;
using RecaudacionApiReporte.Helpers;

namespace RecaudacionApiReporte.Controllers
{
    [Produces("application/json")]
    [Route("api/reportes")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private IMediator _mediator;

        public ReportesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("recibo-ingreso")]
        [InjectionHtmlAtribute]
        public async Task<IActionResult> FinReporteReciboIngerso(ReciboIngresoDto reciboIngresoDto)
        {
            var response = await _mediator.Send(new ReciboIngresoHandler.Command { ReciboIngresoDto = reciboIngresoDto });
            if (response.Success)
            {
                return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);

            }
            else
            {
                return Ok(response);
            }

        }
        [HttpPost("recibo-ingreso/captacion-ventanilla")]
        [InjectionHtmlAtribute]
        public async Task<IActionResult> FinReporteReciboIngersoVentanilla(ReciboIngresoVentanillaDto reciboIngresoVentanillaDto)
        {
            var response = await _mediator.Send(new ReciboIngresoVentanillaHandler.Command { ReciboIngresoVentanillaDto = reciboIngresoVentanillaDto });
            if (response.Success)
            {
                return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);

            }
            else
            {
                return Ok(response);
            }

        }

        [HttpPost("saldo-almacen")]
        [InjectionHtmlAtribute]
        public async Task<IActionResult> FinReporteSaldoAlmacen(SaldoAlmacenDto saldoAlmacenDto)
        {

            var response = await _mediator.Send(new SaldoAlmacenHandler.Command { SaldoAlmacenDto = saldoAlmacenDto });
            if (response.Success)
            {
                return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);

            }
            else
            {
                return Ok(response);
            }

        }

        [HttpPost("kardex-almacen")]
        [InjectionHtmlAtribute]
        public async Task<IActionResult> FinReporteKardeAlmacen(KardexAlmacenDto kardexAlmacenDto)
        {

            var response = await _mediator.Send(new KardexAlmacenHandler.Command { KardexAlmacenDto = kardexAlmacenDto });
            if (response.Success)
            {
                return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);

            }
            else
            {
                return Ok(response);
            }

        }
    }

}
