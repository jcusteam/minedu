using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiDepositoBanco.Domain
{
    [Table("DEPOSITO_BANCOS")]
    public class DepositoBanco
    {
        [Key]
        [Column("DEPOSITO_BANCO_ID")]
        public int DepositoBancoId { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("BANCO_ID")]
        public int BancoId { get; set; }
        [NotMapped]
        public Banco Banco { get; set; }
        [Column("CUENTA_CORRIENTE_ID")]
        public int CuentaCorrienteId { get; set; }
        [NotMapped]
        public CuentaCorriente CuentaCorriente { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("DEPOSITO_BANCO_NUMERO")]
        public string Numero { get; set; }
        [Column("DEPOSITO_BANCO_IMPORTE", TypeName = "decimal(12,2)")]
        public decimal Importe { get; set; }
        [Column("DEPOSITO_BANCO_FECHA_DEPOSITO")]
        public DateTime FechaDeposito { get; set; }
        [Column("DEPOSITO_BANCO_FECHA_REGISTRO")]
        public DateTime FechaRegistro { get; set; }
        [Column("DEPOSITO_BANCO_NOMBRE_ARCHIVO")]
        public string NombreArchivo { get; set; }
        [Column("DEPOSITO_BANCO_CANTIDAD")]
        public int Cantidad { get; set; }
        [Column("DEPOSITO_BANCO_ESTADO")]
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
