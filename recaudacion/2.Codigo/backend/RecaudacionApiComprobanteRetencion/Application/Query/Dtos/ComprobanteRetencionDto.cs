using System;
using System.Collections.Generic;
using RecaudacionApiComprobanteRetencion.Domain;

namespace RecaudacionApiComprobanteRetencion.Application.Query.Dtos
{
    public class ComprobanteRetencionDto
    {
        public int ComprobanteRetencionId { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int TipoDocumentoId { get; set; }
        public int TipoComprobanteId { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime Periodo { get; set; }
        public string RegimenRetencion { get; set; }
        public string RegimenRetencionDesc { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPago { get; set; }
        public decimal Porcentaje { get; set; }
        public string NombreArchivo { get; set; }
        public string Observacion { get; set; }
        public string EstadoSunat { get; set; }
        public int Estado { get; set; }
        public string NombreEstado { get; set; }
        public List<ComprobanteRetencionDetalleDto> ComprobanteRetencionDetalle { get; set; }

        public ComprobanteRetencionDto()
        {
            ComprobanteRetencionDetalle = new List<ComprobanteRetencionDetalleDto>();
        }
    }
}
