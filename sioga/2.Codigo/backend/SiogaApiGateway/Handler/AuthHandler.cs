using System.Diagnostics;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SiogaUtils;
using SiogaApiGateway.Helpers;
using Newtonsoft.Json;

namespace SiogaApiGateway.Handler
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly ILogger<AuthHandler> _logger;
        private readonly AppSettings _appSettings;
        public AuthHandler(
            AppSettings appSettings,
            ILogger<AuthHandler> logger)
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

            try
            {
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

                if (!apiResponse.Success)
                    return ResponseMessage.ErrorAuth(apiResponse.Messages[0].Message);

                if (apiResponse.Data != null)
                {
                    // Encript Data
                    string jsonData = JsonConvert.SerializeObject(apiResponse.Data);
                    apiResponse.Data = aes.Encrypt(jsonData, key);
                }

                return ResponseMessage.Ok(apiResponse);
            }

            _logger.LogInformation($"Time start in {time.ElapsedMilliseconds}ms");

            return response;
        }
    }

}
