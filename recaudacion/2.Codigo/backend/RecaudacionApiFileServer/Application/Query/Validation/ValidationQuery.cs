using System.Text.RegularExpressions;

namespace RecaudacionApiFileServer.Application.Query.Validation
{
    public class ValidationQuery
    {
        // Validation Subdirectory
        public static bool Subdirectory(string subdirectory)
        {

            if (string.IsNullOrEmpty(subdirectory))
            {
                return false;
            }

            if (Regex.Replace(subdirectory, @"[A-Za-z0-9_-]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }

        public static bool FileName(string fileName)
        {

            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            if (Regex.Replace(fileName, @"[A-Za-z0-9_.-]", string.Empty).Length > 0)
            {
                return false;
            }

            return true;
        }
    }

}
