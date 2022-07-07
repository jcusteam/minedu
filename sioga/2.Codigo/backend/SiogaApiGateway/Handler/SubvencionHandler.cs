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
using System.Net.Http.Headers;
using System.Net;

namespace SiogaApiGateway.Handler
{
    public class SubvencionHandler : DelegatingHandler
    {
        private readonly ILogger<SubvencionHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IAutorizationService _autorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SubvencionHandler(
            AppSettings appSettings,
            ILogger<SubvencionHandler> logger,
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
                var authToken = Extensions.GetAuthToken(request);

                if (!string.IsNullOrEmpty(authToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                }

                var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                _logger.LogInformation("Client Ip Address :" + ipAddress);

                if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                {
                    var headerAuth = Extensions.GetAuthorization(request);

                    var jwtUserData = JwtToken.GetUserData(headerAuth, Definition.JWT_KEY);

                    if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                    {
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
                                obj.usuarioCreador = jwtUserData.Username;
                            if (request.Method == HttpMethod.Put)
                                obj.usuarioModificador = jwtUserData.Username;

                            _logger.LogInformation("Usuario Identificador " + jwtUserData.Username);

                            request.Content = RequestContent.DynamicContentString(obj);
                        }
                        else
                        {
                            request.Content = RequestContent.ContentString();
                        }
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
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return ResponseMessage.Error(Message.ERROR_USER_NOT_UNAUTHORIZED);
                }
                else
                {
                    return ResponseMessage.Error(Message.ERROR_SERVICE);
                }
            }

        }
    }
}
