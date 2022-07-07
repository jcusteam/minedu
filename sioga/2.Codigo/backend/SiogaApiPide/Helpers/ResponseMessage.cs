using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SiogaApiPide.Clients.Response;

namespace SiogaApiPide.Helpers
{
    public static class ResponseMessage
    {
        public static HttpResponseMessage ErrorReniec(string message)
        {
            var result = new ReniecResponse();
            result.messages.Add(message);
            result.success = false;
            var responseMessage = new HttpResponseMessage();
            var content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            responseMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            return responseMessage;
        }

        public static HttpResponseMessage ErrorMigracion(string message)
        {
            var result = new MigracionResponse();
            result.message = message;
            result.success = false;
            var responseMessage = new HttpResponseMessage();
            var content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            responseMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            return responseMessage;
        }

        public static HttpResponseMessage ErrorSunat(string message)
        {
            var result = new SunatResponse();
            result.message = message;
            result.success = false;
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
    }

}
