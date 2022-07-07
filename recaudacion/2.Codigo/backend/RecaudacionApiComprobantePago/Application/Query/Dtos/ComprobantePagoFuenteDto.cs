namespace RecaudacionApiComprobantePago.Application.Query.Dtos
{
    public class ComprobantePagoFuenteDto
    {
        public int UnidadEjecutoraId { get; set; }
        public int TipoComprobanteId { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }

    }
}
