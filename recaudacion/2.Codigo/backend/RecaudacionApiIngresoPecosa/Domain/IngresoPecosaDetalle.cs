using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecaudacionApiIngresoPecosa.Domain
{
    [Table("INGRESO_PECOSA_DETALLE")]
    public class IngresoPecosaDetalle
    {
        [Key]
        [Column("INGRESO_PECOSA_DETALLE_ID")]
        public int IngresoPecosaDetalleId { get; set; }
        [Column("INGRESO_PECOSA_ID")]
        public int IngresoPecosaId { get; set; }
        [Column("CATALOGO_BIEN_ID")]
        public int CatalogoBienId { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_UNIDAD_MEDIDA")]
        public string UnidadMedida { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_CODIGO_ITEM")]
        public string CodigoItem { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_NOMBRE_ITEM")]
        public string NombreItem { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_NOMBRE_MARCA")]
        public string NombreMarca { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_CANTIDAD")]
        public int Cantidad { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_CANTIDAD_SALIDA")]
        public int CantidadSalida { get; set; }
        [NotMapped]
        public int Saldo { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_PRECIO_UNITARIO", TypeName = "decimal(12,2)")]
        public decimal PrecioUnitario { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_VALOR_TOTAL", TypeName = "decimal(12,2)")]
        public decimal ValorTotal { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_SERIE_FORMATO")]
        public string SerieFormato { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_SERIE_DEL")]
        public int SerieDel { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_SERIE_AL")]
        public int SerieAl { get; set; }
        [Column("INGRESO_PECOSA_DETALLE_ESTADO")]
        public string Estado { get; set; }
        [Column("USUARIO_CREADOR")]
        public string UsuarioCreador { get; set; }
        [Column("FECHA_CREACION")]
        public DateTime? FechaCreacion { get; set; }
        [Column("USUARIO_MODIFICADOR")]
        public string UsuarioModificador { get; set; }
        [Column("FECHA_MODIFICACION")]
        public DateTime? FechaModificacion { get; set; }
        [NotMapped]
        public int AnioPecosa { get; set; }
        [NotMapped]

        public int NumeroPecosa { get; set; }
    }
}
