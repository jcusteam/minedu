using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiReciboIngreso.Domain
{
    [Table("RECIBOS_INGRESOS")]
    public class ReciboIngreso
    {
        [Key]
        [Column("RECIBO_INGRESO_ID")]
        public int ReciboIngresoId { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("TIPO_RECIBO_INGRESO_ID")]
        public int TipoReciboIngresoId { get; set; }
        [NotMapped]
        public TipoReciboIngreso TipoReciboIngreso { get; set; }
        [Column("CLIENTE_ID")]
        public int ClienteId { get; set; }
        [NotMapped]
        public Cliente Cliente { get; set; }
        [Column("CUENTA_CORRIENTE_ID")]
        public int CuentaCorrienteId { get; set; }
        [NotMapped]
        public CuentaCorriente CuentaCorriente { get; set; }
        [Column("FUENTE_FINANCIAMIENTO_ID")]
        public int FuenteFinanciamientoId { get; set; }
        [Column("REGISTRO_LINEA_ID")]
        public int? RegistroLineaId { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("RECIBO_INGRESO_NUMERO")]
        public string Numero { get; set; }
        [Column("RECIBO_INGRESO_FECHA_EMISION")]
        public DateTime FechaEmision { get; set; }
        [Column("TIPO_CAPTACION_ID")]
        public int TipoCaptacionId { get; set; }
        [NotMapped]
        public TipoCaptacion TipoCaptacion { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_ID")]
        public int? DepositoBancoDetalleId { get; set; }
        [Column("RECIBO_INGRESO_IMPORTE_TOTAL", TypeName = "decimal(12,2)")]
        public decimal ImporteTotal { get; set; }
        [Column("RECIBO_INGRESO_NUMERO_DEPOSITO")]
        public string NumeroDeposito { get; set; }
        [Column("RECIBO_INGRESO_FECHA_DEPOSITO")]
        public DateTime? FechaDeposito { get; set; }
        [Column("RECIBO_INGRESO_NUMERO_CHEQUE")]
        public string NumeroCheque { get; set; }
        [Column("RECIBO_INGRESO_NUMERO_OFICIO")]
        public string NumeroOficio { get; set; }
        [Column("RECIBO_INGRESO_NUMERO_COMPROBANTE_PAGO")]
        public string NumeroComprobantePago { get; set; }
        [Column("RECIBO_INGRESO_EXPEDIENTE_SIAF")]
        public string ExpedienteSiaf { get; set; }
        [Column("RECIBO_INGRESO_NUMERO_RESOLUCION")]
        public string NumeroResolucion { get; set; }
        [Column("RECIBO_INGRESO_CARTA_ORDEN")]
        public string CartaOrden { get; set; }
        [Column("RECIBO_INGRESO_LIQUIDACION_INGRESO")]
        public string LiquidacionIngreso { get; set; }
        [Column("RECIBO_INGRESO_PAPELETA_DEPOSITO")]
        public string PapeletaDeposito { get; set; }
        [Column("RECIBO_INGRESO_CONCEPTO")]
        public string Concepto { get; set; }
        [Column("RECIBO_INGRESO_REFERENCIA")]
        public string Referencia { get; set; }
        [Column("LIQUIDACION_RECAUDACION_ID")]
        public int? LiquidacionId { get; set; }
        [Column("RECIBO_INGRESO_VALIDAR_DEPOSITO")]
        public string ValidarDeposito { get; set; }
        [Column("RECIBO_INGRESO_ESTADO")]
        public int Estado { get; set; }
        [NotMapped]
        public string EstadoNombre { get; set; }
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
