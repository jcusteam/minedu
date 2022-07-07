using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiParametro.Application.Command.Dtos;
using RecaudacionApiParametro.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiParametro.Domain;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiParametro.Clients;
using RecaudacionApiParametro.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiParametro.Application.Command
{
    public class UpdateParametroHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
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


                RuleFor(x => x.Id).NotEmpty().WithMessage("Id es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Parametro no debe ser {x}");
                        }
                    });

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
                    }).WithMessage("Id Unidad Ejecutora no existe");

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
                   }).WithMessage("Id Tipo Documento no existe.");

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

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(ParametroConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {ParametroConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }
        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
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

            public async Task<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_tipoDocumentoAPI, _unidadEjecutoraAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {

                        var parametroEx = await _repository.FindById(request.Id);
                        if (parametroEx == null || (request.Id != request.FormDto.ParametroId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }


                        var parametro = _mapper.Map<ParametroFormDto, Parametro>(request.FormDto);
                        parametro.FechaModificacion = DateTime.Now;

                        var exists = await _repository.VerifyExists(Definition.UPDATE, parametro);

                        if (exists)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                            response.Success = false;
                            return response;
                        }

                        await _repository.Update(parametro);
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
