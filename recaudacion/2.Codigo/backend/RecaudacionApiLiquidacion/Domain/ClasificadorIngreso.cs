namespace RecaudacionApiLiquidacion.Domain
{
    public class ClasificadorIngreso
    {
        public int ClasificadorIngresoId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int CuentaContableIdDebe { get; set; }
        public int CuentaContableIdHaber { get; set; }
        public bool Estado { get; set; }
    }
}