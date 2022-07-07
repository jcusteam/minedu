using System.Diagnostics;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SiogaUtils;
using SiogaApiGateway.Helpers;
using Newtonsoft.Json;
using SiogaApiGateway.Service.Contracts;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace SiogaApiGateway.Handler
{
    public class RecaudacionHandler : DelegatingHandler
    {
        private readonly ILogger<RecaudacionHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IAutorizationService _autorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RecaudacionHandler(
            AppSettings appSettings,
            ILogger<RecaudacionHandler> logger,
            IAutorizationService autorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appSettings;
            _logger = logger;
            _autorizationService = autorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            _logger.LogInformation("Start request");
            var aes = new AES256();
            var key = _appSettings.SecretKeyAES + "SISSIOGA";
            try
            {
                var codeSistema = _appSettings.CodeSistema;
                var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                _logger.LogInformation("Client Ip Address :" + ipAddress);

                if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                {
                    // Obtener Token
                    var headerAuth = Extensions.GetAuthorization(request);
                    if (headerAuth == null)
                        return ResponseMessage.Unauthorized();

                    var dataAuth = new DataAuth();
                    dataAuth.CodigoSistema = codeSistema;

                    var authResponse = await _autorizationService.GetUsuario(dataAuth, headerAuth);

                    if (!authResponse.Success)
                        return ResponseMessage.ErrorAuth(authResponse.Messages[0].Message);

                    var usuario = authResponse.Data;
                    var modulos = usuario.Modulos;
                    var roles = usuario.Roles;

                    // Modulos App
                    var modulosCodeApp = _appSettings.Modulos;

                    // Roles App
                    var rolesRecaudacion = _appSettings.Rol.Recaudacion;

                    var rolesCodeApp = new List<string>();
                    rolesCodeApp.AddRange(rolesRecaudacion);

                    // Acceso Modulos
                    if (usuario.Modulos.Count == 0)
                        return ResponseMessage.Error(Message.ERROR_USER_NOT_UNAUTHORIZED);

                    if (!modulosCodeApp.Intersect(modulos).Any())
                        return ResponseMessage.Error(Message.ERROR_USER_NOT_UNAUTHORIZED);

                    // Acceso Roles
                    if (usuario.Roles.Count == 0)
                        return ResponseMessage.Error(Message.ERROR_USER_NOT_UNAUTHORIZED);

                    if (!rolesCodeApp.Intersect(roles.Intersect(rolesCodeApp)).Any())
                        return ResponseMessage.Error(Message.ERROR_USER_NOT_UNAUTHORIZED);


                    // Decript Data
                    var body = await request.Content.ReadAsStringAsync();
                    var dataModel = JsonConvert.DeserializeObject<DataModel>(body);
                    if (dataModel.data != null)
                    {
                        var decriptData = aes.Decrypt(dataModel.data, key);
                        dynamic obj = JsonConvert.DeserializeObject<object>(decriptData);
                        obj.usuarioCreador = "";
                        obj.usuarioModificador = "";
                        obj.ipAddress = ipAddress;

                        if (request.Method == HttpMethod.Post)
                            obj.usuarioCreador = authResponse.Data.NumeroDocumento;
                        if (request.Method == HttpMethod.Put)
                            obj.usuarioModificador = authResponse.Data.NumeroDocumento;

                        _logger.LogInformation("Usuario Identificador " + authResponse.Data.NumeroDocumento);

                        request.Content = RequestContent.DynamicContentString(obj);
                    }
                    else
                    {
                        request.Content = RequestContent.ContentString();
                    }
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Error {e.Message}");
                return ResponseMessage.Error(Message.ERROR_SERVICE_GATEWAY);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<StatusApiResponse<object>>(body);
                if (apiResponse.Success)
                {
                    if (apiResponse.Data != null)
                    {
                        // Encript Data
                        string jsonData = JsonConvert.SerializeObject(apiResponse.Data);
                        apiResponse.Data = aes.Encrypt(jsonData, key);
                    }
                }

                return ResponseMessage.Ok(apiResponse);
            }

            _logger.LogInformation($"Time start in {time.ElapsedMilliseconds}ms");

            return response;
        }
    }
}
