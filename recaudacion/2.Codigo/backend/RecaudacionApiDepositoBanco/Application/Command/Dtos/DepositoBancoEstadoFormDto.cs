using System;
using System.Collections.Generic;

namespace RecaudacionApiDepositoBanco.Application.Command.Dtos
{
    public class DepositoBancoEstadoFormDto
    {
        public int DepositoBancoId { get; set; }
        public int Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
