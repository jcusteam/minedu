using System;

namespace RecaudacionApiLiquidacion.Application.Command.Dtos
{
    public class LiquidacionEstadoFormDto
    {
        public int LiquidacionId { get; set; }
        public int Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
