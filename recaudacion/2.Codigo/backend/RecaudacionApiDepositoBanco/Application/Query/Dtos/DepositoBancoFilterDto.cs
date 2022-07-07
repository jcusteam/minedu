using RecaudacionUtils;
using System;

namespace RecaudacionApiDepositoBanco.Application.Query.Dtos
{
    public class DepositoBancoFilterDto : BaseFilter
    {
        public int? UnidadEjecutoraId { get; set; }
        public int? BancoId { get; set; }
        public int? CuentaCorrienteId { get; set; }
        public string Numero { get; set; }
        public string NombreArchivo { get; set; }
        public int? Estado { get; set; }
        public string Rol { get; set; }

    }

    public class DepositoBancoDetalleFilterDto
    {
        public string NumeroDeposito { get; set; }
        public DateTime FechaDeposito { get; set; }
        public int CuentaCorrienteId { get; set; }
        public int ClienteId { get; set; }

    }
}
