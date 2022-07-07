using RecaudacionUtils;

namespace RecaudacionApiParametro.Domain
{
    public class ParametroFilter : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? TipoDocumentoId { get; set; }
        public bool? Estado { get; set; }
    }
}
