using RecaudacionUtils;

namespace RecaudacionApiEstado.Application.Query.Dtos
{
    public class EstadoFilterDto : BaseFilter
    {
        public int? TipoDocumentoId { get; set; }
        public string Nombre { get; set; }

    }
}
