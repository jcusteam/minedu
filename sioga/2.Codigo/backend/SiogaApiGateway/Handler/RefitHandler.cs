using Microsoft.Extensions.Logging;
using SiogaApiGateway.Helpers;
using SiogaUtils;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SiogaApiGateway.Handler
{

    public class RefitHandler : DelegatingHandler
    {
        private readonly ILogger<RefitHandler> _logger;

        public RefitHandler(ILogger<RefitHandler> logger)
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
                    return ResponseMessage.ErrorApi(Message.ERROR_SERVICE_AUTH);
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Error {e.Message}");

                return ResponseMessage.ErrorApi(Message.ERROR_SERVICE_REFIT);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }


}
