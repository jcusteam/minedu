using System;
using System.Collections.Generic;

namespace RecaudacionApiReporte.Application.Command.Dtos
{
    public class ReciboIngresoDto
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
        public string NumeroCheque { get; set; }
        public string NumeroOficio { get; set; }
        public string NumeroComprobantePago { get; set; }
        public string ExpedienteSiaf { get; set; }
        public string NumeroDeposito { get; set; }
        public string NumeroResolucion { get; set; }
        public decimal Parcial { get; set; }
        public decimal Total { get; set; }
        public string Pliego { get; set; }
        public string FuenteFinanciamiento { get; set; }
        public string Unidad { get; set; }
        public string Codigo { get; set; }
        public string CodigoDos { get; set; }
        public string CuentaDebe { get; set; }
        public string CuentaHaber { get; set; }

        public List<ReciboIngresoDetalleDto> detalles { get; set; }
    }

    public class ReciboIngresoDetalleDto
    {
        public string Clasificador { get; set; }
        public decimal Parcial { get; set; }
    }


}