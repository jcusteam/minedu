using RecaudacionApiReciboIngreso.Domain;

namespace RecaudacionApiReciboIngreso.Application.Query.Dtos
{
    public class ReciboIngresoDetalleDto
    {
        public int ReciboIngresoDetalleId { get; set; }
        public int ReciboIngresoId { get; set; }
        public int ClasificadorIngresoId { get; set; }
        public ClasificadorIngreso ClasificadorIngreso { get; set; }
        public decimal Importe { get; set; }
        public string Referencia { get; set; }
    }
}