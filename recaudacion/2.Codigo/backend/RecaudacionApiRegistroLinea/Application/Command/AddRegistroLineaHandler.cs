using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Application.Command.Dtos;
using RecaudacionApiRegistroLinea.DataAccess;
using RecaudacionApiRegistroLinea.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionApiRegistroLinea.Clients;
using RecaudacionUtils;
using RecaudacionApiRegistroLinea.Helpers;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace RecaudacionApiRegistroLinea.Application.Command
{
    public class AddRegistroLineaHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusAddResponse>
        {
            public RegistroLineaFormDto FormDto { get; set; }
        }

        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
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

                RuleFor(x => x.FormDto.UsuarioCreador)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Usuario Creador es requerido")
                   .MaximumLength(RegistroLineaConsts.UsuarioCreadorMaxLength)
                   .WithMessage($"La longitud del Usuario Creador debe tener {RegistroLineaConsts.UsuarioCreadorMaxLength} caracteres o menos");

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

        public class ClienteValidator : AbstractValidator<Command>
        {
            public ClienteValidator()
            {

                RuleFor(x => x.FormDto.Cliente)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("Información personal es requerido")
                   .NotEmpty().WithMessage("Información personal es requerido");

                RuleFor(x => x.FormDto.Cliente.TipoDocumentoIdentidadId)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Id Tipo Documento Identidad es requerido")
                 .Custom((x, context) =>
                 {
                     if (x < 1)
                     {
                         context.AddFailure($"Id Tipo Documento Identidad no debe ser {x}");
                     }
                 });

                RuleFor(x => x.FormDto.Cliente.NumeroDocumento)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Número Documento es requerido")
                    .MaximumLength(RegistroLineaConsts.NumeroDocumentoClienteMaxLength)
                    .WithMessage($"La longitud del Número Documento debe tener {RegistroLineaConsts.NumeroDocumentoClienteMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!string.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Número de documento contiene caracter no válido.");
                            }
                    });


                RuleFor(x => x.FormDto.Cliente.Nombre)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Nombres y Apellidos ó Razón Social es requerido")
                    .MaximumLength(RegistroLineaConsts.NombreClienteMaxLength)
                    .WithMessage($"La longitud de Nombres y Apellidos ó Razón Social debe tener {RegistroLineaConsts.NombreClienteMaxLength} caracteres o menos");

                RuleFor(x => x.FormDto.Cliente.Correo)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Correo electrónico es requerido")
                    .MaximumLength(RegistroLineaConsts.CorreoClienteMaxLength)
                    .WithMessage($"La longitud del Correo electrónico debe tener {RegistroLineaConsts.CorreoClienteMaxLength} caracteres o menos")
                    .EmailAddress().WithMessage("La direccíon del Correo electrónico no es válida");

            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;

            public CommandValidator(
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI)
            {
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;

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
                   }).WithMessage("Id Banco no existe.");

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
                 }).WithMessage("Id Cuenta Corriente no existe.");

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


                // Cliente
                Include(new ClienteValidator());

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


        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IRegistroLineaRepository _repository;
            private readonly IRegistroLineaDetalleRepository _detalleRepository;
            private readonly IPideAPI _pideAPI;
            private readonly IESindadExpedienteAPI _expedienteAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;
            private readonly IMapper _mapper;

            public Handler(IRegistroLineaRepository repository,
                IRegistroLineaDetalleRepository detalleRepository,
                IPideAPI pideAPI,
                IClienteAPI clienteAPI,
                IESindadExpedienteAPI expedienteAPI,
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IClasificadorIngresoAPI clasificadorIngresoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _clienteAPI = clienteAPI;
                _detalleRepository = detalleRepository;
                _pideAPI = pideAPI;
                _expedienteAPI = expedienteAPI;
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _clasificadorIngresoAPI = clasificadorIngresoAPI;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_bancoAPI, _cuentaCorrienteAPI, _tipoReciboIngresoAPI,
                        _estadoAPI, _unidadEjecutoraAPI, _tipoDocumentoAPI);
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

                    var registroLinea = _mapper.Map<RegistroLineaFormDto, RegistroLinea>(request.FormDto);

                    var detalles = _mapper.Map<List<RegistroLineaDetalle>>(request.FormDto.RegistroLineaDetalle);

                    var totalDetalle = _detalleRepository.SumImporte(detalles);

                    if (registroLinea.ImporteDeposito != totalDetalle)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "El importe total del detalle no es igual al importe del depósito"));
                        response.Success = false;
                        return response;
                    }

                    var cliente = request.FormDto.Cliente;

                    switch (cliente.TipoDocumentoIdentidadId)
                    {
                        case Definition.TIPO_DOCUMENTO_IDENTIDAD_DNI:
                            var reniecResponse = await _pideAPI.FindReniecByDniAsync(cliente.NumeroDocumento);
                            if (!reniecResponse.Success)
                            {
                                response.Messages.Add(new GenericMessage(reniecResponse.Messages[0].Type, $"Servicio de Reniec: {reniecResponse.Messages[0].Message}"));
                                response.Success = false;
                                return response;
                            }
                            cliente.Nombre = reniecResponse.Data.nombreCompleto;
                            cliente.Direccion = reniecResponse.Data.domicilioApp;

                            break;
                        case Definition.TIPO_DOCUMENTO_IDENTIDAD_CE:
                            var migracionResponse = await _pideAPI.FindMigracionByNumeroAsync(cliente.NumeroDocumento);
                            if (!migracionResponse.Success)
                            {
                                response.Messages.Add(new GenericMessage(migracionResponse.Messages[0].Type, $"Servicio de Migraciones: {migracionResponse.Messages[0].Message}"));
                                response.Success = false;
                                return response;
                            }
                            cliente.Nombre = migracionResponse.Data.strNombreCompleto;
                            cliente.Direccion = "";
                            break;
                        case Definition.TIPO_DOCUMENTO_IDENTIDAD_RUC:
                            var sunatResponse = await _pideAPI.FindSunatByRucAsync(cliente.NumeroDocumento);
                            if (!sunatResponse.Success)
                            {
                                response.Messages.Add(new GenericMessage(sunatResponse.Messages[0].Type, $"Servicio SUNAT: {sunatResponse.Messages[0].Message}"));
                                response.Success = false;
                                return response;
                            }
                            cliente.Nombre = sunatResponse.Data.ddp_nombre.Trim();
                            cliente.Direccion = sunatResponse.Data.desc_domi_fiscal;
                            break;
                        default:
                            // code block
                            break;
                    }

                    if (registroLinea.TipoReciboIngresoId == Definition.TIPO_RECIBO_INGRESO_DEPOSITO_INDEBIDO)
                    {
                        var status = await _repository.VerifyExistsESinad(registroLinea.ExpedienteESinad);
                        if (status)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "Ya se encuentra registrado con el expediente SINAD " + registroLinea.ExpedienteESinad));
                            response.Success = false;
                            return response;
                        }

                        var expedienteResponse = await _expedienteAPI.FindByNumeroExpediente(registroLinea.ExpedienteESinad);
                        if (!expedienteResponse.Success)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "El  número de expediente SINAD " + registroLinea.ExpedienteESinad + " no existe en el registro de ESINAD"));
                            response.Success = false;
                            return response;
                        }

                        registroLinea.NumeroESinad = expedienteResponse.Data.Numero;

                    }

                    var clienteResponse = await _clienteAPI.FindByNroDocAsync(cliente.TipoDocumentoIdentidadId, cliente.NumeroDocumento);

                    if (clienteResponse.Success)
                    {
                        var clienteAdd = clienteResponse.Data;
                        clienteAdd.Correo = registroLinea.Cliente.Correo;
                        clienteAdd.UsuarioModificador = registroLinea.UsuarioCreador;
                        await _clienteAPI.UpdateAsync(clienteAdd, clienteAdd.ClienteId);
                        registroLinea.ClienteId = clienteResponse.Data.ClienteId;
                    }
                    else
                    {
                        cliente.Estado = true;
                        cliente.UsuarioCreador = registroLinea.UsuarioCreador;
                        cliente.Correo = registroLinea.Cliente.Correo;
                        var clienteAddResponse = await _clienteAPI.AddAsync(cliente);
                        if (!clienteAddResponse.Success)
                        {
                            response.Messages = clienteAddResponse.Messages;
                            response.Messages.Add(new GenericMessage(clienteAddResponse.Messages[0].Type, $"Servicio de Cliente: {clienteAddResponse.Messages[0].Message}"));
                            response.Success = false;
                            return response;
                        }

                        registroLinea.ClienteId = clienteAddResponse.Data.ClienteId;
                    }

                    registroLinea.FechaRegistro = DateTime.Now;
                    registroLinea.TipoDocumentoId = Definition.TIPO_DOCUMENTO_REGISTRO_LINEA;
                    var numero = await _repository.FindCorrelativo(registroLinea.UnidadEjecutoraId, registroLinea.TipoDocumentoId);
                    registroLinea.Numero = numero;
                    registroLinea.ValidarDeposito = Definition.VALIDAR_DEPOSITO_PENDIENTE;
                    registroLinea.FechaCreacion = DateTime.Now;

                    var exists = await _repository.VerifyExists(Definition.INSERT, registroLinea);
                    if (exists)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                        return response;
                    }

                    registroLinea = await _repository.Add(registroLinea);

                    if (registroLinea == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                        response.Success = false;
                        return response;
                    }

                    foreach (var detalle in detalles)
                    {
                        detalle.RegistroLineaId = registroLinea.RegistroLineaId;
                        detalle.Estado = "1";
                        detalle.UsuarioCreador = registroLinea.UsuarioCreador;
                        detalle.FechaCreacion = DateTime.Now;
                        await _detalleRepository.Add(detalle);
                    }

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
