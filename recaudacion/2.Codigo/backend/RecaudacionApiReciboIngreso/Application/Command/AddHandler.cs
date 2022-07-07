using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Application.Command.Dtos;
using RecaudacionApiReciboIngreso.DataAccess;
using RecaudacionApiReciboIngreso.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionApiReciboIngreso.Clients;
using RecaudacionUtils;
using RecaudacionApiReciboIngreso.Helpers;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace RecaudacionApiReciboIngreso.Application.Command
{
    public class AddHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddResponse>
        {
            public ReciboIngresoFormDto FormDto { get; set; }
        }

        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
                RuleFor(x => x.FormDto.NumeroDeposito)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().When(x => x.FormDto.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA)
                    .WithMessage("Número Depósito es requerido")
                    .MaximumLength(ReciboIngresoConsts.NumeroDepositoMaxLength)
                    .WithMessage($"La longitud del Número Depósito debe tener {ReciboIngresoConsts.NumeroDepositoMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Número Depósito contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.FechaDeposito)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().When(x => x.FormDto.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA)
                    .WithMessage("Fecha Depósito es requerido")
                    .Custom((x, context) =>
                    {
                        if (x != null)
                            if (DateTime.Compare(new DateTime(x.Value.Year, x.Value.Month, x.Value.Day, 0, 0, 0), DateTime.Today) > 0)
                            {
                                var date = DateTime.Today.ToString("dd/MM/yyyy");
                                context.AddFailure($"Fecha Depósito no puede ser mayor a {date}");
                            }
                    });

                RuleFor(x => x.FormDto.ValidarDeposito)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().When(x => x.FormDto.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA).WithMessage("Validar Depósito es requerido")
                  .MaximumLength(ReciboIngresoConsts.ValidarDepositoMaxLength)
                  .WithMessage($"La longitud de Validación de cuenta corriente no debe tener {ReciboIngresoConsts.ValidarDepositoMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!string.IsNullOrEmpty(x))
                          if (x != Definition.VALIDAR_DEPOSITO_SI)
                          {
                              context.AddFailure($"Se debe validar el depósito de cuenta corriente");
                          }
                  });

                RuleFor(x => x.FormDto.NumeroCheque)
                    .Cascade(CascadeMode.Stop)
                    .MaximumLength(ReciboIngresoConsts.NumeroChequeMaxLength)
                    .WithMessage($"La longitud del Número Cheque debe tener {ReciboIngresoConsts.NumeroChequeMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Número Cheque contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.NumeroOficio)
                    .Cascade(CascadeMode.Stop)
                    .MaximumLength(ReciboIngresoConsts.NumeroOficioMaxLength)
                    .WithMessage($"La longitud del Número Depósito debe tener {ReciboIngresoConsts.NumeroOficioMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Número Oficio contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.NumeroComprobantePago)
                    .Cascade(CascadeMode.Stop)
                    .MaximumLength(ReciboIngresoConsts.NumeroComprobantePagoMaxLength)
                    .WithMessage($"La longitud del Número Comprobante Pago debe tener {ReciboIngresoConsts.NumeroComprobantePagoMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Número Comprobante Pago contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.ExpedienteSiaf)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ReciboIngresoConsts.ExpedienteSiafMaxLength)
                   .WithMessage($"La longitud del Expediente SIAF debe tener {ReciboIngresoConsts.ExpedienteSiafMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!string.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Expediente SIAF contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.NumeroResolucion)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ReciboIngresoConsts.NumeroResolucionMaxLength)
                   .WithMessage($"La longitud del Número Resolución debe tener {ReciboIngresoConsts.NumeroResolucionMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!string.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Número Resolución contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.CartaOrden)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ReciboIngresoConsts.CartaOrdenMaxLength)
                   .WithMessage($"La longitud de la Carta Orden debe tener {ReciboIngresoConsts.CartaOrdenMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!string.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                           {
                               context.AddFailure("La Carta Orden contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.LiquidacionIngreso)
                  .Cascade(CascadeMode.Stop)
                  .MaximumLength(ReciboIngresoConsts.LiquidacionIngresoMaxLength)
                  .WithMessage($"La longitud de la Liquidación de ingreso debe tener {ReciboIngresoConsts.LiquidacionIngresoMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!string.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                          {
                              context.AddFailure("La Liquidación de ingreso contiene caracter no válido");
                          }
                  });

                RuleFor(x => x.FormDto.PapeletaDeposito)
                  .Cascade(CascadeMode.Stop)
                  .MaximumLength(ReciboIngresoConsts.PapeletaDepositoMaxLength)
                  .WithMessage($"La longitud de la Papeleta Depósito de ingreso debe tener {ReciboIngresoConsts.PapeletaDepositoMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!string.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                          {
                              context.AddFailure("La Papeleta Depósito contiene caracter no válido");
                          }
                  });

                RuleFor(x => x.FormDto.Concepto)
                    .Cascade(CascadeMode.Stop)
                    .MaximumLength(ReciboIngresoConsts.ConceptoMaxLength)
                    .WithMessage($"La longitud del Concepto de ingreso debe tener {ReciboIngresoConsts.ConceptoMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                     {
                         if (!string.IsNullOrEmpty(x))
                             if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]", string.Empty).Length > 0)
                             {
                                 context.AddFailure("El Concepto contiene caracter no válido");
                             }
                     });

                RuleFor(x => x.FormDto.Referencia)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ReciboIngresoConsts.ConceptoMaxLength)
                   .WithMessage($"La longitud de la Referencia debe tener {ReciboIngresoConsts.ConceptoMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!string.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]", string.Empty).Length > 0)
                           {
                               context.AddFailure("La Referencia contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.ImporteTotal)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Importe Total es requerido")
                  .Custom((x, context) =>
                  {
                      if (x <= 0)
                      {
                          context.AddFailure($"Importe Total no debe ser {x}");
                      }
                  })
                  .Must(x => !(x > ReciboIngresoConsts.ImporteTotalMax)).WithMessage($"Importe Total no debe ser mayor a  {Tools.ToFormat(ReciboIngresoConsts.ImporteTotalMax.ToString())}");


                RuleFor(x => x.FormDto.UsuarioCreador)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Usuario Creador es requerido")
                   .MaximumLength(ReciboIngresoConsts.UsuarioCreadorMaxLength)
                   .WithMessage($"La longitud del Usuario Creador debe tener {ReciboIngresoConsts.UsuarioCreadorMaxLength} caracteres o menos");

            }
        }

        public class DetallesValidator : AbstractValidator<Command>
        {
            private readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;
            public DetallesValidator(
                IClasificadorIngresoAPI clasificadorIngresoAPI)
            {
                _clasificadorIngresoAPI = clasificadorIngresoAPI;
                // Detalle
                RuleFor(x => x.FormDto.ReciboIngresoDetalle)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("Detalle de Recibo Ingreso es requerido")
                 .Custom((x, context) =>
                 {
                     if (x.Count == 0)
                     {
                         context.AddFailure($"Detalle de Recibo Ingreso es requerido");
                     }
                 })
                 .ForEach(detalles =>
                 {
                     detalles.ChildRules(detalle =>
                     {
                         detalle.RuleFor(x => x.ClasificadorIngresoId)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty().WithMessage("Detalle: Id Clasificador Ingreso es requerido")
                            .Custom((x, context) =>
                            {
                                if (x < 1)
                                {
                                    context.AddFailure($"Detalle: Id Clasificador Ingreso no debe ser {x}");
                                }
                            })
                            .MustAsync(async (id, cancellation) =>
                            {
                                var response = await _clasificadorIngresoAPI.FindByIdAsync(id);
                                bool exists = response.Success;
                                return exists;
                            }).WithMessage("Detalle: Id Clasificador Ingreso no existe");

                         detalle.RuleFor(x => x.Importe)
                             .Cascade(CascadeMode.Stop)
                             .NotEmpty().WithMessage("Detalle: Importe es requerido")
                             .Custom((x, context) =>
                             {
                                 if (x <= 0)
                                 {
                                     context.AddFailure($"Detalle: Importe no debe ser {x}");
                                 }
                             });

                         detalle.RuleFor(x => x.Referencia)
                             .Cascade(CascadeMode.Stop)
                             .MaximumLength(ReciboIngresoDetalleConsts.ReferenciaMaxLength)
                             .WithMessage($"Detalle: La longitud de la Referencia debe tener {ReciboIngresoDetalleConsts.ReferenciaMaxLength} caracteres o menos");

                     });
                 });
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IFuenteFinanciamientoAPI _fuenteFinanciamientoAPI;
            private readonly ITipoCaptacionAPI _tipoCaptacionAPI;
            private readonly IClienteAPI _clienteAPI;
            public CommandValidator(
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IFuenteFinanciamientoAPI fuenteFinanciamientoAPI,
                ITipoCaptacionAPI tipoCaptacionAPI,
                IClienteAPI clienteAPI)
            {
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _fuenteFinanciamientoAPI = fuenteFinanciamientoAPI;
                _tipoCaptacionAPI = tipoCaptacionAPI;
                _clienteAPI = clienteAPI;

                Include(new GeneralValidator());

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

                RuleFor(x => x.FormDto.TipoReciboIngresoId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Tipo Recibo Ingreso es requerido")
                    .Custom((x, context) =>
                   {
                       if (x < 1)
                       {
                           context.AddFailure($"Id Tipo Recibo Ingreso no debe ser {x}");
                       }
                   })
                    .MustAsync(async (id, cancellation) =>
                        {
                            var response = await _tipoReciboIngresoAPI.FindByIdAsync(id);
                            bool exists = response.Success;
                            return exists;
                        }).WithMessage("Id Tipo Recibo Ingreso no existe");



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

                RuleFor(x => x.FormDto.CuentaCorrienteId)
                    .Cascade(CascadeMode.Stop)
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

                RuleFor(x => x.FormDto.RegistroLineaId)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x != null)
                           if (x < 1)
                           {
                               context.AddFailure($"Id Registro Linea no debe ser {x}");
                           }
                   });

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

                RuleFor(x => x.FormDto.TipoCaptacionId)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Id Tipo Captación es requerido")
                  .Custom((x, context) =>
                  {
                      if (x < 1)
                      {
                          context.AddFailure($"Id Tipo Captación no debe ser {x}");
                      }
                  })
                  .MustAsync(async (id, cancellation) =>
                  {
                      var response = await _tipoCaptacionAPI.FindByIdAsync(id);
                      bool exists = response.Success;
                      return exists;
                  }).WithMessage("Id Tipo Captación no existe");

                RuleFor(x => x.FormDto.LiquidacionId)
                  .Cascade(CascadeMode.Stop)
                  .Custom((x, context) =>
                  {
                      if (x != null)
                          if (x < 1)
                          {
                              context.AddFailure($"Id Liquidacion no debe ser {x}");
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
                       var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_RECIBO_INGRESO, estado);
                       bool exists = response.Success;
                       return exists;
                   }).WithMessage("Número de estado no existe");
            }
        }

        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IReciboIngresoRepository _repository;
            private readonly IReciboIngresoDetalleRepository _detalleRepository;
            private readonly IDepositoBancoAPI _depositoBancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;
            private readonly IFuenteFinanciamientoAPI _fuenteFinanciamientoAPI;
            private readonly ITipoCaptacionAPI _tipoCaptacionAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly IMapper _mapper;

            public Handler(IReciboIngresoRepository repository,
                IReciboIngresoDetalleRepository detalleRepository,
                IDepositoBancoAPI depositoBancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IClasificadorIngresoAPI clasificadorIngresoAPI,
                IFuenteFinanciamientoAPI fuenteFinanciamientoAPI,
                ITipoCaptacionAPI tipoCaptacionAPI,
                IClienteAPI clienteAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _depositoBancoAPI = depositoBancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _clasificadorIngresoAPI = clasificadorIngresoAPI;
                _fuenteFinanciamientoAPI = fuenteFinanciamientoAPI;
                _tipoCaptacionAPI = tipoCaptacionAPI;
                _clienteAPI = clienteAPI;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_cuentaCorrienteAPI, _tipoReciboIngresoAPI,
                        _estadoAPI, _unidadEjecutoraAPI, _tipoDocumentoAPI,
                        _fuenteFinanciamientoAPI, _tipoCaptacionAPI, _clienteAPI);

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
                    DetallesValidator detalleValidations = new DetallesValidator(_clasificadorIngresoAPI);
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

                    var reciboIngreso = _mapper.Map<ReciboIngresoFormDto, ReciboIngreso>(request.FormDto);

                    var detalles = _mapper.Map<List<ReciboIngresoDetalle>>(request.FormDto.ReciboIngresoDetalle);

                    var totalDetalle = _detalleRepository.SumImporte(detalles);

                    if (reciboIngreso.ImporteTotal != totalDetalle)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "El 'importe total' del detalle no es igual al 'importe total' del recibo de ingreso"));
                        response.Success = false;
                        return response;
                    }

                    reciboIngreso.TipoDocumentoId = Definition.TIPO_DOCUMENTO_RECIBO_INGRESO;
                    var numero = await _repository.FindCorrelativo(reciboIngreso.UnidadEjecutoraId, reciboIngreso.TipoDocumentoId);
                    reciboIngreso.Numero = numero;
                    reciboIngreso.FechaEmision = DateTime.Now;
                    reciboIngreso.FechaCreacion = DateTime.Now;
                    reciboIngreso = await _repository.Add(reciboIngreso);

                    if (reciboIngreso != null)
                    {

                        foreach (var item in request.FormDto.ReciboIngresoDetalle)
                        {
                            var detalle = _mapper.Map<ReciboIngresoDetalleFormDto, ReciboIngresoDetalle>(item);
                            detalle.ReciboIngresoId = reciboIngreso.ReciboIngresoId;
                            detalle.Estado = "1";
                            detalle.UsuarioCreador = reciboIngreso.UsuarioCreador;
                            detalle.FechaCreacion = DateTime.Now;
                            await _detalleRepository.Add(detalle);

                        }

                        response.Data = reciboIngreso;
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_INSERT));

                        if ((reciboIngreso.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA
                            && reciboIngreso.ValidarDeposito == Definition.VALIDAR_DEPOSITO_SI) && reciboIngreso.RegistroLineaId == null)
                        {
                            var responseDepositoBancoDetalle = await _depositoBancoAPI.FindDetalleByIdAsync(reciboIngreso.DepositoBancoDetalleId ?? 0);
                            var depositoBancoDetalle = responseDepositoBancoDetalle.Data;
                            depositoBancoDetalle.TipoDocumento = reciboIngreso.TipoDocumentoId;
                            depositoBancoDetalle.NumeroDocumento = reciboIngreso.Numero;
                            depositoBancoDetalle.FechaDocumento = reciboIngreso.FechaEmision;
                            depositoBancoDetalle.Utilizado = Definition.DEPOSITO_BANCO_DETALLE_UTILIZADO_SI;
                            depositoBancoDetalle.UsuarioModificador = reciboIngreso.UsuarioCreador;
                            await _depositoBancoAPI.UpdateDetalleAsync(depositoBancoDetalle.DepositoBancoDetalleId, depositoBancoDetalle);

                        }


                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                        response.Success = false;
                    }
                }
                catch (System.Exception e)
                {
                    //response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
