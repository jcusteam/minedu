using System;

namespace RecaudacionApiComprobantePago.Application.Command.Dtos
{
    public class ComprobantePagoDetalleFormDto
    {
        public int ComprobantePagoDetalleId { get; set; }
        public int ComprobantePagoId { get; set; }
        public int TipoAdquisicion { get; set; }
        public int? CatalogoBienId { get; set; }
        public int? TarifarioId { get; set; }
        public int? ClasificadorIngresoId { get; set; }
        public string UnidadMedida { get; set; }
        public int Cantidad { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string CodigoTipoMoneda { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioSinIGV { get; set; }
        public decimal TotalItemSinIGV { get; set; }
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
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
