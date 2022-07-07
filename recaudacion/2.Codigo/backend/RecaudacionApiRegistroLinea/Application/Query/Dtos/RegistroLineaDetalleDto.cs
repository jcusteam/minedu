using RecaudacionApiRegistroLinea.Domain;

namespace RecaudacionApiRegistroLinea.Application.Query.Dtos
{
    public class RegistroLineaDetalleDto
    {
        public int RegistroLineaDetalleId { get; set; }
        public int RegistroLineaId { get; set; }
        public int ClasificadorIngresoId { get; set; }
        public ClasificadorIngreso ClasificadorIngreso { get; set; }
        public decimal Importe { get; set; }
        public string Referencia { get; set; }
    }
}