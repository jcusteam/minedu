using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
namespace SiogaApiAuthorization.Helpers
{
    public class PassportAes
    {
        public string DecryptStringAES(string cipherText, string key, bool decode = false)
        {
            byte[] bytes1 = Encoding.UTF8.GetBytes(key);
            byte[] bytes2 = Encoding.UTF8.GetBytes(key);
            if (decode)
                cipherText = WebUtility.UrlDecode(cipherText);
            return this.DecryptStringFromBytes(Convert.FromBase64String(cipherText), bytes1, bytes2);
        }
        private string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length == 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length == 0)
                throw new ArgumentNullException(nameof(key));
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.FeedbackSize = 128;
                rijndaelManaged.Key = key;
                rijndaelManaged.IV = iv;
                ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream(cipherText))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                                return streamReader.ReadToEnd();
                        }
                    }
                }
                catch
                {
                    return "";
                }
            }
        }
        public string EncryptStringAES(string plainText, string key, bool encode = true)
        {
            byte[] bytes1 = Encoding.UTF8.GetBytes(key);
            byte[] bytes2 = Encoding.UTF8.GetBytes(key);
            string str = Convert.ToBase64String(this.EncryptStringToBytes(plainText, bytes1, bytes2));
            if (encode)
                str = WebUtility.UrlEncode(str);
            return str;
        }
        private byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length == 0)
                throw new ArgumentNullException(nameof(key));
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.FeedbackSize = 128;
                rijndaelManaged.Key = key;
                rijndaelManaged.IV = iv;
                ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            streamWriter.Write(plainText);
                        return memoryStream.ToArray();
                    }
                }
            }
        }
        public LLaves LetraToNumero(string r)
        {
            var e = new string[] { "xZxS%jqm", "nr%Ft1Jr", "60Vc%UNh", "6e9hv9%M", "K%NZThUV", "JT%WG5aU", "hn8q%xb4", "QO1%qim9", "EjuRBck%", "eX1%P2Gd" };

            //var a = new {
            //    'a' = 0,
            //    'B' = 1,
            //    'X' = 2,
            //    'D' = 3,
            //    'e' = 4,
            //    '%' = 5,
            //    'g' = 6,
            //    'H' = 7,
            //    'i' = 8,
            //    'Z' = 9
            //};

            var a = new Dictionary<char, int>();

            a.Add('a', 0);
            a.Add('B', 1);
            a.Add('X', 2);
            a.Add('D', 3);
            a.Add('e', 4);

            a.Add('%', 5);
            a.Add('g', 6);
            a.Add('H', 7);

            a.Add('i', 8);
            a.Add('Z', 9);


            var separetor = "";
            var pos = -1;

            int t = e.Length;

            for (int o = 0; t > o; o++)
            {
                var p = e[o];
                pos = r.LastIndexOf(p);

                if (pos > 1)
                {
                    separetor = p;
                    break;
                }
            }

            var n = r.Substring(pos + separetor.Length, 16);
            var v = n.ToArray();
            var f = new List<string>();

            int g = v.Length;

            for (int u = 0; g > u; u++)
            {
                f.Add(a[v[u]].ToString());
            }

            var h = string.Join("", f.ToArray());

            return new LLaves
            {
                Separetor = separetor,
                Key1 = n,
                Key2 = h
            };
        }
        public string MessagePassport(string codigoUnicoSesion, object value)
        {
            var data = EncryptStringAES(JsonConvert.SerializeObject(value), codigoUnicoSesion, true);
            var dataRequest = new { key = codigoUnicoSesion, data = data };
            string json = JsonConvert.SerializeObject(dataRequest);
            return json;
        }
        public T ResultPassport<T>(string param) where T : new()
        {
            var jsonString = JsonConvert.DeserializeObject<RequestDataModel>(param);

            var llaves = LetraToNumero(jsonString.DataModel);
            var ndata = jsonString.DataModel.ToString().Replace(llaves.Separetor + llaves.Key1, "");
            var decodeData = DecryptStringAES(ndata, llaves.Key2, true);
            var dataAuth = JsonConvert.DeserializeObject<string>(decodeData);
            var response = JsonConvert.DeserializeObject<T>(dataAuth);
            return response;
        }

        public T ResultData<T>(string data) where T : new()
        {
            var dataAuth = JsonConvert.DeserializeObject<string>(data);
            var response = JsonConvert.DeserializeObject<T>(dataAuth);
            return response;
        }
    }

    public class LLaves
    {
        public string Separetor { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
    }

    public class RequestDataModel
    {
        public string DataModel { get; set; }
    }
}
