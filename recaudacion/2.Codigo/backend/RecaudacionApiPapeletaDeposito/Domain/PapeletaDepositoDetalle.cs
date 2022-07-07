using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiPapeletaDeposito.Domain
{
    [Table("PAPELETA_DEPOSITOS_DETALLE")]
    public class PapeletaDepositoDetalle
    {
        [Key]
        [Column("PAPELETA_DEPOSITO_DETALLE_ID")]
        public int PapeletaDepositoDetalleId { get; set; }
        [Column("PAPELETA_DEPOSITO_ID")]
        public int PapeletaDepositoId { get; set; }
        [Column("RECIBO_INGRESO_ID")]
        public int ReciboIngresoId { get; set; }
        [NotMapped]
        public ReciboIngreso ReciboIngreso { get; set; }
        [Column("PAPELETA_DEPOSITO_DETALLE_MONTO", TypeName = "decimal(12,2)")]
        public decimal Monto { get; set; }
        [Column("PAPELETA_DEPOSITO_DETALLE_ESTADO")]
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