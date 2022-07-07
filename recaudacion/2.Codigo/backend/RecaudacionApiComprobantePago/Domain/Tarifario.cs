namespace RecaudacionApiComprobantePago.Domain
{
    public class Tarifario
    {
        public int TarifarioId { get; set; }
        public int ClasificadorIngresoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }

    }
}