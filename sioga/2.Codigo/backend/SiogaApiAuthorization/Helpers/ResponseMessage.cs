using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SiogaUtils;

namespace SiogaApiAuthorization.Helpers
{
    public static class ResponseMessage
    {

        public static HttpResponseMessage ErrorAuth(string message)
        {
            var result = new GenericResponse<object>();
            result.Messages.Add(new GenericMessagePass(GenericMessageType.Error, message));
            result.HasErrors = true;
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
