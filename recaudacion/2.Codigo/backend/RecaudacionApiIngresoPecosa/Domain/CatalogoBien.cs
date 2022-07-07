namespace RecaudacionApiIngresoPecosa.Domain
{
    public class CatalogoBien
    {
        public int CatalogoBienId { get; set; }
        public int ClasificadorIngresoId { get; set; }
        public int UnidadMedidaId { get; set; }
        public UnidadMedida UnidadMedida { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? StockMaximo { get; set; }
        public int? StockMinimo { get; set; }
        public int? PuntoReorden { get; set; }
        public bool Estado { get; set; }
        public int? Saldo { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}