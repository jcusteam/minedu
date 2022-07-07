using System.Linq;
using Microsoft.AspNetCore.Http;

namespace SiogaApiAuthorization.Helpers
{
    public static class Extensions
    {
        public static string GetHeader(this HttpRequest request, string key)
        {
            return request.Headers.FirstOrDefault(x => x.Key == key).Value.FirstOrDefault();
        }

    }
}
