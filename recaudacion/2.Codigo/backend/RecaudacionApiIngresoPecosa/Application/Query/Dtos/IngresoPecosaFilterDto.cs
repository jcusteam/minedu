using RecaudacionUtils;
using System;

namespace RecaudacionApiIngresoPecosa.Application.Query.Dtos
{
    public class IngresoPecosaFilterDto : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? AnioPecosa { get; set; }
        public string TipoBien { get; set; }
        public int? NumeroPecosa { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }

    }
}
