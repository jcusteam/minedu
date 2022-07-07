using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiComprobantePago.Domain
{
    [Table("COMPROBANTE_PAGOS")]
    public class ComprobantePago
    {
        [Key]
        [Column("COMPROBANTE_PAGO_ID")]
        public int ComprobantePagoId { get; set; }
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
        [Column("DEPOSITO_BANCO_DETALLE_ID")]
        public int? DepositoBancoDetalleId { get; set; }
        [Column("CUENTA_CORRIENTE_ID")]
        public int? CuentaCorrienteId { get; set; }
        [Column("TIPO_CAPTACION_ID")]
        public int TipoCaptacionId { get; set; }
        [Column("COMPROBANTE_EMISOR_ID")]
        public int ComprobanteEmisorId { get; set; }
        [Column("COMPROBANTE_PAGO_SERIE")]
        public string Serie { get; set; }
        [Column("COMPROBANTE_PAGO_CORRELATIVO")]
        public string Correlativo { get; set; }
        [Column("COMPROBANTE_PAGO_FECHA_EMISION")]
        public DateTime FechaEmision { get; set; }
        [Column("COMPROBANTE_PAGO_FECHA_VENCIMIENTO")]
        public DateTime FechaVencimiento { get; set; }
        [Column("COMPROBANTE_PAGO_TIPO_ADQUISICION")]
        public int TipoAdquisicion { get; set; }
        [Column("COMPROBANTE_PAGO_CODIGO_TIPO_OPERACION")]
        public string CodigoTipoOperacion { get; set; }
        [Column("COMPROBANTE_PAGO_TIPO_CONDICION_PAGO")]
        public int TipoCondicionPago { get; set; }
        [Column("COMPROBANTE_PAGO_NUMERO_DEPOSITO")]
        public string NumeroDeposito { get; set; }
        [Column("COMPROBANTE_PAGO_FECHA_DEPOSITO")]
        public DateTime? FechaDeposito { get; set; }
        [Column("COMPROBANTE_PAGO_VALIDAR_DEPOSITO")]
        public string ValidarDeposito { get; set; }
        [Column("COMPROBANTE_PAGO_NUMERO_CHEQUE")]
        public string NumeroCheque { get; set; }
        [Column("COMPROBANTE_PAGO_ENCARGADO_TIPO_DOCUMENTO")]
        public int? EncargadoTipoDocumento { get; set; }
        [Column("COMPROBANTE_PAGO_ENCARGADO_NOMBRE")]
        public string EncargadoNombre { get; set; }
        [Column("COMPROBANTE_PAGO_ENCARGADO_NUMERO_DOCUMENTO")]
        public string EncargadoNumeroDocumento { get; set; }
        [Column("COMPROBANTE_PAGO_FUENTE_ID")]
        public int? FuenteId { get; set; }
        [Column("COMPROBANTE_PAGO_FUENTE_TIPO_DOCUMENTO")]
        public int? FuenteTipoDocumento { get; set; }
        [Column("COMPROBANTE_PAGO_FUENTE_SERIE")]
        public string FuenteSerie { get; set; }
        [Column("COMPROBANTE_PAGO_FUENTE_CORRELATIVO")]
        public string FuenteCorrelativo { get; set; }
        [Column("COMPROBANTE_PAGO_FUENTE_ORIGEN")]
        public string FuenteOrigen { get; set; }
        [Column("COMPROBANTE_PAGO_FUENTE_VALIDAR")]
        public string FuenteValidar { get; set; }
        [Column("COMPROBANTE_PAGO_SUSTENTO")]
        public string Sustento { get; set; }
        [Column("COMPROBANTE_PAGO_OBSERVACION")]
        public string Observacion { get; set; }
        [Column("COMPROBANTE_PAGO_NOMBRE_ARCHIVO")]
        public string NombreArchivo { get; set; }
        [Column("COMPROBANTE_PAGO_TIPO_CAMBIO", TypeName = "decimal(12,4)")]
        public decimal TipoCambio { get; set; }
        [Column("COMPROBANTE_PAGO_PAGADO")]
        public bool Pagado { get; set; }
        [Column("COMPROBANTE_PAGO_ESTADO_SUNAT")]
        public string EstadoSunat { get; set; }

        [Column("COMPROBANTE_PAGO_CODIGO_TIPO_MONEDA")]
        public string CodigoTipoMoneda { get; set; }
        [Column("COMPROBANTE_PAGO_IMPORTE_BRUTO", TypeName = "decimal(12,2)")]
        public decimal ImporteBruto { get; set; }
        [Column("COMPROBANTE_PAGO_VALOR_IGV", TypeName = "decimal(12,2)")]
        public decimal ValorIGV { get; set; }
        [Column("COMPROBANTE_PAGO_IGV_TOTAL", TypeName = "decimal(12,2)")]
        public decimal IGVTotal { get; set; }
        [Column("COMPROBANTE_PAGO_ISC_TOTAL", TypeName = "decimal(12,2)")]
        public decimal ISCTotal { get; set; }
        [Column("COMPROBANTE_PAGO_OTR_TOTAL", TypeName = "decimal(12,2)")]
        public decimal OTRTotal { get; set; }
        [Column("COMPROBANTE_PAGO_OTRC_TOTAL", TypeName = "decimal(12,2)")]
        public decimal OTRCTotal { get; set; }
        [Column("COMPROBANTE_PAGO_IMPORTE_TOTAL", TypeName = "decimal(12,2)")]
        public decimal ImporteTotal { get; set; }
        [Column("COMPROBANTE_PAGO_IMPORTE_TOTAL_LETRA")]
        public string ImporteTotalLetra { get; set; }
        [Column("COMPROBANTE_PAGO_TOTAL_VENTA_OPGRAVADA", TypeName = "decimal(12,2)")]
        public decimal TotalOpGravada { get; set; }
        [Column("COMPROBANTE_PAGO_TOTAL_VENTA_OPINAFECTA", TypeName = "decimal(12,2)")]
        public decimal TotalOpInafecta { get; set; }
        [Column("COMPROBANTE_PAGO_TOTAL_VENTA_OPEXONERADA", TypeName = "decimal(12,2)")]
        public decimal TotalOpExonerada { get; set; }
        [Column("COMPROBANTE_PAGO_TOTAL_VENTA_OPGRATUITA", TypeName = "decimal(12,2)")]
        public decimal TotalOpGratuita { get; set; }
        [Column("COMPROBANTE_PAGO_TOTAL_DESCUENTO", TypeName = "decimal(12,2)")]
        public decimal TotalDescuento { get; set; }
        [Column("COMPROBANTE_PAGO_ORDEN_COMPRA")]
        public string OrdenCompra { get; set; }
        [Column("COMPROBANTE_PAGO_GUIA_REMISION")]
        public string GuiaRemision { get; set; }
        [Column("COMPROBANTE_PAGO_CODIGO_TIPO_NOTA")]
        public string CodigoTipoNota { get; set; }
        [Column("COMPROBANTE_PAGO_CODIGO_MOTIVO_NOTA")]
        public string CodigoMotivoNota { get; set; }
        [Column("COMPROBANTE_PAGO_ESTADO")]
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
