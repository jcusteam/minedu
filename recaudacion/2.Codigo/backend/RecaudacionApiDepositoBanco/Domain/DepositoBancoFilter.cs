using RecaudacionUtils;

namespace RecaudacionApiDepositoBanco.Domain
{
    public class DepositoBancoFilter : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? BancoId { get; set; }
        public int? CuentaCorrienteId { get; set; }
        public string Numero { get; set; }
        public string NombreArchivo { get; set; }
        public int? Estado { get; set; }
        public string Estados { get; set; }
    }
}
