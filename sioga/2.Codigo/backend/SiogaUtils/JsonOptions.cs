using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SiogaUtils
{
    public class JsonOptions<T>
    {
        public static T ToObjet(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (System.Exception)
            {
                return JsonConvert.DeserializeObject<T>("{}");
            }

        }

        public static string ToString(T data)
        {
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return json;
        }


        public static T ToDecript(string decript, string key)
        {
            try
            {
                var aes = new AES256();
                var data = aes.Decrypt(decript, key);
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (System.Exception)
            {
                return JsonConvert.DeserializeObject<T>("{}");
            }

        }

        public static string ToEncrypt(T data, string key)
        {

            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var aes = new AES256();
            var encript = aes.Encrypt(json, key);

            return encript;
        }

    }
}
