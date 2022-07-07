namespace RecaudacionApiGuiaSalidaBien.Domain
{
    public class IngresoPecosaDetalle
    {
        public int IngresoPecosaDetalleId { get; set; }
        public int IngresoPecosaId { get; set; }
        public int AnioPecosa { get; set; }
        public int NumeroPecosa { get; set; }
        public int CatalogoBienId { get; set; }
        public string Estado { get; set; }
    }
}
