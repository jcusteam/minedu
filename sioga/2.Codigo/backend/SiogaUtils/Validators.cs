using System.Text.RegularExpressions;

namespace SiogaUtils
{
    public static class Validators
    {
        public static bool IsDNI(string dni)
        {

            if (string.IsNullOrEmpty(dni))
            {
                return false;
            }

            if (dni.Length != 8 || Regex.Replace(dni, @"[0-9]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsRUC(string ruc)
        {

            if (string.IsNullOrEmpty(ruc))
            {
                return false;
            }

            if (ruc.Length != 11 || Regex.Replace(ruc, @"[0-9]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsCE(string numero)
        {

            if (string.IsNullOrEmpty(numero))
            {
                return false;
            }

            if ((numero.Length > 12 || numero.Length < 8) || Regex.Replace(numero, @"[a-zA-Z0-9]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }
    }
}
