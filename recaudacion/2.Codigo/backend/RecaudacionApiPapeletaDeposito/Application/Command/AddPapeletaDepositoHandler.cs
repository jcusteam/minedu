using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Application.Command.Dtos;
using RecaudacionApiPapeletaDeposito.DataAccess;
using RecaudacionApiPapeletaDeposito.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiPapeletaDeposito.Clients;
using RecaudacionApiPapeletaDeposito.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiPapeletaDeposito.Application.Command
{
    public class AddPapeletaDepositoHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddResponse>
        {
            public PapeletaDepositoFormDto FormDto { get; set; }
        }

        public class DetallesValidator : AbstractValidator<Command>
        {
            private readonly IReciboIngresoAPI _reciboIngresoAPI;
            public DetallesValidator(
                IReciboIngresoAPI reciboIngresoAPI)
            {
                _reciboIngresoAPI = reciboIngresoAPI;

                // Detalle
                RuleFor(x => x.FormDto.PapeletaDepositoDetalle)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("Detalle de Papeleta de Depósito es requerido")
                 .Custom((x, context) =>
                 {
                     if (x.Count == 0)
                     {
                         context.AddFailure($"Detalle de Papeleta de Depósito es requerido");
                     }
                 })
                 .ForEach(detalles =>
                 {
                     detalles.ChildRules(detalle =>
                     {
                         detalle.RuleFor(x => x.ReciboIngresoId)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Id Recibo Ingreso es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 1)
                                    {
                                        context.AddFailure($"Detalle: Id Recibo Ingreso no debe ser {x}");
                                    }
                                })
                                .MustAsync(async (id, cancellation) =>
                                {
                                    var response = await _reciboIngresoAPI.FindByIdAsync(id);
                                    bool exists = response.Success;
                                    return exists;
                                }).WithMessage("Detalle: Id Recibo Ingreso no existe ");

                         detalle.RuleFor(x => x.Monto)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Monto es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Monto no debe ser {x}");
                                    }
                                });

                     });
                 });

            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;

            public CommandValidator(
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI)
            {
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;

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

                RuleFor(x => x.FormDto.BancoId)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Id Banco es requerido")
                   .Custom((x, context) =>
                   {
                       if (x < 1)
                       {
                           context.AddFailure($"Id Banco no debe ser {x}");
                       }
                   })
                   .MustAsync(async (id, cancellation) =>
                   {
                       var response = await _bancoAPI.FindByIdAsync(id);
                       bool exists = response.Success;
                       return exists;
                   }).WithMessage("Id Banco no existe");

                RuleFor(x => x.FormDto.Descripcion)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Descripción es requerido")
                    .MaximumLength(PapeletaDepositoConsts.DescripcionMaxLength)
                    .WithMessage($"La longitud de la Descripción debe tener {PapeletaDepositoConsts.DescripcionMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Concepto contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.Estado)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Estado es requerido")
                    .Custom((x, context) =>
                    {
                        if (x != Definition.PAPELETA_DEPOSITO_ESTADO_EMITIDO)
                        {
                            context.AddFailure($"Estado no debe ser {x}");
                        }
                    })
                    .MustAsync(async (estado, cancellation) =>
                    {
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_PAPELETA_DEPOSITO, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Número de estado no existe.");

                RuleFor(x => x.FormDto.UsuarioCreador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Creador es requerido")
                    .MaximumLength(PapeletaDepositoConsts.UsuarioCreadorMaxLength)
                    .WithMessage($"La longitud del Usuario Creador debe tener {PapeletaDepositoConsts.UsuarioCreadorMaxLength} caracteres o menos");

            }
        }


        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IPapeletaDepositoRepository _repository;
            private readonly IPapeletaDepositoDetalleRepository _detalleRepository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IReciboIngresoAPI _reciboIngresoAPI;
            private readonly IMapper _mapper;

            public Handler(
                IPapeletaDepositoRepository repository,
                IPapeletaDepositoDetalleRepository detalleRepository,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IReciboIngresoAPI reciboIngresoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _reciboIngresoAPI = reciboIngresoAPI;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI, _unidadEjecutoraAPI,
                        _tipoDocumentoAPI, _bancoAPI, _cuentaCorrienteAPI);

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

                    // Detalle validación
                    DetallesValidator detalleValidations = new DetallesValidator(_reciboIngresoAPI);
                    var detalleResult = await detalleValidations.ValidateAsync(request);

                    if (!detalleResult.IsValid)
                    {
                        foreach (var item in detalleResult.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                            response.Success = false;
                            return response;
                        }
                    }

                    foreach (var item in request.FormDto.PapeletaDepositoDetalle)
                    {

                        var reciboIngresoResponse = await _reciboIngresoAPI.FindByIdAsync(item.ReciboIngresoId);

                        if (reciboIngresoResponse.Success)
                        {
                            if (reciboIngresoResponse.Data.Estado != Definition.RECIBO_INGRESO_ESTADO_TRANSMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"Detalle: El número '{reciboIngresoResponse.Data.Numero}' de recibo de ingreso debe estar en estado 'Transmitido'"));
                                response.Success = false;
                                return response;
                            }
                        }

                        var papeletaDepositoDetalle = await _detalleRepository.FindByReciboIngreso(item.ReciboIngresoId);

                        if (papeletaDepositoDetalle != null)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"Detalle: El número '{reciboIngresoResponse.Data.Numero}' de recibo de ingreso ya se encuentra registrado"));
                            response.Success = false;
                            return response;
                        }
                    }

                    var papeletaDeposito = _mapper.Map<PapeletaDepositoFormDto, PapeletaDeposito>(request.FormDto);
                    papeletaDeposito.TipoDocumentoId = Definition.TIPO_DOCUMENTO_PAPELETA_DEPOSITO;
                    var numero = await _repository.FindCorrelativo(papeletaDeposito.UnidadEjecutoraId, papeletaDeposito.TipoDocumentoId);
                    papeletaDeposito.Numero = numero;
                    papeletaDeposito.FechaCreacion = DateTime.Now;

                    papeletaDeposito = await _repository.Add(papeletaDeposito);

                    if (papeletaDeposito == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                        response.Success = false;
                        return response;
                    }

                    foreach (var item in request.FormDto.PapeletaDepositoDetalle)
                    {
                        var detalle = _mapper.Map<PapeletaDepositoDetalleFormDto, PapeletaDepositoDetalle>(item);
                        detalle.PapeletaDepositoId = papeletaDeposito.PapeletaDepositoId;
                        detalle.FechaCreacion = DateTime.Now;
                        detalle.UsuarioCreador = papeletaDeposito.UsuarioCreador;
                        await _detalleRepository.Add(detalle);
                    }

                    response.Data = papeletaDeposito;
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_INSERT));

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
