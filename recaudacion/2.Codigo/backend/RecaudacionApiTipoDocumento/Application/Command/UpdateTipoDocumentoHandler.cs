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
    public class UpdateTipoDocumentoHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public TipoDocumentoFormDto FormDto { get; set; }
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

                RuleFor(x => x.FormDto.Nombre)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Nombre es requerido")
                    .MaximumLength(TipoDocumentoConsts.NombreMaxLength)
                    .WithMessage($"La longitud del Nombre debe tener {TipoDocumentoConsts.NombreMaxLength} caracteres o menos")
                     .Custom((x, context) =>
                     {
                         if (!String.IsNullOrEmpty(x))
                             if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                             {
                                 context.AddFailure("El Nombre contiene caracter no válido.");
                             }
                     });
                RuleFor(x => x.FormDto.Abreviatura)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Abreviatura es requerido")
                    .MaximumLength(TipoDocumentoConsts.NombreMaxLength)
                    .WithMessage($"La longitud de la Abreviatura debe tener {TipoDocumentoConsts.NombreMaxLength} caracteres o menos")
                     .Custom((x, context) =>
                     {
                         if (!String.IsNullOrEmpty(x))
                             if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                             {
                                 context.AddFailure("La Abreviatura contiene caracter no válido");
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

        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
        {
            private readonly ITipoDocumentoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(ITipoDocumentoRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateResponse();

                try
                {
                    CommandValidator validations = new CommandValidator();
                    var result = validations.Validate(request);

                    if (result.IsValid)
                    {
                        var tipoDocumentoEx = await _repository.FindById(request.Id);
                        if (tipoDocumentoEx == null || (request.Id != request.FormDto.TipoDocumentoId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }

                        var tipoDocumento = _mapper.Map<TipoDocumentoFormDto, TipoDocumento>(request.FormDto);
                        tipoDocumento.FechaModificacion = DateTime.Now;
                        tipoDocumento.Nombre = Regex.Replace(tipoDocumento.Nombre, @"\s+", " ");
                        tipoDocumento.Nombre = tipoDocumento.Nombre.ToUpper();

                        var exists = await _repository.VerifyExists(Definition.UPDATE, tipoDocumento);

                        if (!exists)
                        {
                            await _repository.Update(tipoDocumento);
                            if (tipoDocumento.Estado == tipoDocumentoEx.Estado)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE));
                            }
                            else
                            {
                                if (tipoDocumento.Estado)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE_ACTIVO));
                                }
                                else
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE_INACTIVO));
                                }
                            }
                            response.Success = true;
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
