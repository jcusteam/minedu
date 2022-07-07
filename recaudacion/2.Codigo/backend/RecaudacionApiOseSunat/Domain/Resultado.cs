namespace RecaudacionApiOseSunat.Domain
{
    public class Resultado
    {
        public string CDR { get; set; }
        public string EstadoComprobante { get; set; }
        public string MensajeResultadoSunat { get; set; }
        public string NombrePDF { get; set; }
        public string NombreXML { get; set; }
        public string NombreZip { get; set; }
        public string HashXML { get; set; }
        public string HashXMLSunat { get; set; }
    }
}
