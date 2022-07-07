using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SiogaUtils;

namespace SiogaApiGateway.Helpers
{
    public static class ResponseMessage
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


        public static HttpResponseMessage Ok(StatusApiResponse<object> apiResponse)
        {
            var result = new StatusResponse<object>();
            result.Data = apiResponse.Data;
            result.Success = apiResponse.Success;
            foreach (var message in apiResponse.Messages)
            {
                result.Messages.Add(message.Message);
                result.MessageType = message.Type;
            }

            var responseMessage = new HttpResponseMessage();
            var content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            responseMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            return responseMessage;
        }

        public static HttpResponseMessage Error(string message)
        {
            var result = new StatusResponse<object>();
            result.Messages.Add(message);
            result.Success = false;
            var responseMessage = new HttpResponseMessage();
            var content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            responseMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            return responseMessage;
        }


        public static HttpResponseMessage ErrorAuth(string message)
        {

            if (message == Message.ERROR_UNAUTHORIZED)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            var result = new StatusResponse<object>();
            result.Messages.Add(message);
            result.Success = false;
            var responseMessage = new HttpResponseMessage();
            var content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            responseMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            return responseMessage;
        }

        public static HttpResponseMessage ErrorApi(string message)
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
