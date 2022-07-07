using RecaudacionUtils;
using System;

namespace RecaudacionApiGuiaSalidaBien.Domain
{
    public class GuiaSalidaBienFilter : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public string Numero { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }
        public string Estados { get; set; }
    }
}
