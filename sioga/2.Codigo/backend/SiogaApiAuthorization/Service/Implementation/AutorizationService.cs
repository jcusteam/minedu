using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiogaApiAuthorization.Domain;
using SiogaApiAuthorization.Helpers;
using SiogaApiAuthorization.Proxy;
using SiogaApiAuthorization.Service.Contracts;
using SiogaUtils;

namespace SiogaApiAuthorization.Service.Implementation
{
    public class AutorizationService : IAutorizationService
    {

        private readonly IAuthProxy _authProxy;
        private readonly AppSettings _appSettings;
        public AutorizationService(IAuthProxy authProxy, AppSettings appSettings)
        {
            _authProxy = authProxy;
            _appSettings = appSettings;
        }

        public async Task<StatusApiResponse<List<Accion>>> GetAcciones(string codigoModulo, string codigoMenu, string headerAuth)
        {
            var response = new StatusApiResponse<List<Accion>>();

            var idMenu = 0;
            var roleResponse = await GetRoles(headerAuth);
            var acciones = new List<Accion>();

            if (!roleResponse.Success)
            {
                response.Success = roleResponse.Success;
                response.Messages = roleResponse.Messages;
                return response;
            }

            var menuResponse = await GetMenus(codigoModulo, headerAuth);

            if (!menuResponse.Success)
            {
                response.Success = menuResponse.Success;
                response.Messages = menuResponse.Messages;
                return response;
            }

            if (menuResponse.Data.Count > 0)
            {
                var menus = menuResponse.Data.Where(x => x.Codigo == codigoMenu).ToList();

                if (menus.Count > 0)
                {
                    idMenu = menus[0].IdMenu;
                }
            }

            if (roleResponse.Data.Count > 0)
            {
                foreach (var item in roleResponse.Data)
                {
                    var accionResponse = await _authProxy.GetAcciones(_appSettings.CodigoSistema, item.IdRol, idMenu, headerAuth);

                    if (accionResponse.Success)
                    {
                        if (accionResponse.Data.Count > 0)
                        {
                            acciones.AddRange(accionResponse.Data);
                        }
                    }
                }

                response.Data = acciones;
            }


            return response;
        }

        public async Task<StatusApiResponse<Usuario>> GetAllUsuario(string codigoModulo, string headerAuth)
        {
            var response = new StatusApiResponse<Usuario>();
            var usuarioResponse = await GetUsuario(headerAuth);
            var sesionResponse = await GetSesion(headerAuth);
            var rolResponse = await GetRoles(headerAuth);
            var menuResponse = await GetMenus(codigoModulo, headerAuth);
            var moduloResponse = await GetModulos(headerAuth);

            var usuario = new Usuario();

            if (!usuarioResponse.Success)
            {
                response.Success = usuarioResponse.Success;
                response.Messages = usuarioResponse.Messages;
                return response;
            }

            usuario = usuarioResponse.Data;

            if (!sesionResponse.Success)
            {
                response.Success = sesionResponse.Success;
                response.Messages = sesionResponse.Messages;
                return response;
            }

            usuario.Sesion = sesionResponse.Data;

            if (!rolResponse.Success)
            {
                response.Success = rolResponse.Success;
                response.Messages = rolResponse.Messages;
                return response;

            }

            usuario.Roles = rolResponse.Data;

            if (!menuResponse.Success)
            {
                response.Success = menuResponse.Success;
                response.Messages = menuResponse.Messages;
                return response;
            }

            usuario.Menus = menuResponse.Data;

            if (!moduloResponse.Success)
            {
                response.Success = moduloResponse.Success;
                response.Messages = moduloResponse.Messages;
                return response;
            }

            usuario.Modulos = moduloResponse.Data;

            response.Data = usuario;

            return response;
        }

        public async Task<StatusApiResponse<AppSettings>> GetConfiguracion()
        {
            var response = new StatusApiResponse<AppSettings>();
            response.Data = _appSettings;
            return await Task.FromResult<StatusApiResponse<AppSettings>>(response);
        }

        public async Task<StatusApiResponse<List<Menu>>> GetMenus(string codigoModulo, string headerAuth)
        {
            return await _authProxy.GetMenus(_appSettings.CodigoSistema, codigoModulo, headerAuth);
        }

        public async Task<StatusApiResponse<List<Modulo>>> GetModulos(string headerAuth)
        {
            return await _authProxy.GetModulos(_appSettings.CodigoSistema, headerAuth);
        }

        public async Task<StatusApiResponse<List<Rol>>> GetRoles(string headerAuth)
        {
            return await _authProxy.GetRoles(_appSettings.CodigoSistema, headerAuth);
        }

        public async Task<StatusApiResponse<Sesion>> GetSesion(string headerAuth)
        {
            return await _authProxy.GetSesion(_appSettings.CodigoSistema, headerAuth);
        }

        public async Task<StatusApiResponse<Usuario>> GetUsuario(string headerAuth)
        {
            return await _authProxy.GetUsuario(_appSettings.CodigoSistema, headerAuth);
        }

        public async Task<StatusApiResponse<UsuarioToken>> GetUsuarioToken(string codigoModulo, string headerAuth)
        {
            var response = new StatusApiResponse<UsuarioToken>();
            var usuarioResponse = await GetAllUsuario(codigoModulo, headerAuth);

            if (!usuarioResponse.Success)
            {
                response.Messages = usuarioResponse.Messages;
                response.Success = false;
                return response;
            }
            var secretKey = _appSettings.JwtConfig.SecretKey;
            var audience = _appSettings.JwtConfig.AudienceToken;
            var issuer = _appSettings.JwtConfig.IssuerToken;
            var expireMinutes = int.Parse(_appSettings.JwtConfig.ExpireMinutes);
            var roles = usuarioResponse.Data.Roles.Select(x => "ROLE_" + x.Codigo.ToUpper()).ToArray().ToList();

            var userJwt = new JwtUserData();
            userJwt.Username = usuarioResponse.Data.NumeroDocumento;
            userJwt.Roles = roles;
            var userData = JsonOptions<JwtUserData>.ToEncrypt(userJwt, Definition.JWT_KEY);
            var tokenJwt = await JwtToken.CreateJWTAsync(userData, roles, issuer, audience, secretKey, expireMinutes);
            var usuarioToken = new UsuarioToken();
            usuarioToken.Token = tokenJwt;
            usuarioToken.Usuario = usuarioResponse.Data;
            response.Data = usuarioToken;

            return response;
        }
    }
}
