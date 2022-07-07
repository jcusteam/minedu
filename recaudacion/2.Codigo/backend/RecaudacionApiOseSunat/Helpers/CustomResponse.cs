using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RecaudacionUtils;

namespace RecaudacionApiOseSunat.Helpers
{
    public static class CustomResponse
    {
        public static HttpResponseMessage Ok(StatusResponse<object> response)
        {
            var responseMessage = new HttpResponseMessage();
            var content = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            responseMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            return responseMessage;
        }

        public static HttpResponseMessage Error(string message)
        {
            var result = new StatusResponse<object>();
            result.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, message));
            result.Success = false;
            var responseMessage = new HttpResponseMessage();
            var content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            responseMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            return responseMessage;
        }

        public static HttpResponseMessage Unauthorized()
        {
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }


        public async static Task<HttpResponse> NotFound(this HttpResponse response)
        {
            var result = new StatusResponse<object>();
            result.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_NOT_FOUND));
            result.Success = false;
            var content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            response.ContentType = "application/json";
            await response.WriteAsync(content);

            return response;
        }

    }

}
