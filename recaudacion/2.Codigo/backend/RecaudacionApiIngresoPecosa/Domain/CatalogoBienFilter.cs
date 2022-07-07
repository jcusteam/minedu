namespace RecaudacionApiIngresoPecosa.Domain
{
    public class CatalogoBienFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int? ClasificadorIngresoId { get; set; }
        public int? UnidadMedidaId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool? Estado { get; set; }
    }
}