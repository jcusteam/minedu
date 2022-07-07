using Microsoft.Extensions.Logging;
using RecaudacionApiReciboIngreso.Helpers;
using RecaudacionUtils;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RecaudacionApiReciboIngreso.Handler
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
                    return CustomResponse.Error(Message.ERROR_SERVICE);
                }

                return response;
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Error {e.Message}");

                return CustomResponse.Error(Message.ERROR_SERVICE_REFIT);
            }
        }
    }


}
