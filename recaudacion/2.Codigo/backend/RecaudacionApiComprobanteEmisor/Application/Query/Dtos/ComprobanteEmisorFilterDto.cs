using RecaudacionUtils;

namespace RecaudacionApiComprobanteEmisor.Application.Query.Dtos
{
    public class ComprobanteEmisorFilterDto : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public bool? Estado { get; set; }

    }
}
