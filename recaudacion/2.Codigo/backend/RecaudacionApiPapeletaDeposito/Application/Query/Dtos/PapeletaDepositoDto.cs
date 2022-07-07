using System;
using System.Collections.Generic;
using RecaudacionApiPapeletaDeposito.Domain;

namespace RecaudacionApiPapeletaDeposito.Application.Query.Dtos
{
    public class PapeletaDepositoDto
    {
        public int PapeletaDepositoId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int BancoId { get; set; }
        public Banco Banco { get; set; }
        public int CuentaCorrienteId { get; set; }
        public CuentaCorriente CuentaCorriente { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Numero { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; }
        public string EstadoNombre { get; set; }
        public List<PapeletaDepositoDetalleDto> PapeletaDepositoDetalle { get; set; }
        public PapeletaDepositoDto()
        {
            PapeletaDepositoDetalle = new List<PapeletaDepositoDetalleDto>();
        }
    }
}
