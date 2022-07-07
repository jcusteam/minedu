using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiTipoDocumento.Domain
{
    [Table("TIPO_DOCUMENTO")]
    public class TipoDocumento
    {
        [Key]
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("TIPO_DOCUMENTO_NOMBRE")]
        public string Nombre { get; set; }
        [Column("TIPO_DOCUMENTO_ABREVIATURA")]
        public string Abreviatura { get; set; }
        [Column("TIPO_DOCUMENTO_ESTADO")]
        public bool Estado { get; set; }
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
