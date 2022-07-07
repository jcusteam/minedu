using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecaudacionApiRegistroLinea.Domain
{
    [Table("REGISTROS_LINEA_DETALLE")]
    public class RegistroLineaDetalle
    {
        [Key]
        [Column("REGISTRO_LINEA_DETALLE_ID")]
        public int RegistroLineaDetalleId { get; set; }
        [Column("REGISTRO_LINEA_ID")]
        public int RegistroLineaId { get; set; }
        [Column("CLASIFICADOR_INGRESO_ID")]
        public int ClasificadorIngresoId { get; set; }
        [NotMapped]
        public ClasificadorIngreso ClasificadorIngreso { get; set; }
        [Column("REGISTRO_LINEA_DETALLE_IMPORTE", TypeName = "decimal(12,2)")]
        public decimal Importe { get; set; }
        [Column("REGISTRO_LINEA_DETALLE_REFERENCIA")]
        public string Referencia { get; set; }
        [Column("REGISTRO_LINEA_DETALLE_ESTADO")]
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