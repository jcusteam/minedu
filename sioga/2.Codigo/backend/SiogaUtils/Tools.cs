using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SiogaUtils
{
    public static class Tools
    {
        public static string DatetimeToUnixTimeStamp()
        {
            long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string hexValue = unixTime.ToString("X2");
            return hexValue;
        }

        public static DateTime ToUnixTimeStampDatetime(string hexValue)
        {
            int secondsAfterEpoch = Int32.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            DateTime epoch = new DateTime(1970, 1, 1);
            DateTime myDateTime = epoch.ToLocalTime().AddSeconds(secondsAfterEpoch);
            return myDateTime;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static bool CompareWith(DateTime dt1, DateTime dt2)
        {
            return
                //dt1.Second == dt2.Second && // 1 of 60 match chance
                dt1.Minute == dt2.Minute && // 1 of 60 chance
                dt1.Day == dt2.Day &&       // 1 of 28-31 chance
                dt1.Hour == dt2.Hour &&     // 1 of 24 chance
                dt1.Month == dt2.Month &&   // 1 of 12 chance
                dt1.Year == dt2.Year;       // depends on dataset
        }


        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".xml", "application/xml"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip", "application/zip"}
            };
        }
    }
}
