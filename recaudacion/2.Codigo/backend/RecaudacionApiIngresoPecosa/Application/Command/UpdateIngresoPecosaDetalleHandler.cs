using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Application.Command.Dtos;
using RecaudacionApiIngresoPecosa.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiIngresoPecosa.Domain;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiIngresoPecosa.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiIngresoPecosa.Application.Command
{
    public class UpdateIngresoPecosaDetalleHandler
    {
        public class StatusUpdateDetalleResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateDetalleResponse>
        {
            public int Id { get; set; }
            public IngresoPecosaDetalleFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Ingreso pecosa detalle es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Ingreso pecosa detalle no debe ser {x}");
                        }
                    });

                RuleFor(x => x.FormDto.CantidadSalida)
                    .Cascade(CascadeMode.Stop)
                    .Custom((x, context) =>
                    {
                        if (x < 0)
                        {
                            context.AddFailure($"Cantidad salida no debe ser {x}");
                        }
                    });

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(IngresoPecosaDetalleConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {IngresoPecosaDetalleConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateDetalleResponse>
        {

            private readonly IIngresoPecosaDetalleRepository _detalleRepository;

            private readonly IMapper _mapper;

            public Handler(
                 IIngresoPecosaDetalleRepository detalleRepository,
                IMapper mapper)
            {
                _detalleRepository = detalleRepository;
                _mapper = mapper;

            }

            public async Task<StatusUpdateDetalleResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateDetalleResponse();
                try
                {
                    CommandValidator validations = new CommandValidator();
                    var result = validations.Validate(request);

                    if (result.IsValid)
                    {
                        var ingresoPecosaDetalle = await _detalleRepository.FindById(request.Id);
                        if (ingresoPecosaDetalle == null || (request.Id != request.FormDto.IngresoPecosaDetalleId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }

                        var ingresoPecosaDetalleForm = _mapper.Map<IngresoPecosaDetalleFormDto, IngresoPecosaDetalle>(request.FormDto);

                        ingresoPecosaDetalle.CantidadSalida = ingresoPecosaDetalleForm.CantidadSalida;
                        ingresoPecosaDetalle.UsuarioModificador = ingresoPecosaDetalle.UsuarioModificador;
                        ingresoPecosaDetalle.FechaModificacion = DateTime.Now;
                        await _detalleRepository.Update(ingresoPecosaDetalle);
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE));
                        response.Success = true;
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                    }
                }
                catch (System.Exception)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }

                return response;

            }
        }
    }
}
