using System;

namespace RecaudacionApiComprobanteRetencion.Application.Query.Dtos
{
    public class ComprobanteRetencionFilterDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int? UnidadEjecutoraId { get; set; }
        public int? ClienteId { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }

    }
}
