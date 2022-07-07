namespace RecaudacionApiLiquidacion.Domain
{
    public class LiquidacionDetalle
    {
        public int ClasificadorIngresoId { get; set; }
        public ClasificadorIngreso ClasificadorIngreso { get; set; }
        public int TipoCaptacionId { get; set; }
        public TipoCaptacion TipoCaptacion { get; set; }
        public decimal ImporteParcial { get; set; }
    }
}