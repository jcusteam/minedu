using System.Linq;
using System.Net.Http;

namespace SiogaApiGateway.Helpers
{
    public static class Extensions
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        public static string GetHeaderContent(this HttpRequestMessage request, string key)
        {
            return request.Content.Headers.FirstOrDefault(x => x.Key.ToLower().Trim() == key.ToLower().Trim()).Value.FirstOrDefault();
        }

        public static string GetHeaderContent(this HttpResponseMessage response, string key)
        {
            return response.Content.Headers.FirstOrDefault(x => x.Key.ToLower().Trim() == key.ToLower().Trim()).Value.FirstOrDefault();
        }

        public static string GetAuthorization(this HttpRequestMessage request)
        {
            try
            {
                return request.Headers.Authorization.ToString();
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public static string GetAuthToken(this HttpRequestMessage request)
        {
            try
            {
                return request.Headers.FirstOrDefault(x => x.Key.ToLower().Trim() == "auth-token").Value.FirstOrDefault();
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public static string GetOrigin(this HttpRequestMessage request, string key)
        {
            try
            {
                return request.Headers.FirstOrDefault(x => x.Key.ToLower().Trim() == key.ToLower().Trim()).Value.FirstOrDefault();
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            try
            {
                if (request.Properties.ContainsKey(HttpContext))
                {
                    dynamic ctx = request.Properties[HttpContext];
                    if (ctx != null)
                    {
                        return ctx.Request.UserHostAddress;
                    }
                }

                if (request.Properties.ContainsKey(RemoteEndpointMessage))
                {
                    dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                    if (remoteEndpoint != null)
                    {
                        return remoteEndpoint.Address;
                    }
                }
            }
            catch (System.Exception)
            {

                return null;
            }


            return null;
        }
    }
}
