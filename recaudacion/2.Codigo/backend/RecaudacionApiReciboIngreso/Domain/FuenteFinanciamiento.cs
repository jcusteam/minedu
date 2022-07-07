namespace RecaudacionApiReciboIngreso.Domain
{
    public class FuenteFinanciamiento
    {
        public int FuenteFinanciamientoId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string RubroCodigo { get; set; }
        public string RubroDescripcion { get; set; }
    }
}
