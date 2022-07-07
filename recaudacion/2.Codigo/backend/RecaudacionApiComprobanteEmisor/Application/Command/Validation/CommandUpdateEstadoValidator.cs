using FluentValidation;
using RecaudacionApiComprobanteEmisor.Application.Command.Request;
using RecaudacionApiComprobanteEmisor.Helpers;

namespace RecaudacionApiComprobanteEmisor.Application.Command.Validation
{

    public class CommandUpdateEstadoValidator : AbstractValidator<CommandUpdateEstado>
    {
        public CommandUpdateEstadoValidator()
        {

            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Id Comprobante Emisor es requerido")
                .Custom((x, context) =>
                {
                    if (x < 1)
                    {
                        context.AddFailure($"Id Comprobante Emisor no debe ser {x}");
                    }
                });

            RuleFor(x => x.FormDto.Estado).NotNull().WithMessage("Estado es requerido");

            RuleFor(x => x.FormDto.UsuarioModificador)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Usuario Modificador es requerido")
                .MaximumLength(ComprobanteEmisorConsts.UsuarioModificadorMaxLength)
                .WithMessage($"La longitud del Usuario Modificador debe tener {ComprobanteEmisorConsts.UsuarioModificadorMaxLength} caracteres o menos");
        }
    }

}
