using AutoMapper;
using FluentValidation;
using MediatR;
using RecaudacionApiRegistroLinea.Application.Command.Dtos;
using RecaudacionApiRegistroLinea.Clients;
using RecaudacionApiRegistroLinea.DataAccess;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionApiRegistroLinea.Helpers;
using RecaudacionUtils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RecaudacionApiRegistroLinea.Application.Command
{
    public class UpdateRegistroLineaHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public RegistroLineaFormDto FormDto { get; set; }
        }

        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("Id Registro Linea es requerido")
                   .Custom((x, context) =>
                   {
                       if (x < 1)
                       {
                           context.AddFailure($"Id Registro Linea no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.NumeroDeposito)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Número Depósito es requerido")
                  .MaximumLength(RegistroLineaConsts.NumeroDepositoMaxLength)
                  .WithMessage($"La longitud del Número Depósito debe tener {RegistroLineaConsts.NumeroDepositoMaxLength} caracteres o menos")
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
                   .NotNull().WithMessage("Fecha Depósito es requerido")
                   .Custom((x, context) =>
                   {
                       if (x != null)
                           if (DateTime.Compare(new DateTime(x.Year, x.Month, x.Day, 0, 0, 0), DateTime.Today) > 0)
                           {
                               var date = DateTime.Today.ToString("dd/MM/yyyy");
                               context.AddFailure($"Fecha Depósito no puede ser mayor a {date}");
                           }
                   });

                RuleFor(x => x.FormDto.ValidarDeposito)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Validación del depósito de cuenta corriente es requerido")
                  .MaximumLength(RegistroLineaConsts.ValidarDepositoMaxLength)
                  .WithMessage($"La longitud de validación del depósito debe tener {RegistroLineaConsts.ValidarDepositoMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!String.IsNullOrEmpty(x))
                          if (x != Definition.VALIDAR_DEPOSITO_PENDIENTE && x != Definition.VALIDAR_DEPOSITO_SI && x != Definition.VALIDAR_DEPOSITO_NO)
                          {
                              context.AddFailure("Validación del depósito de cuenta corriente debe ser (Pendiente,Si,No)");
                          }
                  });

                RuleFor(x => x.FormDto.NumeroOficio)
                    .Cascade(CascadeMode.Stop)
                    .MaximumLength(RegistroLineaConsts.NumeroOficioMaxLength)
                    .WithMessage($"La longitud del Número Depósito debe tener {RegistroLineaConsts.NumeroOficioMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-z0-9 -]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Número Oficio contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.NumeroComprobantePago)
                    .Cascade(CascadeMode.Stop)
                    .MaximumLength(RegistroLineaConsts.NumeroComprobantePagoMaxLength)
                    .WithMessage($"La longitud del Número Comprobante Pago debe tener {RegistroLineaConsts.NumeroComprobantePagoMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-z0-9 -]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Número Comprobante Pago contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.ExpedienteSiaf)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(RegistroLineaConsts.ExpedienteSiafMaxLength)
                   .WithMessage($"La longitud del Expediente SIAF debe tener {RegistroLineaConsts.ExpedienteSiafMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!string.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-z0-9 -]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Expediente SIAF contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.NumeroResolucion)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(RegistroLineaConsts.NumeroResolucionMaxLength)
                   .WithMessage($"La longitud del Número Resolución debe tener {RegistroLineaConsts.NumeroResolucionMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!string.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-z0-9 -]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Número Resolución contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.ExpedienteESinad)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(RegistroLineaConsts.ExpedienteEsinadMaxLength)
                   .WithMessage($"La longitud del Expediente SINAD debe tener {RegistroLineaConsts.ExpedienteEsinadMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!string.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-z0-9 -]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Expediente SINAD contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.NumeroESinad)
                 .Cascade(CascadeMode.Stop)
                 .Custom((x, context) =>
                 {
                     if (x < 0)
                     {
                         context.AddFailure($"Número SINAD no debe ser {x}");
                     }
                 });

                RuleFor(x => x.FormDto.Observacion)
                  .Cascade(CascadeMode.Stop)
                  .MaximumLength(RegistroLineaConsts.ObservacionMaxLength)
                  .WithMessage($"La longitud de la Observación debe tener {RegistroLineaConsts.ObservacionMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!string.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]", string.Empty).Length > 0)
                          {
                              context.AddFailure("La Observación contiene caracter no válido");
                          }
                  });

                RuleFor(x => x.FormDto.ImporteDeposito)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Importe Depósito es requerido")
                  .Custom((x, context) =>
                  {
                      if (x <= 0)
                      {
                          context.AddFailure($"Importe Depósito no debe ser {x}");
                      }
                  });

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(RegistroLineaConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {RegistroLineaConsts.UsuarioModificadorMaxLength} caracteres o menos");

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
                RuleFor(x => x.FormDto.RegistroLineaDetalle)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("Detalle de Registro Linea es requerido")
                   .Custom((x, context) =>
                   {
                       if (x.Count == 0)
                       {
                           context.AddFailure($"Detalle de Registro Linea es requerido");
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
                               .MaximumLength(RegistroLineaDetalleConsts.ReferenciaMaxLength)
                               .WithMessage($"Detalle: La longitud de la Referencia debe tener {RegistroLineaDetalleConsts.ReferenciaMaxLength} caracteres o menos");

                       });
                   });
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            public CommandValidator(
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IEstadoAPI estadoAPI)
            {
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _estadoAPI = estadoAPI;

                Include(new GeneralValidator());

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
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_REGISTRO_LINEA, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Número de estado no existe");

            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
        {
            private readonly IRegistroLineaRepository _repository;
            private readonly IRegistroLineaDetalleRepository _detalleRepository;
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IDepositoBancoAPI _depositoBancoAPI;
            private readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;
            private readonly IMapper _mapper;
            public Handler(IRegistroLineaRepository repository,
                IRegistroLineaDetalleRepository detalleRepository,
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IEstadoAPI estadoAPI,
                IDepositoBancoAPI depositoBancoAPI,
                IClasificadorIngresoAPI clasificadorIngresoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _estadoAPI = estadoAPI;
                _depositoBancoAPI = depositoBancoAPI;
                _clasificadorIngresoAPI = clasificadorIngresoAPI;
                _mapper = mapper;

            }

            public async Task<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_bancoAPI, _cuentaCorrienteAPI, _tipoReciboIngresoAPI, _estadoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                    }

                    var registroLinea = _mapper.Map<RegistroLineaFormDto, RegistroLinea>(request.FormDto);

                    var detalles = _mapper.Map<List<RegistroLineaDetalle>>(request.FormDto.RegistroLineaDetalle);

                    if (registroLinea.Estado == Definition.REGISTRO_LINEA_ESTADO_EMITIDO && detalles.Count > 0)
                    {

                        var totalDetalle = _detalleRepository.SumImporte(detalles);

                        if (registroLinea.ImporteDeposito != totalDetalle)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "El importe total del detalle no es igual al importe del depósito"));
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

                    var registroLineaEx = await _repository.FindById(request.Id);

                    if (registroLineaEx == null || (request.Id != request.FormDto.RegistroLineaId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    if (registroLinea.ValidarDeposito == Definition.VALIDAR_DEPOSITO_SI && registroLinea.Estado == Definition.REGISTRO_LINEA_ESTADO_EN_PROCESO)
                    {
                        var responseDepositoDetalle = await _depositoBancoAPI.FindDetalleByIdAsync(registroLinea.DepositoBancoDetalleId ?? 0);
                        if (!responseDepositoDetalle.Success)
                        {
                            response.Messages.Add(new GenericMessage(responseDepositoDetalle.Messages[0].Type, $"Servicio de Depósito de bancos 1: {responseDepositoDetalle.Messages[0].Message}"));
                            response.Success = false;
                            return response;
                        }

                        var depositobancoDetalle = responseDepositoDetalle.Data;
                        depositobancoDetalle.SerieDocumento = "-";
                        depositobancoDetalle.NumeroDocumento = registroLinea.Numero;
                        depositobancoDetalle.TipoDocumento = registroLinea.TipoDocumentoId;
                        depositobancoDetalle.FechaDocumento = registroLinea.FechaRegistro;
                        depositobancoDetalle.Utilizado = Definition.DEPOSITO_BANCO_DETALLE_UTILIZADO_SI;
                        depositobancoDetalle.UsuarioModificador = registroLinea.UsuarioModificador;
                        var reponseDepositobancoDetalleUpt = await _depositoBancoAPI.UpdateDetalleAsync(depositobancoDetalle.DepositoBancoDetalleId, depositobancoDetalle);
                        if (!reponseDepositobancoDetalleUpt.Success)
                        {
                            response.Messages.Add(new GenericMessage(reponseDepositobancoDetalleUpt.Messages[0].Type, $"Servicio de Depósito de bancos: {reponseDepositobancoDetalleUpt.Messages[0].Message}"));
                            response.Success = false;
                            return response;
                        }

                    }

                    registroLinea.FechaModificacion = DateTime.Now;
                    await _repository.Update(registroLinea);

                    if (registroLinea.Estado == Definition.REGISTRO_LINEA_ESTADO_EMITIDO)
                    {
                        if (detalles.Count > 0)
                        {
                            var detallesEx = await _detalleRepository.FindAll(registroLinea.RegistroLineaId);

                            foreach (var item in detallesEx)
                            {
                                await _detalleRepository.Delete(item);
                            }

                            foreach (var detalle in detalles)
                            {
                                detalle.RegistroLineaId = registroLinea.RegistroLineaId;
                                detalle.Estado = "1";
                                detalle.UsuarioCreador = registroLinea.UsuarioModificador;
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
