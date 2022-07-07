using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiComprobanteRetencion.Domain
{
    [Table("COMPROBANTE_RETENCION")]
    public class ComprobanteRetencion
    {
        [Key]
        [Column("COMPROBANTE_RETENCION_ID")]
        public int ComprobanteRetencionId { get; set; }
        [Column("CLIENTE_ID")]
        public int ClienteId { get; set; }
        [NotMapped]
        public Cliente Cliente { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("TIPO_COMPROBANTE_PAGO_ID")]
        public int TipoComprobanteId { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("COMPROBANTE_RETENCION_SERIE")]
        public string Serie { get; set; }
        [Column("COMPROBANTE_RETENCION_CORRELATIVO")]
        public string Correlativo { get; set; }
        [Column("COMPROBANTE_RETENCION_FECHA_EMISION")]
        public DateTime FechaEmision { get; set; }
        [Column("COMPROBANTE_RETENCION_PERIODO")]
        public DateTime Periodo { get; set; }
        [Column("COMPROBANTE_RETENCION_REGIMEN")]
        public string RegimenRetencion { get; set; }
        [NotMapped]
        public string RegimenRetencionDesc { get; set; }
        [Column("COMPROBANTE_RETENCION_TOTAL", TypeName = "decimal(16,2)")]
        public decimal Total { get; set; }
        [Column("COMPROBANTE_RETENCION_TOTAL_PAGO", TypeName = "decimal(16,2)")]
        public decimal TotalPago { get; set; }
        [Column("COMPROBANTE_RETENCION_PORCENTAJE", TypeName = "decimal(16,2)")]
        public decimal Porcentaje { get; set; }
        [Column("COMPROBANTE_RETENCION_NOMBRE_ARCHIVO")]
        public string NombreArchivo { get; set; }
        [Column("COMPROBANTE_RETENCION_OBSERVACION")]
        public string Observacion { get; set; }
        [Column("COMPROBANTE_RETENCION_ESTADO_SUNAT")]
        public string EstadoSunat { get; set; }
        [Column("COMPROBANTE_RETENCION_ESTADO")]
        public int Estado { get; set; }
        [NotMapped]
        public string NombreEstado { get; set; }
        [Column("USUARIO_CREADOR")]
        public string UsuarioCreador { get; set; }
        [Column("FECHA_CREACION")]
        public DateTime FechaCreacion { get; set; }
        [Column("USUARIO_MODIFICADOR")]
        public string UsuarioModificador { get; set; }
        [Column("FECHA_MODIFICACION")]
        public DateTime? FechaModificacion { get; set; }
    }

}
