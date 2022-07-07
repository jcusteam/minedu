using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecaudacionApiDepositoBanco.Domain
{
    [Table("DEPOSITO_BANCOS_DETALLE")]
    public class DepositoBancoDetalle
    {
        [Key]
        [Column("DEPOSITO_BANCO_DETALLE_ID")]
        public int DepositoBancoDetalleId { get; set; }
        [Column("DEPOSITO_BANCO_ID")]
        public int DepositoBancoId { get; set; }
        [Column("CLIENTE_ID")]
        public int ClienteId { get; set; }
        [NotMapped]
        public Cliente Cliente { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_NUMERO_DEPOSITO")]
        public string NumeroDeposito { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_IMPORTE", TypeName = "decimal(16,2)")]
        public decimal Importe { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_FECHA_DEPOSITO")]
        public DateTime FechaDeposito { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_SECUENCIA")]
        public string Secuencia { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_TIPO_DOCUMENTO")]
        public int? TipoDocumento { get; set; }
        [NotMapped]
        public string TipoDocumentoNombre { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_SERIE_DOCUMENTO")]
        public string SerieDocumento { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_NUMERO_DOCUMENTO")]
        public string NumeroDocumento { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_FECHA_DOCUMENTO")]
        public DateTime? FechaDocumento { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_UTILIZADO")]
        public bool Utilizado { get; set; }
        [Column("DEPOSITO_BANCO_DETALLE_ESTADO")]
        public string Estado { get; set; }
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
