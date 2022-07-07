using RecaudacionUtils;

namespace RecaudacionApiLiquidacion.Domain
{
    public class LiquidacionFilter : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public string Numero { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }
        public string Estados { get; set; }
    }
}
