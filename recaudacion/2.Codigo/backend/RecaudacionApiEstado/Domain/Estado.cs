using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiEstado.Domain
{
    [Table("ESTADOS")]
    public class Estado
    {
        [Key]
        [Column("ESTADO_ID")]
        public int EstadoId { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("ESTADO_NUMERO")]
        public int Numero { get; set; }
        [Column("ESTADO_ORDEN")]
        public int Orden { get; set; }
        [Column("ESTADO_NOMBRE")]
        public string Nombre { get; set; }
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
