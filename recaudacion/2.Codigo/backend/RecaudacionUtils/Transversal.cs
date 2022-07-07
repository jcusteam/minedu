using System.Collections.Generic;
using System.Linq;

namespace RecaudacionUtils
{
    public static class Transversal
    {
        public static List<Combobox<string>> FindAllTipoRegRetenciones()
        {
            var list = new List<Combobox<string>>();
            list.Add(new Combobox<string>("01", "Tasa 3%"));
            list.Add(new Combobox<string>("02", "Tasa 3%"));
            return list;
        }

        public static Combobox<string> FindByValueTipoRegRetencion(string value)
        {
            var combobox = FindAllTipoRegRetenciones().Where(x => x.Value == value).FirstOrDefault();
            return combobox;
        }

    }

}
