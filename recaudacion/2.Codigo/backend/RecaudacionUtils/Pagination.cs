using System.Collections.Generic;

namespace RecaudacionUtils
{
    public class Pagination<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
    }
}
