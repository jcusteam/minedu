using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiParametro.Application.Command.Dtos;
using RecaudacionApiParametro.DataAccess;
using RecaudacionApiParametro.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiParametro.Helpers;
using RecaudacionApiParametro.Clients;
using System.Text.RegularExpressions;

namespace RecaudacionApiParametro.Application.Command
{
    public class AddParametroHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddResponse>
        {
            public ParametroFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            public CommandValidator(
                ITipoDocumentoAPI tipoDocumentoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI)
            {
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;

                RuleFor(x => x.FormDto.UnidadEjecutoraId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Unidad Ejecutora es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Unidad Ejecutora no debe ser {x}");
                        }
                    })
                    .MustAsync(async (id, cancellation) =>
                    {
                        var response = await _unidadEjecutoraAPI.FindByIdAsync(id);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Id Unidad Ejecutora no existe.");

                RuleFor(x => x.FormDto.TipoDocumentoId)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Id Tipo Documento es requerido")
                   .Custom((x, context) =>
                   {
                       if (x < 1)
                       {
                           context.AddFailure($"Id Tipo Documento no debe ser {x}");
                       }
                   })
                   .MustAsync(async (id, cancellation) =>
                   {
                       var response = await _tipoDocumentoAPI.FindByIdAsync(id);
                       bool exists = response.Success;
                       return exists;
                   }).WithMessage("Id Tipo Documento no existe");

                RuleFor(x => x.FormDto.Serie)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Serie es requerido")
                    .MaximumLength(ParametroConsts.SerieMaxLength)
                    .WithMessage($"La longitud de la Serie debe tener {ParametroConsts.SerieMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                        {
                            context.AddFailure("La Serie contiene caracter no válido");
                        }
                    });

                RuleFor(x => x.FormDto.Correlativo)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Correlativo es requerido")
                    .MaximumLength(ParametroConsts.CorrelativoMaxLength)
                    .WithMessage($"La longitud del Correlativo debe tener {ParametroConsts.CorrelativoMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                        {
                            context.AddFailure("El Correlativo debe ser númerico");
                        }
                    });

                RuleFor(x => x.FormDto.Estado).NotNull().WithMessage("Estado es requerido");

                RuleFor(x => x.FormDto.UsuarioCreador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Creador es requerido")
                    .MaximumLength(ParametroConsts.UsuarioCreadorMaxLength)
                    .WithMessage($"La longitud del Usuario Creador debe tener {ParametroConsts.UsuarioCreadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IParametroRepository _repository;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly IMapper _mapper;

            public Handler(IParametroRepository repository,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                IMapper mapper)
            {
                _repository = repository;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_tipoDocumentoAPI, _unidadEjecutoraAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {
                        var parametro = new Parametro();
                        parametro = _mapper.Map<ParametroFormDto, Parametro>(request.FormDto);
                        parametro.Estado = true;
                        parametro.FechaCreacion = DateTime.Now;

                        var exists = await _repository.VerifyExists(Definition.INSERT, parametro);

                        if (exists)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                            response.Success = false;
                            return response;
                        }

                        parametro = await _repository.Add(parametro);

                        if (parametro != null)
                        {
                            response.Data = _mapper.Map<Parametro, ParametroFormDto>(parametro);
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
