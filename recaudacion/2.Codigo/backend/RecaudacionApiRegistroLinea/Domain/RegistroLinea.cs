using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiRegistroLinea.Domain
{
    [Table("REGISTROS_LINEA")]
    public class RegistroLinea
    {
        [Key]
        [Column("REGISTRO_LINEA_ID")]
        public int RegistroLineaId { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("CUENTA_CORRIENTE_ID")]
        public int CuentaCorrienteId { get; set; }
        [NotMapped]
        public CuentaCorriente CuentaCorriente { get; set; }
        [Column("BANCO_ID")]
        public int BancoId { get; set; }
        [NotMapped]
        public Banco Banco { get; set; }
        [Column("RECIBO_INGRESO_ID")]
        public int? ReciboIngresoId { get; set; }
        [Column("CLIENTE_ID")]
        public int ClienteId { get; set; }
        [NotMapped]
        public Cliente Cliente { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("REGISTRO_LINEA_NUMERO")]
        public string Numero { get; set; }
        [Column("REGISTRO_LINEA_FECHA_REGISTRO")]
        public DateTime FechaRegistro { get; set; }
        [Column("TIPO_RECIBO_INGRESO_ID")]
        public int TipoReciboIngresoId { get; set; }
        [NotMapped]
        public TipoReciboIngreso TipoReciboIngreso { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_ID")]
        public int? DepositoBancoDetalleId { get; set; }

        [Column("REGISTRO_LINEA_NUMERO_DEPOSITO")]
        public string NumeroDeposito { get; set; }
        [Column("REGISTRO_LINEA_IMPORTE_DEPOSITO", TypeName = "decimal(12,2)")]
        public decimal ImporteDeposito { get; set; }
        [Column("REGISTRO_LINEA_FECHA_DEPOSITO")]
        public DateTime FechaDeposito { get; set; }
        [Column("REGISTRO_LINEA_VALIDAR_DEPOSITO")]
        public string ValidarDeposito { get; set; }
        [Column("REGISTRO_LINEA_NUMERO_OFICIO")]
        public string NumeroOficio { get; set; }
        [Column("REGISTRO_LINEA_NUMERO_COMPROBANTE_PAGO")]
        public string NumeroComprobantePago { get; set; }
        [Column("REGISTRO_LINEA_EXPEDIENTE_SIAF")]
        public string ExpedienteSiaf { get; set; }
        [Column("REGISTRO_LINEA_NUMERO_RESOLUCION")]
        public string NumeroResolucion { get; set; }
        [Column("REGISTRO_LINEA_EXPEDIENTE_ESINAD")]
        public string ExpedienteESinad { get; set; }
        [Column("REGISTRO_LINEA_NUMERO_ESINAD")]
        public int? NumeroESinad { get; set; }
        [Column("REGISTRO_LINEA_OBSERVACION")]
        public string Observacion { get; set; }
        [Column("REGISTRO_LINEA_ESTADO")]
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
