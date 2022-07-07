using RecaudacionUtils;

namespace RecaudacionApiComprobanteEmisor.Domain
{
    public class ComprobanteEmisorFilter : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public bool? Estado { get; set; }
    }
}
