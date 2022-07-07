using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiIngresoPecosa.Domain
{
    [Table("INGRESO_PECOSA")]
    public class IngresoPecosa
    {
        [Key]
        [Column("INGRESO_PECOSA_ID")]
        public int IngresoPecosaId { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("TIPO_DOCUMENTO_ID")]
        public int TipoDocumentoId { get; set; }
        [Column("INGRESO_PECOSA_ANIO")]
        public int AnioPecosa { get; set; }
        [Column("INGRESO_PECOSA_TIPO_BIEN")]
        public string TipoBien { get; set; }
        [Column("INGRESO_PECOSA_NUMERO")]
        public int NumeroPecosa { get; set; }
        [Column("INGRESO_PECOSA_FECHA")]
        public DateTime FechaPecosa { get; set; }
        [Column("INGRESO_PECOSA_NOMBRE_ALMACEN")]
        public string NombreAlmacen { get; set; }
        [Column("INGRESO_PECOSA_MOTIVO_PEDIDO")]
        public string MotivoPedido { get; set; }
        [Column("INGRESO_PECOSA_FECHA_REGISTRO")]
        public DateTime FechaRegistro { get; set; }
        [Column("INGRESO_PECOSA_ESTADO")]
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
