using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiTipoDocumento.Application.Command.Dtos;
using RecaudacionApiTipoDocumento.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiTipoDocumento.Domain;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiTipoDocumento.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiTipoDocumento.Application.Command
{
    public class UpdateEstadoTipoDocumentoHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public TipoDocumentoEstadoFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("Id es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Tipo Documento no debe ser {x}");
                        }
                    });
                RuleFor(x => x.FormDto.Estado).NotNull().WithMessage("Estado es requerido");

                RuleFor(x => x.FormDto.UsuarioModificador)
                     .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(TipoDocumentoConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {TipoDocumentoConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
        {
            private readonly ITipoDocumentoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(ITipoDocumentoRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<StatusUpdateEstadoResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateEstadoResponse();

                try
                {
                    CommandValidator validations = new CommandValidator();
                    var result = validations.Validate(request);

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    var tipoDocumento = await _repository.FindById(request.Id);
                    var tipoDocumentoForm = request.FormDto;
                    if (tipoDocumento == null || (request.Id != tipoDocumentoForm.TipoDocumentoId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    tipoDocumento.Estado = tipoDocumentoForm.Estado;
                    tipoDocumento.UsuarioModificador = tipoDocumentoForm.UsuarioModificador;
                    tipoDocumento.FechaModificacion = DateTime.Now;
                    await _repository.Update(tipoDocumento);

                    if (tipoDocumento.Estado)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE_ACTIVO));
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE_INACTIVO));
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
