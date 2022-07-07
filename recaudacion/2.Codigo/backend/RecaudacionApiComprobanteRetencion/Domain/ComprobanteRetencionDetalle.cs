using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace RecaudacionApiComprobanteRetencion.Domain
{
    [Table("COMPROBANTE_RETENCION_DETALLE")]
    public class ComprobanteRetencionDetalle
    {
        [Key]
        [Column("COMPROBANTE_RETENCION_DETALLE_ID")]
        public int ComprobanteRetencionDetalleId { get; set; }
        [Column("COMPROBANTE_RETENCION_ID")]
        public int ComprobanteRetencionId { get; set; }
        [Column("COMPROBANTE_PAGO_ID")]
        public int? ComprobantePagoId { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_TIPO_DOCUMENTO")]
        public string TipoDocumento { get; set; }
        [NotMapped]
        public string TipoDocumentoNombre { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_SERIE")]
        public string Serie { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_CORRELATIVO")]
        public string Correlativo { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_FECHA_EMISION")]
        public DateTime FechaEmision { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_IMPORTE_TOTAL", TypeName = "decimal(12,2)")]
        public decimal ImporteTotal { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_TIPO_MONEDA")]
        public string TipoModena { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_IMPORTE_OPERACION", TypeName = "decimal(12,2)")]
        public decimal ImporteOperacion { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_MODIFICA_NOTA_CREDITO")]
        public bool ModificaNotaCredito { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_NUMERO_CORRELATIVO_PAGO")]
        public int NumeroCorrelativoPago { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_FECHA_PAGO")]
        public DateTime FechaPago { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_TIPO_CAMBIO", TypeName = "decimal(12,2)")]
        public decimal TipoCambio { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_IMPORTE_PAGO", TypeName = "decimal(12,2)")]
        public decimal ImportePago { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_TASA", TypeName = "decimal(12,2)")]
        public decimal Tasa { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_IMPORTE_RETENIDO", TypeName = "decimal(12,2)")]
        public decimal ImporteRetenido { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_FECHA")]
        public DateTime FechaRetencion { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_IMPORTE_NETO_PAGAGO", TypeName = "decimal(12,2)")]
        public decimal ImporteNetoPagado { get; set; }
        [Column("COMPROBANTE_RETENCION_DETALLE_ESTADO")]
        public string Estado { get; set; }
        [Column("USUARIO_CREADOR")]
        public string UsuarioCreador { get; set; }
        [Column("FECHA_CREACION")]
        public DateTime? FechaCreacion { get; set; }
        [Column("USUARIO_MODIFICADOR")]
        public string UsuarioModificador { get; set; }
        [Column("FECHA_MODIFICACION")]
        public DateTime? FechaModificacion { get; set; }
    }
}
