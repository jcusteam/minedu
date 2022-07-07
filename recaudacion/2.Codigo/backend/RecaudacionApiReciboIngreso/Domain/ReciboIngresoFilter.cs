using RecaudacionUtils;

namespace RecaudacionApiReciboIngreso.Domain
{
    public class ReciboIngresoFilter : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? TipoReciboIngresoId { get; set; }
        public int? ClienteId { get; set; }
        public string Numero { get; set; }
        public int? TipoCaptacionId { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }
        public string Estados { get; set; }
    }
}
