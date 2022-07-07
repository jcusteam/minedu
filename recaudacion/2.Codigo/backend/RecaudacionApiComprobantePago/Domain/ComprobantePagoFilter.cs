using RecaudacionUtils;
using System;

namespace RecaudacionApiComprobantePago.Domain
{
    public class ComprobantePagoFilter : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? TipoDocumentoId { get; set; }
        public int? ClienteId { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public int? TipoCaptacionId { get; set; }
        public int? TipoAdquisicion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }
        public string Estados { get; set; }
    }
}
