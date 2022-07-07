using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecaudacionApiComprobantePago.Domain
{
    [Table("COMPROBANTE_PAGOS_DETALLE")]
    public class ComprobantePagoDetalle
    {
        [Key]
        [Column("COMPROBANTE_PAGO_DETALLE_ID")]
        public int ComprobantePagoDetalleId { get; set; }
        [Column("COMPROBANTE_PAGO_ID")]
        public int ComprobantePagoId { get; set; }
        [Column("CATALOGO_BIEN_ID")]
        public int? CatalogoBienId { get; set; }
        [NotMapped]
        public CatalogoBien CatalogoBien { get; set; }
        [Column("TARIFARIO_ID")]
        public int? TarifarioId { get; set; }
        [NotMapped]
        public Tarifario Tarifario { get; set; }
        [Column("CLASIFICADOR_INGRESO_ID")]
        public int? ClasificadorIngresoId { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_UNIDAD_MEDIDA")]
        public string UnidadMedida { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_CANTIDAD")]
        public int Cantidad { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_CODIGO")]
        public string Codigo { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_DESCRIPCION")]
        public string Descripcion { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_CODIGO_TIPO_MONEDA")]
        public string CodigoTipoMoneda { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_PRECIO_UNITARIO", TypeName = "decimal(12,2)")]
        public decimal PrecioUnitario { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_CODIGO_TIPO_PRECIO")]
        public string CodigoTipoPrecio { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_AFECTO_IGV")]
        public bool AfectoIGV { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_IGV_ITEM", TypeName = "decimal(12,2)")]
        public decimal IGVItem { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_CODIGO_TIPO_IGV")]
        public int CodigoTipoIGV { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_DESCUENTO_ITEM", TypeName = "decimal(12,2)")]
        public decimal DescuentoItem { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_DESCUENTO_TOTAL", TypeName = "decimal(12,2)")]
        public decimal DescuentoTotal { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_FACTOR_DESCUENTO", TypeName = "decimal(12,2)")]
        public decimal FactorDescuento { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_SUB_TOTAL", TypeName = "decimal(12,2)")]
        public decimal SubTotal { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_VALOR_VENTA", TypeName = "decimal(12,2)")]
        public decimal ValorVenta { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_ID")]
        public int? IngresoPecosaDetalleId { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_SERIE_FORMATO")]
        public string SerieFormato { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_SERIE_DEL")]
        public int? SerieDel { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_SERIE_AL")]
        public int? SerieAl { get; set; }
        [Column("COMPROBANTE_PAGO_DETALLE_ESTADO")]
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
