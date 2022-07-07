using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiEstado.Application.Command.Dtos;
using RecaudacionApiEstado.DataAccess;
using RecaudacionApiEstado.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiEstado.Clients;
using RecaudacionApiEstado.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiEstado.Application.Command
{
    public class AddEstadoHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddResponse>
        {
            public EstadoFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            public CommandValidator(ITipoDocumentoAPI tipoDocumentoAPI)
            {
                _tipoDocumentoAPI = tipoDocumentoAPI;
                RuleFor(x => x.FormDto.TipoDocumentoId).NotEmpty().WithMessage("Tipo Documento es requerido")
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

                RuleFor(x => x.FormDto.Orden).NotEmpty().WithMessage("Orden es requerido")
                   .Custom((x, context) =>
                   {
                       if (x < 1)
                       {
                           context.AddFailure($"El Orden no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.Numero).NotEmpty().WithMessage("Número es requerido")
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
                            context.AddFailure("El Nombre contiene caracter no válido.");
                        }
                    });

                RuleFor(x => x.FormDto.UsuarioCreador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Creador es requerido")
                    .MaximumLength(EstadoConsts.UsuarioCreadorMaxLength)
                    .WithMessage($"La longitud del Usuario Creador debe tener {EstadoConsts.UsuarioCreadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusAddResponse>
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

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_tipoDocumentoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {
                        var estado = new Estado();
                        estado = _mapper.Map<EstadoFormDto, Estado>(request.FormDto);
                        estado.FechaCreacion = DateTime.Now;
                        estado.Nombre = Regex.Replace(estado.Nombre, @"\s+", " ");
                        estado.Nombre = estado.Nombre.ToUpper();

                        var exists = await _repository.VerifyExists(Definition.INSERT, estado);

                        if (!exists)
                        {
                            estado = await _repository.Add(estado);

                            if (estado != null)
                            {
                                response.Data = _mapper.Map<Estado, EstadoFormDto>(estado);
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_INSERT));
                                response.Success = true;
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                                response.Success = false;
                            }
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
