using System.Diagnostics;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SiogaUtils;
using SiogaApiGateway.Helpers;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Configuration;
using SiogaApiGateway.Service.Contracts;
using System.Net;
using Newtonsoft.Json.Serialization;

namespace SiogaApiGateway.Handler
{
    public class SiogaHandler : DelegatingHandler
    {
        private readonly ILogger<SiogaHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IAutorizationService _autorizationService;
        public SiogaHandler(
            AppSettings appSettings,
            ILogger<SiogaHandler> logger,
            IAutorizationService autorizationService)
        {
            _appSettings = appSettings;
            _logger = logger;
            _autorizationService = autorizationService;
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
                var headerAuth = request.Headers.Authorization.ToString();
                var dataAuth = new DataAuth();
                dataAuth.CodigoSistema = codeSistema;

                var authResponse = await _autorizationService.GetUsuario(dataAuth, headerAuth);

                if (!authResponse.Success)
                    return ResponseMessage.ErrorAuth(authResponse.Messages[0].Message);

                if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                {
                    // Decript Data
                    var body = await request.Content.ReadAsStringAsync();
                    var dataModel = JsonConvert.DeserializeObject<DataModel>(body);
                    if (dataModel.data != null)
                    {
                        var decriptData = aes.Decrypt(dataModel.data, key);
                        request.Content = RequestContent.ContentString(decriptData);
                    }
                    else
                    {
                        request.Content = RequestContent.ContentString();
                    }

                }
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Error..... {e.Message}");
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
