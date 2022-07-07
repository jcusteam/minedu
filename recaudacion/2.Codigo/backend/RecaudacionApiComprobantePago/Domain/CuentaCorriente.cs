namespace RecaudacionApiComprobantePago.Domain
{
    public class CuentaCorriente
    {
        public int CuentaCorrienteId { get; set; }
        public int BancoId { get; set; }
        public string Codigo { get; set; }
        public string Numero { get; set; }
        public string Denominacion { get; set; }
    }
}
