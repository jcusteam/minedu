using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SiogaUtils;

namespace SiogaApiGateway.Helpers
{
    public static class RequestContent
    {
        public static StringContent DynamicContentString(dynamic obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            return new StringContent(content, System.Text.Encoding.UTF8, "application/json");
        }

        public static StringContent ContentString(string data)
        {
            var resultData = JsonConvert.DeserializeObject<object>(data);
            var content = JsonConvert.SerializeObject(resultData);
            return new StringContent(content, System.Text.Encoding.UTF8, "application/json");
        }

        public static StringContent ContentString()
        {
            var content = JsonConvert.SerializeObject(new Object());
            return new StringContent(content, System.Text.Encoding.UTF8, "application/json");
        }
    }

}
