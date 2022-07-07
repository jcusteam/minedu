using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiLiquidacion.Domain
{
    [Table("LIQUIDACION_RECAUDACION")]
    public class Liquidacion
    {
        [Key]
        [Column("LIQUIDACION_RECAUDACION_ID")]
        public int LiquidacionId { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("FUENTE_FINANCIAMIENTO_ID")]
        public int FuenteFinanciamientoId { get; set; }
        [NotMapped]
        public FuenteFinanciamiento FuenteFinanciamiento { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("CLIENTE_ID")]
        public int ClienteId { get; set; }
        [Column("CUENTA_CORRIENTE_ID")]
        public int CuentaCorrienteId { get; set; }
        [Column("RECIBO_INGRESO_ID")]
        public int? ReciboIngresoId { get; set; }
        [Column("LIQUIDACION_RECAUDACION_NUMERO")]
        public string Numero { get; set; }
        [Column("LIQUIDACION_RECAUDACION_PROCEDENCIA")]
        public string Procedencia { get; set; }
        [Column("LIQUIDACION_RECAUDACION_FECHA")]
        public DateTime FechaRegistro { get; set; }
        [Column("LIQUIDACION_RECAUDACION_TOTAL", TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }
        [Column("LIQUIDACION_RECAUDACION_FACTURA")]
        public string Factura { get; set; }
        [Column("LIQUIDACION_RECAUDACION_BOLETA")]
        public string BoletaVenta { get; set; }
        [Column("LIQUIDACION_RECAUDACION_ESTADO")]
        public int Estado { get; set; }
        [NotMapped]
        public string EstadoNombre { get; set; }
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
