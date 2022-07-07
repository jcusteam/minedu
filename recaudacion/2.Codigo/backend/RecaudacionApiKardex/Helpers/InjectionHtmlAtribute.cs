using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RecaudacionApiKardex.Helpers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InjectionHtmlAtribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var item in context.ActionArguments)
            {
                PropertyInfo[] properties = item.Value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        string value = (string)property.GetValue(item.Value, null);

                        /* DATOS DE PRUEBA/
                            value = StripHTMLTags("<strong>texto en negrita</strong> prueba");
                            value = StripHTMLTags("prueba <script>alert('sdfd')</script>");
                            value = StripHTMLTags("prueba <style>body{ background-color: red;</style>");
                        */

                        if (!string.IsNullOrEmpty(value))
                        {
                            //property.SetValue(item.Value, StripHTMLTags(value));
                            property.SetValue(item.Value, StripTagsCharArray(value));
                        }
                    }
                }
            }
        }

        private static string StripHTMLTags(string inputstring)
        {
            /* MEJORAR ESTA FUNCIÓN PARA QUE ELIMINTE TAGS COMPLETOS DE HTML, CSS, SCRIPT */
            return Regex.Replace(inputstring, "<.*?>", String.Empty);
        }

        public static string StripTagsCharArray(string inputstring)
        {
            char[] array = new char[inputstring.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < inputstring.Length; i++)
            {
                char let = inputstring[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }


}
