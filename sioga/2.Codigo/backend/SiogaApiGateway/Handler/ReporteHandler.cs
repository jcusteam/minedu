using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SiogaUtils;
using SiogaApiGateway.Helpers;
using System.Net.Http.Headers;
using System.Net;

namespace SiogaApiGateway.Handler
{
    public class ReporteHandler : DelegatingHandler
    {
        private readonly ILogger<ReporteHandler> _logger;
        private readonly AppSettings _appSettings;
        public ReporteHandler(
            AppSettings appSettings,
            ILogger<ReporteHandler> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            _logger.LogInformation("Start request");
            var key = _appSettings.SecretKeyAES + "SISSIOGA"; ;
            var aes = new AES256();

            try
            {

                var authToken = Extensions.GetAuthToken(request);

                if (!string.IsNullOrEmpty(authToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                }

                if (request.Method == HttpMethod.Post)
                {
                    // Decript Data
                    var body = await request.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<DataModel>(body);
                    if (result.data != null)
                    {
                        var decriptData = aes.Decrypt(result.data, key);
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
                _logger.LogError($"Error: {e.Message}");
                return ResponseMessage.Error(Message.ERROR_SERVICE_GATEWAY);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var contentType = Extensions.GetHeaderContent(response, "Content-Type");
                if (contentType.Contains("application/json"))
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<StatusApiResponse<object>>(body);
                    return ResponseMessage.Ok(apiResponse);
                }

                return response;
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
