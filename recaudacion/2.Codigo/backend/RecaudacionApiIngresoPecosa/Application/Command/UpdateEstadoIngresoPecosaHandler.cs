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
using RecaudacionApiIngresoPecosa.Clients;
using RecaudacionApiIngresoPecosa.Helpers;

namespace RecaudacionApiIngresoPecosa.Application.Command
{
    public class UpdateEstadoIngresoPecosaHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public IngresoPecosaEstadoFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;
            public CommandValidator(
                IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;

                RuleFor(x => x.Id)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Ingreso pecosa es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Ingreso pecosa no debe ser {x}");
                        }
                    });

                RuleFor(x => x.FormDto.Estado)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Estado es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Estado no debe ser {x}");
                        }
                    })
                    .MustAsync(async (estado, cancellation) =>
                    {
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_INGRESO_PECOSA, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Número de estado no existe.");

                RuleFor(x => x.FormDto.UsuarioModificador)
                      .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(IngresoPecosaConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {IngresoPecosaConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
        {
            private readonly IIngresoPecosaRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(IIngresoPecosaRepository repository, IEstadoAPI estadoAPI, IMapper mapper)
            {
                _repository = repository;
                _estadoAPI = estadoAPI;
                _mapper = mapper;
            }

            public async Task<StatusUpdateEstadoResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateEstadoResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    var ingresoPecosa = await _repository.FindById(request.Id);
                    var ingresoPecosaForm = request.FormDto;

                    if (ingresoPecosa == null || (request.Id != ingresoPecosaForm.IngresoPecosaId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    switch (ingresoPecosaForm.Estado)
                    {
                        case Definition.INGRESO_PECOSA_ESTADO_EMITIDO:
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                            response.Success = false;
                            return response;
                        case Definition.INGRESO_PECOSA_ESTADO_PROCESADO:
                            if (ingresoPecosa.Estado != Definition.INGRESO_PECOSA_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            }
                            break;
                        default:
                            break;
                    }

                    ingresoPecosa.Estado = ingresoPecosaForm.Estado;
                    ingresoPecosa.UsuarioModificador = ingresoPecosaForm.UsuarioModificador;
                    ingresoPecosa.FechaModificacion = DateTime.Now;

                    await _repository.Update(ingresoPecosa);
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE));

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
