using System;
using System.Threading.Tasks;
using SiogaApiAuthorization.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SiogaApiAuthorization.Application.Query.Dtos;
using SiogaApiAuthorization.Helpers;

namespace SiogaApiAuthorization.Controllers
{

    [Produces("application/json")]
    [Route("api/authorization")]
    [EnableCors("MineduPolicy")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private IMediator _mediator;

        public AuthorizationController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async Task<ActionResult<FindAllAuthorizationHandler.StatusFindAllResponse>> FindAll(
            [FromBody] AuthDto authDto)
        {
            var headerAuth = Request.GetHeader("Authorization");
            return await _mediator.Send(new FindAllAuthorizationHandler.Query
            {
                AuthDto = authDto,
                HeaderAuth = headerAuth
            });
        }

        [HttpPost("token")]
        public async Task<ActionResult<FindTokenAuthorizationHandler.StatusTokenResponse>> FindToken(
            [FromBody] AuthDto authDto)
        {
            var headerAuth = Request.GetHeader("Authorization");
            return await _mediator.Send(new FindTokenAuthorizationHandler.Query
            {
                AuthDto = authDto,
                HeaderAuth = headerAuth
            });
        }

        [HttpPost("usuarios")]
        public async Task<ActionResult<FindUsuarioAuthorizationHandler.StatusUsuarioResponse>> FindUsuario()
        {
            var headerAuth = Request.GetHeader("Authorization");
            return await _mediator.Send(new FindUsuarioAuthorizationHandler.Query
            {
                HeaderAuth = headerAuth
            });
        }

        [HttpPost("sesion")]
        public async Task<ActionResult<FindSesionAuthorizationHandler.StatusSesionResponse>> FindSesion()
        {
            var headerAuth = Request.GetHeader("Authorization");
            return await _mediator.Send(new FindSesionAuthorizationHandler.Query
            {
                HeaderAuth = headerAuth
            });
        }

        [HttpPost("roles")]
        public async Task<ActionResult<FindRolAuthorizationHandler.StatusRolResponse>> FindRole()
        {
            var headerAuth = Request.GetHeader("Authorization");
            return await _mediator.Send(new FindRolAuthorizationHandler.Query
            {
                HeaderAuth = headerAuth
            });
        }

        [HttpPost("modulos")]
        public async Task<ActionResult<FindModuloAuthorizationHandler.StatusModuloResponse>> FindModulo()
        {
            var headerAuth = Request.GetHeader("Authorization");
            return await _mediator.Send(new FindModuloAuthorizationHandler.Query
            {
                HeaderAuth = headerAuth
            });
        }

        [HttpPost("menus")]
        public async Task<ActionResult<FindMenuAuthorizationHandler.StatusMenuResponse>> FindMenu(
            [FromBody] AuthMenuDto authDto)
        {
            var headerAuth = Request.GetHeader("Authorization");
            return await _mediator.Send(new FindMenuAuthorizationHandler.Query
            {
                AuthDto = authDto,
                HeaderAuth = headerAuth
            });
        }

        [HttpPost("acciones")]
        public async Task<ActionResult<FindAccionAuthorizationHandler.StatusAccionResponse>> FindAccion(
           [FromBody] AuthAccionDto authDto)
        {
            var headerAuth = Request.GetHeader("Authorization");
            return await _mediator.Send(new FindAccionAuthorizationHandler.Query
            {
                AuthDto = authDto,
                HeaderAuth = headerAuth
            });
        }
    }
}
