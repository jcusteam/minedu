using System;
using System.Collections.Generic;

namespace RecaudacionApiPapeletaDeposito.Application.Command.Dtos
{
    public class PapeletaDepositoEstadoFormDto
    {
        public int PapeletaDepositoId { get; set; }
        public int Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
