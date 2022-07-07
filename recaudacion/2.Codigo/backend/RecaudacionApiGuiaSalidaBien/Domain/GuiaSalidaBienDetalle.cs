using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiGuiaSalidaBien.Domain
{
    [Table("GUIA_SALIDA_BIENES_DETALLE")]
    public class GuiaSalidaBienDetalle
    {
        [Key]
        [Column("GUIA_SALIDA_BIEN_DETALLE_ID")]
        public int GuiaSalidaBienDetalleId { get; set; }
        [Column("GUIA_SALIDA_BIEN_ID")]
        public int GuiaSalidaBienId { get; set; }
        [Column("CATALOGO_BIEN_ID")]
        public int CatalogoBienId { get; set; }
        [NotMapped]
        public CatalogoBien CatalogoBien { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_ID")]
        public int? IngresoPecosaDetalleId { get; set; }
        [Column("GUIA_SALIDA_BIEN_DETALLE_CANTIDAD")]
        public int Cantidad { get; set; }
        [Column("GUIA_SALIDA_BIEN_DETALLE_SERIE_FORMATO")]
        public string SerieFormato { get; set; }
        [Column("GUIA_SALIDA_BIEN_DETALLE_SERIE_DEL")]
        public int SerieDel { get; set; }
        [Column("GUIA_SALIDA_BIEN_DETALLE_SERIE_AL")]
        public int SerieAl { get; set; }
        [Column("GUIA_SALIDA_BIEN_DETALLE_ESTADO")]
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
