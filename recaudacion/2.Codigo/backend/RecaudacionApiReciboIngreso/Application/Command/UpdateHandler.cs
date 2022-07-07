using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Application.Command.Dtos;
using RecaudacionApiReciboIngreso.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiReciboIngreso.Domain;
using AutoMapper;
using RecaudacionApiReciboIngreso.Clients;
using RecaudacionUtils;
using RecaudacionApiReciboIngreso.Helpers;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace RecaudacionApiReciboIngreso.Application.Command
{
    public class UpdateHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public ReciboIngresoFormDto FormDto { get; set; }
        }

        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
                RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Id Recibo Ingreso es requerido")
                .Custom((x, context) =>
                {
                    if (x < 1)
                    {
                        context.AddFailure($"Id Recibo Ingreso no debe ser {x}");
                    }
                });

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

                RuleFor(x => x.FormDto.UsuarioModificador)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Usuario Modificador es requerido")
                   .MaximumLength(ReciboIngresoConsts.UsuarioModificadorMaxLength)
                   .WithMessage($"La longitud del Usuario Modificador debe tener {ReciboIngresoConsts.UsuarioModificadorMaxLength} caracteres o menos");

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
            private readonly IFuenteFinanciamientoAPI _fuenteFinanciamientoAPI;
            private readonly ITipoCaptacionAPI _tipoCaptacionAPI;
            private readonly IClienteAPI _clienteAPI;
            public CommandValidator(
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IEstadoAPI estadoAPI,
                IFuenteFinanciamientoAPI fuenteFinanciamientoAPI,
                ITipoCaptacionAPI tipoCaptacionAPI,
                IClienteAPI clienteAPI)
            {
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _estadoAPI = estadoAPI;
                _fuenteFinanciamientoAPI = fuenteFinanciamientoAPI;
                _tipoCaptacionAPI = tipoCaptacionAPI;
                _clienteAPI = clienteAPI;

                Include(new GeneralValidator());

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
                    }).WithMessage("Id Tipo Recibo Ingreso no existe.");

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
                    }).WithMessage("Id Cliente no existe.");

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
                    }).WithMessage("Id Cuenta Corriente no existe.");

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
                    }).WithMessage("Id Fuente Financiamiento no existe.");

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
                  }).WithMessage("Id Tipo Captación no existe.");

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



        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
        {
            private readonly IReciboIngresoRepository _repository;
            private readonly IReciboIngresoDetalleRepository _detalleRepository;
            private readonly IDepositoBancoAPI _depositoBancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IFuenteFinanciamientoAPI _fuenteFinanciamientoAPI;
            private readonly ITipoCaptacionAPI _tipoCaptacionAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;
            private readonly IMapper _mapper;

            public Handler(IReciboIngresoRepository repository,
                IReciboIngresoDetalleRepository detalleRepository,
                IDepositoBancoAPI depositoBancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IEstadoAPI estadoAPI,
                IFuenteFinanciamientoAPI fuenteFinanciamientoAPI,
                ITipoCaptacionAPI tipoCaptacionAPI,
                IClienteAPI clienteAPI,
                IClasificadorIngresoAPI clasificadorIngresoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _depositoBancoAPI = depositoBancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _estadoAPI = estadoAPI;
                _fuenteFinanciamientoAPI = fuenteFinanciamientoAPI;
                _tipoCaptacionAPI = tipoCaptacionAPI;
                _clienteAPI = clienteAPI;
                _clasificadorIngresoAPI = clasificadorIngresoAPI;
                _mapper = mapper;
            }

            public async Task<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_cuentaCorrienteAPI, _tipoReciboIngresoAPI,
                         _estadoAPI, _fuenteFinanciamientoAPI, _tipoCaptacionAPI, _clienteAPI);

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

                    var reciboIngreso = _mapper.Map<ReciboIngresoFormDto, ReciboIngreso>(request.FormDto);

                    var detalles = _mapper.Map<List<ReciboIngresoDetalle>>(request.FormDto.ReciboIngresoDetalle);

                    if (reciboIngreso.Estado == Definition.RECIBO_INGRESO_ESTADO_EMITIDO && detalles.Count > 0)
                    {

                        var totalDetalle = _detalleRepository.SumImporte(detalles);

                        if (reciboIngreso.ImporteTotal != totalDetalle)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "El 'importe total' del detalle no es igual al 'importe total' del recibo de ingreso"));
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
                    }



                    var reciboIngresoEx = await _repository.FindById(request.Id);

                    if (reciboIngresoEx == null || (request.Id != request.FormDto.ReciboIngresoId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }


                    reciboIngreso.FechaModificacion = DateTime.Now;
                    await _repository.Update(reciboIngreso);

                    if (reciboIngreso.Estado == Definition.RECIBO_INGRESO_ESTADO_EMITIDO)
                    {
                        if (detalles.Count > 0)
                        {
                            var detallesEx = await _detalleRepository.FindAll(reciboIngreso.ReciboIngresoId);

                            foreach (var item in detallesEx)
                            {
                                await _detalleRepository.Delete(item);
                            }

                            foreach (var detalle in detalles)
                            {
                                detalle.ReciboIngresoId = reciboIngreso.ReciboIngresoId;
                                detalle.UsuarioCreador = reciboIngreso.UsuarioModificador;
                                detalle.Estado = "1";
                                detalle.FechaCreacion = DateTime.Now;
                                await _detalleRepository.Add(detalle);
                            }
                        }
                    }

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
