using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiPapeletaDeposito.Domain
{
    [Table("PAPELETA_DEPOSITOS")]
    public class PapeletaDeposito
    {
        [Key]
        [Column("PAPELETA_DEPOSITO_ID")]
        public int PapeletaDepositoId { get; set; }
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
        [Column("PAPELETA_DEPOSITO_NUMERO")]
        public string Numero { get; set; }
        [Column("PAPELETA_DEPOSITO_FECHA")]
        public DateTime? Fecha { get; set; }
        [Column("PAPELETA_DEPOSITO_MONTO", TypeName = "decimal(12,2)")]
        public decimal Monto { get; set; }
        [Column("PAPELETA_DEPOSITO_DESCRIPCION")]
        public string Descripcion { get; set; }
        [Column("PAPELETA_DEPOSITO_ESTADO")]
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