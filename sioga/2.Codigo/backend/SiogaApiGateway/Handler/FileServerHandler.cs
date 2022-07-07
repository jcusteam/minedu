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
    public class FileServerHandler : DelegatingHandler
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<FileServerHandler> _logger;
        public FileServerHandler(
            AppSettings appSettings,
            ILogger<FileServerHandler> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            _logger.LogInformation("Start request");
            var aes = new AES256();
            var key = _appSettings.SecretKeyAES + "SISSIOGA";

            var authToken = Extensions.GetAuthToken(request);

            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var contentType = Extensions.GetHeaderContent(response, "Content-Type");
                if (contentType.Contains("application/json"))
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
