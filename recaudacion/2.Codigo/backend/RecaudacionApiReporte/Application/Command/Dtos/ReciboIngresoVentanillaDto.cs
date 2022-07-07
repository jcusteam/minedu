using System;
using System.Collections.Generic;

namespace RecaudacionApiReporte.Application.Command.Dtos
{
    public class ReciboIngresoVentanillaDto
    {
        public string Ejecutora { get; set; }
        public string SecEje { get; set; }
        public string NumeroRuc { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Procedencia { get; set; }
        public string Glosa { get; set; }
        public string CuentaCorriente { get; set; }
        public string Concepto { get; set; }
        public decimal Total { get; set; }
        public string Pliego { get; set; }
        public string FuenteFinanciamiento { get; set; }
        public string Unidad { get; set; }
        public string Codigo { get; set; }
        public string CodigoDos { get; set; }
        public string CuentaDebe { get; set; }
        public string CuentaHaber { get; set; }
        public string BoletaVenta { get; set; }
        public string Factura { get; set; }
        public string numeroLiquidacion { get; set; }

        public List<LiquidacionIngresoDto> Liquidaciones { get; set; }

        public ReciboIngresoVentanillaDto()
        {
            Liquidaciones = new List<LiquidacionIngresoDto>();
        }

    }

    public class LiquidacionIngresoDto
    {
        public int ClasificadorIngresoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Clasicador { get; set; }
        public string Parcial { get; set; }
        public string NombreTipoCaptacion { get; set; }
        public decimal Total { get; set; }

    }

}