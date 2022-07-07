using System;
namespace RecaudacionApiPedidoPecosa.Domain
{
    public class PedidoPecosa
    {
        public int AnioEje { get; set; }
        public string Ejecutora { get; set; }
        public string TipoBien { get; set; }
        public int NumeroPecosa { get; set; }
        public DateTime FechaPecosa { get; set; }
        public string NombreAlmacen { get; set; }
        public string MotivoPedido { get; set; }
        public string NombreMarca { get; set; }
        public string CodigoItem { get; set; }
        public string NombreItem { get; set; }
        public string NombreUnidad { get; set; }
        public string Clasificador { get; set; }
        public string NombreClaficador { get; set; }
        public int CantidadAtendida { get; set; }
        public int CantidadAprobada { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }

}