using RecaudacionApiPapeletaDeposito.Domain;

namespace RecaudacionApiPapeletaDeposito.Application.Query.Dtos
{
    public class PapeletaDepositoDetalleDto
    {
        public int PapeletaDepositoDetalleId { get; set; }
        public int PapeletaDepositoId { get; set; }
        public int ReciboIngresoId { get; set; }
        public ReciboIngreso ReciboIngreso { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
    }
}