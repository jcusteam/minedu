namespace RecaudacionApiReporte.Domain
{
    public class CatalogoBien
    {
        public int CatalogoBienNro { get; set; }
        public UnidadMedida UnidadMedida { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? StockMaximo { get; set; }
        public int? StockMinimo { get; set; }
        public int? PuntoReorden { get; set; }
        public int? Saldo { get; set; }
    }
}