using System;

namespace RecaudacionApiReciboIngreso.Domain
{
    public class DepositoBancoDetalle
    {
        public int DepositoBancoDetalleId { get; set; }
        public int DepositoBancoId { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public string NumeroDeposito { get; set; }
        public decimal Importe { get; set; }
        public DateTime? FechaDeposito { get; set; }
        public string Secuencia { get; set; }
        public int? TipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public bool? Utilizado { get; set; }
        public string Estado { get; set; }
        public string UsuarioModificador { get; set; }

    }
}
