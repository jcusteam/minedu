using System;

namespace RecaudacionApiEstado.Application.Query.Dtos
{
    public class EstadoDto
    {
        public int EstadoId { get; set; }
        public int TipoDocumentoId { get; set; }
        public int Numero { get; set; }
        public int Orden { get; set; }
        public string Nombre { get; set; }
    }
}
