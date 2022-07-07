namespace RecaudacionApiIngresoPecosa.Application.Command.Dtos
{
    public class IngresoPecosaDetalleFormDto
    {
        public int IngresoPecosaDetalleId { get; set; }
        public int IngresoPecosaId { get; set; }
        public int CatalogoBienId { get; set; }
        public string UnidadMedida { get; set; }
        public string CodigoItem { get; set; }
        public string NombreItem { get; set; }
        public string NombreMarca { get; set; }
        public int Cantidad { get; set; }
        public int CantidadSalida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public string SerieFormato { get; set; }
        public int SerieDel { get; set; }
        public int SerieAl { get; set; }
        public string Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}