using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace RecaudacionUtils
{
    public static class Tools
    {
        public static string ContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();


            return types[ext];
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
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".cer","application/pkix-cert"},
                {".key","application/pkcs8 "},
            };
        }

        public static string GetMonthName(int data)
        {
            var name = "";
            switch (data)
            {
                case 1:
                    name = "enero";
                    break;
                case 2:
                    name = "febreo";
                    break;
                case 3:
                    name = "marzo";
                    break;
                case 4:
                    name = "abril";
                    break;
                case 5:
                    name = "mayo";
                    break;
                case 6:
                    name = "junio";
                    break;
                case 7:
                    name = "julio";
                    break;
                case 8:
                    name = "agosto";
                    break;
                case 9:
                    name = "septiembre";
                    break;
                case 10:
                    name = "octubre";
                    break;
                case 11:
                    name = "noviembre";
                    break;
                case 12:
                    name = "diciembre";
                    break;
                default:
                    name = "-";
                    break;
            }

            return name;
        }

        public static string Replace(string a, string b, string c)
        {
            if ((a == null) || (b == null) || (c == null))
                return a;

            return a.Replace(b, c);
        }

        public static string ToUpper(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";

            return data.Trim().ToUpper();
        }

        public static string ToLower(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";

            return data.Trim().ToUpper();
        }

        public static string reclaceIsNullOrEmpty(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";

            return data.Trim();
        }

        // Base Precio Sin IGV
        public static decimal basePrecio(decimal total, decimal igv)
        {
            try
            {
                return total / (1 + igv);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        // Base Total
        public static decimal baseTotal(decimal total, decimal igv)
        {
            try
            {
                return total / (1 + igv);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        // Item IGV Total
        public static decimal ItemTotalIGV(decimal total, decimal baseTotal)
        {
            try
            {
                return total - baseTotal;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public static string ToFormat(string cadena)
        {
            string original = cadena;

            if (cadena.Length > 0)
                return String.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:0,0.00}", Convert.ToDouble(cadena));

            return original;
        }
    }

}
