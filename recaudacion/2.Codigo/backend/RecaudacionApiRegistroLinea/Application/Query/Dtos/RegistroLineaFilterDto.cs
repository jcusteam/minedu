using RecaudacionUtils;

namespace RecaudacionApiRegistroLinea.Application.Query.Dtos
{
    public class RegistroLineaFilterDto : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? CuentaCorrienteId { get; set; }
        public int? BancoId { get; set; }
        public int? ClienteId { get; set; }
        public string Numero { get; set; }
        public int? TipoReciboIngresoId { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }
    }
}
