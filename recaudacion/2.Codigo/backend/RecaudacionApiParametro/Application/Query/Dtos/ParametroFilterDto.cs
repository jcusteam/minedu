using RecaudacionUtils;

namespace RecaudacionApiParametro.Application.Query.Dtos
{
    public class ParametroFilterDto : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? TipoDocumentoId { get; set; }
        public bool? Estado { get; set; }

    }
}
