using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiEstado.Application.Command.Dtos;
using RecaudacionApiEstado.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiEstado.Domain;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiEstado.Clients;
using RecaudacionApiEstado.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiEstado.Application.Command
{
    public class UpdateEstadoHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public EstadoFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            public CommandValidator(ITipoDocumentoAPI tipoDocumentoAPI)
            {
                _tipoDocumentoAPI = tipoDocumentoAPI;

                RuleFor(x => x.Id)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id es requerido")
                    .Custom((x, context) =>
                     {
                         if (x < 1)
                         {
                             context.AddFailure($"Id Estado no debe ser {x}");
                         }
                     });

                RuleFor(x => x.FormDto.TipoDocumentoId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Tipo Documento es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Tipo Documento no debe ser {x}");
                        }
                    })
                    .MustAsync(async (id, cancellation) =>
                    {
                        var reponse = await _tipoDocumentoAPI.FindByIdAsync(id);
                        bool exists = reponse.Success;
                        return exists;
                    }).WithMessage("Id Tipo Documento no existe");

                RuleFor(x => x.FormDto.Orden)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Orden es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"El Orden no debe ser {x}");
                        }
                    });
                RuleFor(x => x.FormDto.Numero)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Número es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Número no debe ser {x}");
                        }
                    });

                RuleFor(x => x.FormDto.Nombre)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Nombre es requerido")
                    .MaximumLength(EstadoConsts.NombreMaxLength)
                    .WithMessage($"La longitud del Nombre debe tener {EstadoConsts.NombreMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                        {
                            context.AddFailure("El Nombre contiene caracter no válido");
                        }
                    });

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(EstadoConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {EstadoConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }
        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
        {
            private readonly IEstadoRepository _repository;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IMapper _mapper;

            public Handler(IEstadoRepository repository,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _mapper = mapper;
            }

            public async Task<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_tipoDocumentoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {

                        var estadoEx = await _repository.FindById(request.Id);

                        if (estadoEx.TipoDocumentoId != request.Id || estadoEx == null)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }

                        var estado = _mapper.Map<EstadoFormDto, Estado>(request.FormDto);
                        estado.Nombre = Regex.Replace(estado.Nombre, @"\s+", " ");
                        estado.Nombre = estado.Nombre.ToUpper();

                        var exists = await _repository.VerifyExists(Definition.UPDATE, estado);

                        if (!exists)
                        {
                            estado.FechaModificacion = DateTime.Now;
                            await _repository.Update(estado);
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE));
                        }
                        else
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                            response.Success = false;
                        }

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
