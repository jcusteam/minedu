using RecaudacionUtils;

namespace RecaudacionApiIngresoPecosa.Application.Query.Dtos
{
    public class CatalogoBienFilterDto : BaseFilter
    {
        public int? ClasificadorIngresoId { get; set; }
        public int? UnidadMedidaId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool? Estado { get; set; }
    }
}
