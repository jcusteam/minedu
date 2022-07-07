using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SiogaUtils;

namespace SiogaApiPublic.Helpers
{
    public static class ResponseMessage
    {
        public static HttpResponseMessage Error(string message)
        {
            var result = new StatusApiResponse<object>();
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
    }

}
