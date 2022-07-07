using RecaudacionApiComprobantePago.Domain;

namespace RecaudacionApiComprobantePago.Application.Query.Dtos
{
    public class ComprobantePagoDetalleDto
    {
        public int ComprobantePagoDetalleId { get; set; }
        public int ComprobantePagoId { get; set; }
        public int? CatalogoBienId { get; set; }
        public CatalogoBien CatalogoBien { get; set; }
        public int? TarifarioId { get; set; }
        public Tarifario Tarifario { get; set; }
        public int? ClasificadorIngresoId { get; set; }
        public string UnidadMedida { get; set; }
        public int Cantidad { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string CodigoTipoMoneda { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string CodigoTipoPrecio { get; set; }
        public bool AfectoIGV { get; set; }
        public decimal IGVItem { get; set; }
        public int CodigoTipoIGV { get; set; }
        public decimal DescuentoItem { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal FactorDescuento { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ValorVenta { get; set; }
        public int? IngresoPecosaDetalleId { get; set; }
        public string SerieFormato { get; set; }
        public int? SerieDel { get; set; }
        public int? SerieAl { get; set; }
        public string Estado { get; set; }
    }
}
