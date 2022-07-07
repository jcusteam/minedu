using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiGuiaSalidaBien.Domain
{
    [Table("GUIA_SALIDA_BIENES")]
    public class GuiaSalidaBien
    {
        [Key]
        [Column("GUIA_SALIDA_BIEN_ID")]
        public int GuiaSalidaBienId { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("GUIA_SALIDA_BIEN_NUMERO")]
        public string Numero { get; set; }
        [Column("GUIA_SALIDA_BIEN_FECHA_REGISTRO")]
        public DateTime FechaRegistro { get; set; }
        [Column("GUIA_SALIDA_BIEN_JUSTIFICACION")]
        public string Justificacion { get; set; }
        [Column("GUIA_SALIDA_BIEN_ESTADO")]
        public int Estado { get; set; }
        [NotMapped]
        public string EstadoNombre { get; set; }
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
