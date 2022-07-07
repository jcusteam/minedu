using System;

namespace RecaudacionApiOseSunat.Domain
{
    public class DetalleRetencion
    {
        public string CodigoTipoComprobante { get; set; }
        public string SerieComprobanteRef { get; set; }
        public string NumeroComprobanteRef { get; set; }
        public string FechaEmisionComprobante { get; set; }
        public string MonedaComprobante { get; set; }
        public Decimal ImporteTotalComprobante { get; set; }
        public Decimal MontoTotalPago { get; set; }
        public string FechaPago { get; set; }
        public Decimal MontoRetencion { get; set; }
        public string FechaRetencion { get; set; }
        // Tipo de Cambio
        public string MonedaTCambioOrigen { get; set; }
        public string MonedaTCambioDestino { get; set; }
        public Decimal TipoCambio { get; set; }
        public string FechaTipoCambio { get; set; }
    }
}
