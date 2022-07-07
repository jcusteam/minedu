namespace RecaudacionApiRegistroLinea.Application.Command.Dtos
{
    public class RegistroLineaDetalleFormDto
    {
        public int RegistroLineaId { get; set; }
        public int ClasificadorIngresoId { get; set; }
        public decimal Importe { get; set; }
        public string Referencia { get; set; }
        public string Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}