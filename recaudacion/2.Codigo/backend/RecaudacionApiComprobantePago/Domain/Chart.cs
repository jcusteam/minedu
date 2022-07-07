using System.Collections.Generic;

namespace RecaudacionApiComprobantePago.Domain
{
    public class Chart
    {
        public int Id { get; set; }
        public int TipoId { get; set; }
        public string MonthName { get; set; }
        public int Total { get; set; }
    }
    public class ChartTipo
    {
        public int TipoId { get; set; }
        public string TipoName { get; set; }
        public List<Chart> charts { get; set; }

        public ChartTipo()
        {
            charts = new List<Chart>();
        }
    }
}
