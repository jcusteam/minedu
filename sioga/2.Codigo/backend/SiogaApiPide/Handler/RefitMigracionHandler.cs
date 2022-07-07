using Microsoft.Extensions.Logging;
using SiogaApiPide.Helpers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SiogaUtils;

namespace SiogaApiPide.Handler
{

    public class RefitMigracionHandler : DelegatingHandler
    {
        private readonly ILogger<RefitMigracionHandler> _logger;

        public RefitMigracionHandler(ILogger<RefitMigracionHandler> logger)
        {
            _logger = logger;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return ResponseMessage.ErrorMigracion(Message.ERROR_SERVICE);
                }
                return response;
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Error {e.Message}");

                return ResponseMessage.ErrorMigracion(Message.ERROR_SERVICE_REFIT);
            }

        }
    }


}
