using System;

namespace RecaudacionApiComprobanteRetencion.Application.Query.Dtos
{
    public class ComprobanteRetencionDetalleDto
    {
        public int ComprobanteRetencionDetalleId { get; set; }
        public int ComprobanteRetencionId { get; set; }
        public int? ComprobantePagoId { get; set; }
        public string TipoDocumento { get; set; }
        public string TipoDocumentoNombre { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal ImporteTotal { get; set; }
        public string TipoModena { get; set; }
        public decimal ImporteOperacion { get; set; }
        public bool ModificaNotaCredito { get; set; }
        public int NumeroCorrelativoPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal ImportePago { get; set; }
        public decimal Tasa { get; set; }
        public decimal ImporteRetenido { get; set; }
        public DateTime FechaRetencion { get; set; }
        public decimal ImporteNetoPagado { get; set; }
        public string Estado { get; set; }
    }
}
