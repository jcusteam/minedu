using RecaudacionApiPapeletaDeposito.Domain;

namespace RecaudacionApiPapeletaDeposito.Application.Command.Dtos
{
    public class PapeletaDepositoDetalleFormDto
    {
        public int PapeletaDepositoDetalleId { get; set; }
        public int PapeletaDepositoId { get; set; }
        public int ReciboIngresoId { get; set; }
        public ReciboIngreso ReciboIngreso { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
