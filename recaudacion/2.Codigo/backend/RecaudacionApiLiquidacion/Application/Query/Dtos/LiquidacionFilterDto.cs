using RecaudacionUtils;

namespace RecaudacionApiLiquidacion.Application.Query.Dtos
{
    public class LiquidacionFilterDto : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public string Numero { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }

    }
}
