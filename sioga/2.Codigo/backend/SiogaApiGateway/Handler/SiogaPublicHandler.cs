using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SiogaUtils;
using SiogaApiGateway.Helpers;
using Newtonsoft.Json;

namespace SiogaApiGateway.Handler
{
    public class SiogaPublicHandler : DelegatingHandler
    {
        private readonly ILogger<SiogaPublicHandler> _logger;
        private readonly AppSettings _appSettings;
        public SiogaPublicHandler(
            AppSettings appSettings,
            ILogger<SiogaPublicHandler> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start request");
            var key = _appSettings.SecretKeyAES + "SISSIOGA";
            var aes = new AES256();

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
                return ResponseMessage.Error(Message.ERROR_SERVICE);
            }

        }
    }

}
