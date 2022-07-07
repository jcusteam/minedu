using System.Collections.Generic;
using System.Linq;

namespace RecaudacionUtils
{
    public class Combobox<T>
    {
        public T Value { get; set; }
        public string Label { get; set; }

        public Combobox(T value, string label)
        {
            Value = value;
            Label = label;
        }
    }
}
