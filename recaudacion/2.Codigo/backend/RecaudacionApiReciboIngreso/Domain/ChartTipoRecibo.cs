using System.Collections.Generic;

namespace RecaudacionApiReciboIngreso.Domain
{
    public class ChartTipoRecibo
    {
        public int TipoId { get; set; }
        public string TipoName { get; set; }
        public List<Chart> charts { get; set; }

        public ChartTipoRecibo()
        {
            charts = new List<Chart>();
        }
    }

    public class Chart
    {
        public int Id { get; set; }
        public int TipoId { get; set; }
        public string MonthName { get; set; }
        public int Total { get; set; }
    }
}
