using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SiogaApiPide.Application.Query;
using SiogaUtils;

namespace SiogaApiPide.Controllers
{
    [Produces("application/json")]
    [Route("api/pide")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class PideController : ControllerBase
    {
        private IMediator _mediator;

        public PideController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("reniec/consultas/{dni}")]
        public async Task<ActionResult<FindByDniReniecHandler.StatusReniecResponse>> FindReniecByDni(
            string dni)
        {

            return await _mediator.Send(new FindByDniReniecHandler.Query { Dni = dni, MenorEdad = false });
        }

        [HttpGet("migraciones/consultas/{numero}")]
        public async Task<ActionResult<FindByNroDocMigracionHandler.StatusMigracionResponse>> FindMigracionesByNroDoc(
            string numero)
        {
            return await _mediator.Send(new FindByNroDocMigracionHandler.Query { Numero = numero });
        }

        [HttpGet("sunat/consultas/{numeroRuc}")]
        public async Task<ActionResult<FindByNroRucSunatHandler.StatusSunatResponse>> FindSunatByNroRuc(
            string numeroRuc)
        {
            return await _mediator.Send(new FindByNroRucSunatHandler.Query { NumeroRuc = numeroRuc });
        }

        [HttpGet("reniec/consultas/subvenciones/{dni}")]
        public async Task<ActionResult<FindByDniReniecHandler.StatusReniecResponse>> FindReniecByDniSubvencion(string dni)
        {
            return await _mediator.Send(new FindByDniReniecHandler.Query { Dni = dni, CodigoModulo = Definition.MODULO_SUBVENCIONES, MenorEdad = false });
        }

        [HttpGet("reniec/consultas/subvenciones/beneficiarios/{dni}")]
        public async Task<ActionResult<FindByDniReniecHandler.StatusReniecResponse>> FindReniecByDniSubvencionBeneficiario(string dni)
        {

            return await _mediator.Send(new FindByDniReniecHandler.Query { Dni = dni, CodigoModulo = Definition.MODULO_SUBVENCIONES, MenorEdad = true });
        }

        [HttpGet("sunat/consultas/subvenciones/{numeroRuc}")]
        public async Task<ActionResult<FindByNroRucSunatHandler.StatusSunatResponse>> FindSunatByNroRucSubvencion(
           string numeroRuc)
        {
            return await _mediator.Send(new FindByNroRucSunatHandler.Query { NumeroRuc = numeroRuc, CodigoModulo = Definition.MODULO_SUBVENCIONES });
        }
    }
}
