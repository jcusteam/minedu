using RecaudacionUtils;

namespace RecaudacionApiPapeletaDeposito.Application.Query.Dtos
{
    public class PapeletaDepositoFilterDto : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? BancoId { get; set; }
        public int? CuentaCorrienteId { get; set; }
        public string Numero { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }

    }
}
