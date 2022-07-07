namespace RecaudacionApiComprobantePago.Domain
{
    public class CatalogoBien
    {
        public int CatalogoBienId { get; set; }
        public int ClasificadorIngresoId { get; set; }
        public int UnidadMedidaId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}