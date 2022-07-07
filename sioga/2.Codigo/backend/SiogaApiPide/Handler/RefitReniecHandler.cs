using Microsoft.Extensions.Logging;
using SiogaApiPide.Helpers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SiogaUtils;

namespace SiogaApiPide.Handler
{

    public class RefitReniecHandler : DelegatingHandler
    {
        private readonly ILogger<RefitReniecHandler> _logger;

        public RefitReniecHandler(ILogger<RefitReniecHandler> logger)
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
                    return ResponseMessage.ErrorReniec(Message.ERROR_SERVICE);
                }
                return response;
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Error {e.Message}");

                return ResponseMessage.ErrorReniec(Message.ERROR_SERVICE_REFIT);
            }

        }
    }


}
