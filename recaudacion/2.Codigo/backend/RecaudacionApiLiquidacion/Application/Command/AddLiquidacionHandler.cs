using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Application.Command.Dtos;
using RecaudacionApiLiquidacion.DataAccess;
using RecaudacionApiLiquidacion.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiLiquidacion.Helpers;
using RecaudacionApiLiquidacion.Clients;

namespace RecaudacionApiLiquidacion.Application.Command
{
    public class AddLiquidacionHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddResponse>
        {
            public LiquidacionFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IFuenteFinanciamientoAPI _fuenteFinanciamientoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IClienteAPI _clienteAPI;

            public CommandValidator(
                IFuenteFinanciamientoAPI fuenteFinanciamientoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IClienteAPI clienteAPI)
            {
                _fuenteFinanciamientoAPI = fuenteFinanciamientoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _clienteAPI = clienteAPI;

                RuleFor(x => x.FormDto.UnidadEjecutoraId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Unidad Ejecutora es requerido")
                     .Custom((x, context) =>
                     {
                         if (x != Definition.ID_UE_024 && x != Definition.ID_UE_026 && x != Definition.ID_UE_116)
                         {
                             context.AddFailure($"Unidad Ejecutora permitido ({Definition.CODIGO_UE_024},{Definition.CODIGO_UE_026},{Definition.CODIGO_UE_116})");
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
                         if (x != Definition.TIPO_DOCUMENTO_LIQUIDACION)
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
                RuleFor(x => x.FormDto.FuenteFinanciamientoId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Fuente Financiamiento es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Fuente Financiamiento no debe ser {x}");
                        }
                    })
                    .MustAsync(async (id, cancellation) =>
                     {
                         var response = await _fuenteFinanciamientoAPI.FindByIdAsync(id);
                         bool exists = response.Success;
                         return exists;
                     }).WithMessage("Id Fuente Financiamiento no existe");

                RuleFor(x => x.FormDto.CuentaCorrienteId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Cuenta Corriente es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Cuenta Corriente no debe ser {x}");
                        }
                    })
                    .MustAsync(async (id, cancellation) =>
                     {
                         var response = await _cuentaCorrienteAPI.FindByIdAsync(id);
                         bool exists = response.Success;
                         return exists;
                     }).WithMessage("Id Cuenta Corriente no existe");

                RuleFor(x => x.FormDto.ClienteId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Cliente es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Cliente no debe ser {x}");
                        }
                    })
                    .MustAsync(async (id, cancellation) =>
                    {
                        var response = await _clienteAPI.FindByIdAsync(id);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Id Cliente no existe");

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
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_LIQUIDACION, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Número de estado no existe");

                RuleFor(x => x.FormDto.Procedencia)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Procedencia es requerido")
                    .MaximumLength(LiquidacionConsts.ProcedenciaMaxLength)
                    .WithMessage($"La longitud de la \"Procedencia\" debe tener {LiquidacionConsts.ProcedenciaMaxLength} caracteres o menos");


                RuleFor(x => x.FormDto.UsuarioCreador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Creador es requerido")
                    .MaximumLength(LiquidacionConsts.UsuarioCreadorMaxLength)
                    .WithMessage($"La longitud del Usuario Creador debe tener {LiquidacionConsts.UsuarioCreadorMaxLength} caracteres o menos");

            }
        }

        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly ILiquidacionRepository _repository;
            private readonly IFuenteFinanciamientoAPI _fuenteFinanciamientoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly IMapper _mapper;

            public Handler(ILiquidacionRepository repository,
                IFuenteFinanciamientoAPI fuenteFinanciamientoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IClienteAPI clienteAPI,
                IMapper mapper)
            {
                _repository = repository;
                _fuenteFinanciamientoAPI = fuenteFinanciamientoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _clienteAPI = clienteAPI;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(
                        _fuenteFinanciamientoAPI, _cuentaCorrienteAPI, _estadoAPI,
                        _unidadEjecutoraAPI, _tipoDocumentoAPI, _clienteAPI);

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

                    var liquidacion = _mapper.Map<LiquidacionFormDto, Liquidacion>(request.FormDto);
                    liquidacion.TipoDocumentoId = Definition.TIPO_DOCUMENTO_LIQUIDACION;
                    liquidacion.Estado = Definition.LIQUIDACION_ESTADO_EMITIDO;
                    liquidacion.FechaCreacion = DateTime.Now;

                    var totalDetalle = await _repository.CountDetalleByFecha();

                    if (totalDetalle == 0)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "No hay registros para generar la liquidación por captacion de ventanilla."));
                        response.Success = false;
                        return response;
                    }

                    var numero = await _repository.FindNumeroCorrelativo(liquidacion.UnidadEjecutoraId, liquidacion.TipoDocumentoId);
                    liquidacion.Numero = numero;
                    liquidacion = await _repository.Add(liquidacion);

                    if (liquidacion == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                        response.Success = false;
                        return response;

                    }

                    response.Data = _mapper.Map<Liquidacion, LiquidacionFormDto>(liquidacion);
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_INSERT));

                }
                catch (System.Exception e)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
