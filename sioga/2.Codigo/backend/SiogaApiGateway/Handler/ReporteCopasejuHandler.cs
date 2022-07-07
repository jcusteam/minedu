using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SiogaUtils;
using SiogaApiGateway.Helpers;

namespace SiogaApiGateway.Handler
{
    public class ReporteCopasejuHandler : DelegatingHandler
    {
        private readonly ILogger<ReporteCopasejuHandler> _logger;
        private readonly AppSettings _appSettings;
        public ReporteCopasejuHandler(
            AppSettings appSettings,
            ILogger<ReporteCopasejuHandler> logger)
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

            _logger.LogInformation($"Time start in {time.ElapsedMilliseconds}ms");

            return response;
        }
    }
}
