using System;

namespace RecaudacionApiEstado.Application.Command.Dtos
{
    public class EstadoFormDto
    {
        public int EstadoId { get; set; }
        public int TipoDocumentoId { get; set; }
        public int Numero { get; set; }
        public int Orden { get; set; }
        public string Nombre { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
