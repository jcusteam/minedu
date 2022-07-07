namespace RecaudacionApiRegistroLinea.Application.Command.Dtos
{
    public class RegistroLineaEstadoFormDto
    {
        public int RegistroLineaId { get; set; }
        public string Observacion { get; set; }
        public int Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
