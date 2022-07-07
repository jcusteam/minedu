using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiParametro.Domain
{
    [Table("PARAMETROS")]
    public class Parametro
    {
        [Key]
        [Column("PARAMETRO_ID")]
        public int ParametroId { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("PARAMETRO_SERIE")]
        public string Serie { get; set; }
        [Column("PARAMETRO_CORRELATIVO")]
        public string Correlativo { get; set; }
        [Column("PARAMETRO_ESTADO")]
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