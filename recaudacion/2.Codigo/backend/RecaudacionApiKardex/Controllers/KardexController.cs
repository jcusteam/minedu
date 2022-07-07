using System;
using System.Threading.Tasks;
using RecaudacionApiKardex.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace RecaudacionApiKardex.Controllers
{

    [Produces("application/json")]
    [Route("api/kardex")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class KardexsController : ControllerBase
    {
        private IMediator _mediator;

        public KardexsController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Get: api/Kardexs
        [HttpGet]
        public async Task<ActionResult<FindAllKardexHandler.StatusFindAllResponse>> FindAll([FromQuery] int catalogoBienId)
        {
            return await _mediator.Send(new FindAllKardexHandler.Query { CatalogoBienId = catalogoBienId });
        }

    }
}