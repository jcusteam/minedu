using RecaudacionUtils;

namespace RecaudacionApiPapeletaDeposito.Domain
{
    public class PapeletaDepositoFilter : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? BancoId { get; set; }
        public int? CuentaCorrienteId { get; set; }
        public string Numero { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }
        public string Estados { get; set; }
    }
}
