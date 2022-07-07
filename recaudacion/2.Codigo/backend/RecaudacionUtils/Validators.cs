using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace RecaudacionUtils
{
    public static class Validators
    {
        // Validation Numbers
        public static bool IsNumeric(string number)
        {

            if (string.IsNullOrEmpty(number))
            {
                return false;
            }

            if (Regex.Replace(number, @"[0-9]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }

        // Validation Alpha
        public static bool IsAlpha(string text)
        {

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (Regex.Replace(text, @"[a-zA-Z]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }

        // Validation Alpha and Numeric
        public static bool IsAlphaNumeric(string text)
        {

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (Regex.Replace(text, @"[a-zA-Z0-9]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }

        // Validation text feature
        public static bool IsTextFeature(string text, string match)
        {

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (Regex.Replace(text, $@"[{match}]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }

        // Validation DNI
        public static bool IsNroDNI(string dni)
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

        // Validation DNI
        public static bool IsNroRUC(string ruc)
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

        public static bool IsNroCE(string numero)
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

        public static bool IsMail(string email)
        {

            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }



    }

}
