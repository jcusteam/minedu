using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SiogaApiAuthorization.Domain;
using SiogaApiAuthorization.Helpers;
using SiogaUtils;

namespace SiogaApiAuthorization.Proxy
{
    public class AuthProxy : IAuthProxy
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<AuthProxy> _logger;

        public AuthProxy(IHttpClientFactory httpClient,
            ILogger<AuthProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<StatusApiResponse<Boot>> GetBoot()
        {
            var response = new StatusApiResponse<Boot>();
            try
            {
                var client = _httpClient.CreateClient("BootService");
                client.Timeout = TimeSpan.FromSeconds(10);
                var content = new StringContent("", Encoding.UTF8, "application/json");
                var bootResponse = await client.PostAsync($"", content);
                if (bootResponse.IsSuccessStatusCode)
                {
                    var result = await bootResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var boot = new PassportAes().ResultData<Boot>(result);
                        if (boot == null)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se ha podido obtener el Boot"));
                            response.Success = false;
                        }

                        response.Data = boot;
                    }
                    catch (System.Exception)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error al realizar el desencriptado de Boot"));
                        response.Success = false;
                    }
                }
                else
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error en la conexion de api de Boot"));
                    response.Success = false;
                }
            }
            catch (Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, $"Api de Boot: {Message.ERROR_SERVICE_REFIT}"));
                response.Success = false;
            }

            return response;
        }

        public async Task<StatusApiResponse<List<Accion>>> GetAcciones(string codigoSistema, int idRol, int idMenu, string headerAuth)
        {
            var response = new StatusApiResponse<List<Accion>>();

            try
            {
                var bootReponse = await GetBoot();
                if (!bootReponse.Success)
                {
                    response.Messages.Add(new GenericMessage(bootReponse.Messages[0].Type, bootReponse.Messages[0].Message));
                    response.Success = false;
                    return response;
                }

                var rolAuth = new RolAuth()
                {
                    CODIGO_SISTEMA = codigoSistema,
                    ID_ROL = idRol,
                    ID_MENU = idMenu
                };

                var json = new PassportAes().MessagePassport(bootReponse.Data.Token, rolAuth);

                var client = _httpClient.CreateClient("AccionDataService");
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("Authorization", headerAuth);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var accionResponse = await client.PostAsync($"", content);

                if (accionResponse.IsSuccessStatusCode)
                {
                    var data = await accionResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var result = new PassportAes().ResultPassport<GenericResponse<List<AccionAuth>>>(data);

                        if (!result.HasErrors)
                        {
                            var acciones = new List<Accion>();
                            foreach (var item in result.Data)
                            {
                                if (!string.IsNullOrEmpty(item.NOMBRE_PERMISO))
                                {
                                    var accion = new Accion();
                                    accion.IdPermiso = item.ID_PERMISO;
                                    accion.NombrePermiso = item.NOMBRE_PERMISO.Trim().ToLower();
                                    acciones.Add(accion);
                                }
                            }

                            response.Data = acciones;

                        }
                        else
                        {
                            response.Success = false;
                            if (result.Messages[0].Message.Trim() == Message.ERROR_NOT_EXIST_SESSION.Trim() ||
                                result.Messages[0].Message.Trim() == Message.ERROR_SESSION_EXPIRED.Trim())
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.ERROR_UNAUTHORIZED));
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, result.Messages[0].Message));
                            }

                        }
                    }
                    catch (Exception)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error al realizar el desencriptado de Acciones"));
                        response.Success = false;
                        return response;
                    }
                }
                else
                {
                    response.Success = false;
                    if (accionResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_UNAUTHORIZED));
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_AUTH));
                    }

                }

            }
            catch (Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, $" Api de Acciones: {Message.ERROR_SERVICE_REFIT}"));
                response.Success = false;
            }

            return response;

        }

        public async Task<StatusApiResponse<List<Menu>>> GetMenus(string codigoSistema, string codigoModulo, string headerAuth)
        {
            var response = new StatusApiResponse<List<Menu>>();

            try
            {
                var bootReponse = await GetBoot();
                if (!bootReponse.Success)
                {
                    response.Messages.Add(new GenericMessage(bootReponse.Messages[0].Type, bootReponse.Messages[0].Message));
                    response.Success = false;
                    return response;
                }

                var menuAuth = new MenuAuth()
                {
                    CODIGO_SISTEMA = codigoSistema,
                    ID_ROL = null
                };

                var json = new PassportAes().MessagePassport(bootReponse.Data.Token, menuAuth);

                var client = _httpClient.CreateClient("MenuDataService");
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("Authorization", headerAuth);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var menuResponse = await client.PostAsync($"", content);

                if (menuResponse.IsSuccessStatusCode)
                {
                    var data = await menuResponse.Content.ReadAsStringAsync();

                    try
                    {
                        var result = new PassportAes().ResultPassport<GenericResponse<List<MenuAuth>>>(data);
                        var menus = new List<Menu>();

                        if (!result.HasErrors)
                        {
                            if (String.IsNullOrEmpty(codigoModulo))
                                codigoModulo = "000000";

                            var modulo = result.Data.Where(x => x.CODIGO == codigoModulo).FirstOrDefault();
                            foreach (var item in result.Data)
                            {
                                var codigoMod = codigoModulo.Substring(0, 2);
                                var codigo = item.CODIGO.Substring(0, 2);
                                var menu = new Menu();
                                if (codigo == codigoMod && item.ID_MENU_PADRE > 0)
                                {
                                    if (modulo.ID_MENU == item.ID_MENU_PADRE)
                                    {
                                        item.ID_MENU_PADRE = 0;
                                    }
                                    menu.IdMenu = item.ID_MENU;
                                    menu.Codigo = item.CODIGO;
                                    menu.NombreMenu = item.NOMBRE_MENU;
                                    menu.NombreIcono = item.NOMBRE_ICONO;
                                    menu.OrdenMenu = item.ORDEN_MENU;
                                    menu.IdMenuPadre = item.ID_MENU_PADRE;
                                    menu.Url = item.URL;
                                    menu.TipoOpcion = item.TIPO_OPCION;
                                    menus.Add(menu);
                                }
                            }

                            foreach (var item in menus)
                            {
                                var count = menus.Count(x => x.IdMenuPadre == item.IdMenu);
                                item.TotalChildren = count;
                            }

                            response.Data = menus;

                        }
                        else
                        {
                            response.Success = false;
                            if (result.Messages[0].Message.Trim() == Message.ERROR_NOT_EXIST_SESSION.Trim() ||
                                result.Messages[0].Message.Trim() == Message.ERROR_SESSION_EXPIRED.Trim())
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.ERROR_UNAUTHORIZED));
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, result.Messages[0].Message));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error al realizar el desencriptado de Menus"));
                        response.Success = false;
                    }

                }
                else
                {
                    response.Success = false;
                    if (menuResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_UNAUTHORIZED));
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_AUTH));
                    }
                }

            }
            catch (Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, $"Api de Menus: {Message.ERROR_SERVICE_REFIT}"));
                response.Success = false;
            }

            return response;
        }

        public async Task<StatusApiResponse<List<Modulo>>> GetModulos(string codigoSistema, string headerAuth)
        {
            var response = new StatusApiResponse<List<Modulo>>();

            try
            {
                var bootReponse = await GetBoot();
                if (!bootReponse.Success)
                {
                    response.Messages.Add(new GenericMessage(bootReponse.Messages[0].Type, bootReponse.Messages[0].Message));
                    response.Success = false;
                    return response;
                }

                var moduloAuth = new ModuloAuth()
                {
                    CODIGO_SISTEMA = codigoSistema,
                    ID_ROL = null
                };

                var json = new PassportAes().MessagePassport(bootReponse.Data.Token, moduloAuth);

                var client = _httpClient.CreateClient("MenuDataService");
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("Authorization", headerAuth);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var moduloResponse = await client.PostAsync($"", content);

                if (moduloResponse.IsSuccessStatusCode)
                {
                    var data = await moduloResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var result = new PassportAes().ResultPassport<GenericResponse<List<MenuAuth>>>(data);
                        var modulos = new List<Modulo>();
                        if (!result.HasErrors)
                        {
                            foreach (var item in result.Data)
                            {
                                if (item.ID_MENU_PADRE == 0)
                                {
                                    var modulo = new Modulo();
                                    modulo.IdModulo = item.ID_MENU;
                                    modulo.Codigo = item.CODIGO;
                                    modulo.NombreModulo = item.NOMBRE_MENU;
                                    modulo.NombreIcono = item.NOMBRE_ICONO;
                                    modulo.Orden = item.ORDEN_MENU;
                                    modulo.Url = item.URL;
                                    modulo.TipoOpcion = item.TIPO_OPCION;
                                    modulos.Add(modulo);
                                }
                            }

                            response.Data = modulos;
                        }
                        else
                        {
                            response.Success = false;
                            if (result.Messages[0].Message.Trim() == Message.ERROR_NOT_EXIST_SESSION.Trim() ||
                                result.Messages[0].Message.Trim() == Message.ERROR_SESSION_EXPIRED.Trim())
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.ERROR_UNAUTHORIZED));
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, result.Messages[0].Message));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error al realizar el desencriptado de Modulos"));
                        response.Success = false;
                    }
                }
                else
                {
                    response.Success = false;
                    if (moduloResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_UNAUTHORIZED));
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_AUTH));
                    }

                }
            }
            catch (Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, $"Api de Modulos: {Message.ERROR_SERVICE_REFIT}"));
                response.Success = false;
            }


            return response;
        }

        public async Task<StatusApiResponse<List<Rol>>> GetRoles(string codigoSistema, string headerAuth)
        {
            var response = new StatusApiResponse<List<Rol>>();

            try
            {

                var bootReponse = await GetBoot();
                if (!bootReponse.Success)
                {
                    response.Messages.Add(new GenericMessage(bootReponse.Messages[0].Type, bootReponse.Messages[0].Message));
                    response.Success = false;
                    return response;
                }

                var rolAuth = new RolAuth()
                {
                    CODIGO_SISTEMA = codigoSistema
                };

                var json = new PassportAes().MessagePassport(bootReponse.Data.Token, rolAuth);

                var client = _httpClient.CreateClient("RolUsuarioDataService");
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("Authorization", headerAuth);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var rolResponse = await client.PostAsync($"", content);

                if (rolResponse.IsSuccessStatusCode)
                {
                    var data = await rolResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var result = new PassportAes().ResultPassport<GenericResponse<List<RolAuth>>>(data);
                        if (!result.HasErrors)
                        {
                            List<Rol> roles = new List<Rol>();
                            foreach (var item in result.Data)
                            {
                                var rol = new Rol();
                                rol.IdRol = item.ID_ROL;
                                rol.Codigo = item.CODIGO_ROL;
                                rol.NombreRol = item.NOMBRE_ROL;
                                rol.IdSede = item.ID_SEDE;
                                rol.NombreSede = item.NOMBRE_SEDE;
                                rol.IdTipoSede = item.ID_TIPO_SEDE;
                                rol.porDefecto = item.POR_DEFECTO;
                                roles.Add(rol);
                            }

                            response.Data = roles;
                        }
                        else
                        {
                            response.Success = false;
                            if (result.Messages[0].Message.Trim() == Message.ERROR_NOT_EXIST_SESSION.Trim() ||
                                result.Messages[0].Message.Trim() == Message.ERROR_SESSION_EXPIRED.Trim())
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.ERROR_UNAUTHORIZED));
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, result.Messages[0].Message));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error al realizar el desencriptado de Roles"));
                        response.Success = false;
                    }
                }
                else
                {
                    response.Success = false;
                    if (rolResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_UNAUTHORIZED));
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_AUTH));
                    }

                }

            }
            catch (System.Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, $"Api de Roles: {Message.ERROR_SERVICE_REFIT}"));
                response.Success = false;
            }


            return response;
        }

        public async Task<StatusApiResponse<Sesion>> GetSesion(string codigoSistema, string headerAuth)
        {
            var response = new StatusApiResponse<Sesion>();

            try
            {

                var bootReponse = await GetBoot();
                if (!bootReponse.Success)
                {
                    response.Messages.Add(new GenericMessage(bootReponse.Messages[0].Type, bootReponse.Messages[0].Message));
                    response.Success = false;
                    return response;
                }

                var usuarioAuth = new UsuarioAuth()
                {
                    CODIGO_SISTEMA = codigoSistema
                };

                var json = new PassportAes().MessagePassport(bootReponse.Data.Token, usuarioAuth);
                var client = _httpClient.CreateClient("SesionDataService");
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("Authorization", headerAuth);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var sesionResponse = await client.PostAsync($"", content);

                if (sesionResponse.IsSuccessStatusCode)
                {
                    var data = await sesionResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var result = new PassportAes().ResultPassport<GenericResponse<List<SesionAuth>>>(data);
                        var sesion = new Sesion();
                        if (!result.HasErrors)
                        {
                            sesion.FechaCaducidad = result.Data[0].FECHA_CADUCIDAD;
                            sesion.FechaCreacion = result.Data[0].FECHA_CREACION;
                            sesion.FechaUltimaSesion = result.Data[0].FECHA_ULTIMA_SESION;
                            response.Success = true;
                            response.Data = sesion;
                        }
                        else
                        {
                            response.Success = false;
                            if (result.Messages[0].Message.Trim() == Message.ERROR_NOT_EXIST_SESSION.Trim() ||
                                result.Messages[0].Message.Trim() == Message.ERROR_SESSION_EXPIRED.Trim())
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.ERROR_UNAUTHORIZED));
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, result.Messages[0].Message));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error al realizar el desencriptado de Sesion"));
                        response.Success = false;
                    }
                }
                else
                {
                    response.Success = false;
                    if (sesionResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_UNAUTHORIZED));
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_AUTH));
                    }

                }
            }
            catch (System.Exception)
            {
                response.Success = false;
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, $"Api de Sesion: {Message.ERROR_SERVICE_REFIT}"));
            }

            return response;
        }

        public async Task<StatusApiResponse<Usuario>> GetUsuario(string codigoSistema, string headerAuth)
        {
            var response = new StatusApiResponse<Usuario>();

            try
            {
                var bootReponse = await GetBoot();
                if (!bootReponse.Success)
                {
                    response.Messages.Add(new GenericMessage(bootReponse.Messages[0].Type, bootReponse.Messages[0].Message));
                    response.Success = false;
                    return response;
                }

                var usuarioAuth = new UsuarioAuth()
                {
                    CODIGO_SISTEMA = codigoSistema
                };

                var json = new PassportAes().MessagePassport(bootReponse.Data.Token, usuarioAuth);
                var client = _httpClient.CreateClient("UsuarioDataService");
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("Authorization", headerAuth);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var usuarioResponse = await client.PostAsync($"", content);

                if (usuarioResponse.IsSuccessStatusCode)
                {
                    var data = await usuarioResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var result = new PassportAes().ResultPassport<GenericResponse<List<UsuarioAuth>>>(data);

                        if (!result.HasErrors)
                        {
                            var usuario = new Usuario();
                            usuario.Nombre = result.Data[0].NOMBRES_USUARIO;
                            usuario.ApellidoPaterno = result.Data[0].APELLIDO_PATERNO;
                            usuario.ApellidoMaterno = result.Data[0].APELLIDO_MATERNO;
                            usuario.IdTipoDocumento = result.Data[0].ID_TIPO_DOCUMENTO_ENUM;
                            usuario.NombreTipoDocumento = result.Data[0].TIPO_DOCUMENTO_ENUM;
                            usuario.NumeroDocumento = result.Data[0].NUMERO_DOCUMENTO;
                            usuario.FechaNacimiento = result.Data[0].FECHA_NACIMIENTO;
                            usuario.Correo = result.Data[0].CORREO_USUARIO;

                            var nombreCompleto = $"{usuario.Nombre} {usuario.ApellidoPaterno}";
                            if (!String.IsNullOrEmpty(usuario.ApellidoMaterno))
                            {
                                nombreCompleto = $"{usuario.Nombre} {usuario.ApellidoPaterno} {usuario.ApellidoMaterno}";
                            }
                            usuario.NombreCompleto = nombreCompleto;
                            response.Data = usuario;
                            response.Success = true;
                        }
                        else
                        {
                            response.Success = false;
                            if (result.Messages[0].Message.Trim() == Message.ERROR_NOT_EXIST_SESSION.Trim() ||
                                result.Messages[0].Message.Trim() == Message.ERROR_SESSION_EXPIRED.Trim())
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.ERROR_UNAUTHORIZED));
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, result.Messages[0].Message));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error al realizar el desencriptado de Usuario"));
                        response.Success = false;
                    }
                }
                else
                {
                    response.Success = false;
                    if (usuarioResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_UNAUTHORIZED));
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_AUTH));
                    }

                }
            }
            catch (Exception)
            {
                response.Success = false;
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, $"Api de Usuario: {Message.ERROR_SERVICE_REFIT}"));
            }

            return response;
        }
    }
}