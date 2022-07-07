using System;
using System.Collections.Generic;
using RecaudacionApiDepositoBanco.Domain;

namespace RecaudacionApiDepositoBanco.Application.Query.Dtos
{
    public class DepositoBancoDto
    {
        public int DepositoBancoId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int BancoId { get; set; }
        public Banco Banco { get; set; }
        public int CuentaCorrienteId { get; set; }
        public CuentaCorriente CuentaCorriente { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Numero { get; set; }
        public decimal Importe { get; set; }
        public DateTime FechaDeposito { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NombreArchivo { get; set; }
        public int Cantidad { get; set; }
        public int Estado { get; set; }
        public string EstadoNombre { get; set; }
        public List<DepositoBancoDetalleDto> DepositoBancoDetalle { get; set; }

        public DepositoBancoDto()
        {
            DepositoBancoDetalle = new List<DepositoBancoDetalleDto>();
        }
    }
}
