using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Application.Command.Dtos;
using RecaudacionApiDepositoBanco.DataAccess;
using RecaudacionApiDepositoBanco.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionApiDepositoBanco.Clients;
using RecaudacionUtils;
using RecaudacionApiDepositoBanco.Helpers;
using System.Text.RegularExpressions;
using System.Linq;

namespace RecaudacionApiDepositoBanco.Application.Command
{
    public class AddDepositoBancoHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusAddResponse>
        {
            public DepositoBancoFormDto FormDto { get; set; }
        }


        // General validación
        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
                RuleFor(x => x.FormDto.NombreArchivo)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Nombre de Archivo es requerido")
                    .MaximumLength(DepositoBancoConsts.NombreArchivoMaxLength)
                    .WithMessage($"La longitud del Nombre de Archivo debe tener {DepositoBancoConsts.NombreArchivoMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!String.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-z0-9._-]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Nombre de Archivo contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.FechaDeposito)
                  .Cascade(CascadeMode.Stop)
                  .NotNull().WithMessage("Fecha Depósito es requerido")
                  .Custom((x, context) =>
                  {
                      if (x != null)
                          if (x.Date > DateTime.Now.Date)
                          {
                              var date = DateTime.Now.ToString("dd/MM/yyyy");
                              context.AddFailure($"Fecha Depósito no puede ser mayor {date}");
                          }
                  });

                RuleFor(x => x.FormDto.UsuarioCreador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Creador es requerido")
                    .MaximumLength(DepositoBancoConsts.UsuarioCreadorMaxLength)
                    .WithMessage($"La longitud del Usuario Creador debe tener {DepositoBancoConsts.UsuarioCreadorMaxLength} caracteres o menos");
            }
        }


        // Detalle validación
        public class DetallesValidator : AbstractValidator<Command>
        {
            private readonly IClienteAPI _clienteAPI;

            public DetallesValidator(IClienteAPI clienteAPI)
            {
                _clienteAPI = clienteAPI;

                // Detalle
                RuleFor(x => x.FormDto.DepositoBancoDetalle)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("Listado de depósitos es requerido")
                   .Custom((x, context) =>
                   {
                       if (x.Count == 0)
                       {
                           context.AddFailure($"Listado de depósitos es requerido");
                       }
                   })
                   .ForEach(detalles =>
                   {
                       detalles.ChildRules(detalle =>
                       {
                           detalle.RuleFor(x => x.Cliente).NotNull().WithMessage("Cliente es requerido");

                           detalle.RuleFor(x => x.ClienteId)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Id Cliente es requerido")
                               .Custom((x, context) =>
                               {
                                   if (x < 1)
                                   {
                                       context.AddFailure($"Detalle: Id Cliente no debe ser {x}");
                                   }
                               })
                               .MustAsync(async (id, cancellation) =>
                               {
                                   var response = await _clienteAPI.FindByIdAsync(id);
                                   bool exists = response.Success;
                                   return exists;
                               }).WithMessage("Detalle: Id Cliente no existe");



                           detalle.RuleFor(x => x.Cliente.TipoDocumentoIdentidadId)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Id Tipo Documento de identidad es requerido")
                               .Custom((x, context) =>
                               {
                                   if (x < 1)
                                   {
                                       context.AddFailure($"Detalle: Id Tipo Documento de identidad no debe ser {x}");
                                   }
                               });

                           detalle.RuleFor(x => x.Cliente.Nombre)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Nombre del cliente es requerido");

                           detalle.RuleFor(x => x.Cliente.NumeroDocumento)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Número Documento del cliente es requerido");


                           detalle.RuleFor(x => x.NumeroDeposito)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Número Depósito es requerido")
                               .MaximumLength(DepositoBancoDetalleConsts.NumeroDepositoMaxLength)
                               .WithMessage($"Detalle: La longitud del Número Depósito debe tener {DepositoBancoDetalleConsts.NumeroDepositoMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!string.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: El Número Depósito contiene caracter no válido");
                                       }
                               });

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

                           detalle.RuleFor(x => x.FechaDeposito)
                              .Cascade(CascadeMode.Stop)
                              .NotNull().WithMessage("Detalle: Fecha Depósito es requerido")
                              .Custom((x, context) =>
                              {
                                  if (x != null)
                                      if (x.Date > DateTime.Now.Date)
                                      {
                                          var date = DateTime.Today.ToString("dd/MM/yyyy");
                                          context.AddFailure($"Detalle: Fecha Depósito no puede ser mayor a {date}");
                                      }
                              });

                       });
                   });
            }
        }


        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IEstadoAPI _estadoAPI;

            public CommandValidator(
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IEstadoAPI estadoAPI
                )
            {
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _estadoAPI = estadoAPI;

                Include(new GeneralValidator());

                RuleFor(x => x.FormDto.UnidadEjecutoraId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Unidad Ejecutora es requerido")
                    .Custom((x, context) =>
                    {
                        var id = x;
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

                RuleFor(x => x.FormDto.TipoDocumentoId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Tipo Documento es requerido")
                    .Custom((x, context) =>
                    {
                        if (x != Definition.TIPO_DOCUMENTO_DEPOSITO_BANCO)
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

                RuleFor(x => x.FormDto.Estado)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Número de Estado es requerido")
                    .Custom((x, context) =>
                    {
                        if (x != Definition.DEPOSITO_BANCO_ESTADO_EMITIDO)
                        {
                            context.AddFailure($"Número de Estado no debe ser {x}");
                        }
                    })
                     .MustAsync(async (estado, cancellation) =>
                     {
                         var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_DEPOSITO_BANCO, estado);
                         bool exists = response.Success;
                         return exists;
                     }).WithMessage("Numero de estado no existe");


            }
        }


        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IDepositoBancoRepository _repository;
            private readonly IDepositoBancoDetalleReposiory _detalleReposiory;
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IMapper _mapper;
            public Handler(
                IDepositoBancoRepository repository,
                IDepositoBancoDetalleReposiory detalleReposiory,
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                IClienteAPI clienteAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
                _detalleReposiory = detalleReposiory;
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _clienteAPI = clienteAPI;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_bancoAPI, _cuentaCorrienteAPI, _unidadEjecutoraAPI, _tipoDocumentoAPI, _estadoAPI);

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

                    DetallesValidator detalleValidations = new DetallesValidator(_clienteAPI);
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

                    var depositoBanco = _mapper.Map<DepositoBancoFormDto, DepositoBanco>(request.FormDto);

                    var totalImporte = request.FormDto.DepositoBancoDetalle.Sum(x => x.Importe);

                    if (depositoBanco.Importe != totalImporte)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "El importe total del detalle no es igual al importe del depósito"));
                        response.Success = false;
                        return response;
                    }

                    var status = await _repository.VerifyExistsNombreArchivo(depositoBanco.NombreArchivo);

                    if (status)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "El registro con el nombre " + depositoBanco.NombreArchivo + ", ya se encuentra registrado"));
                        response.Success = false;
                        return response;
                    }

                    depositoBanco.TipoDocumentoId = Definition.TIPO_DOCUMENTO_DEPOSITO_BANCO;
                    depositoBanco.Numero = await _repository.FindCorrelativo(depositoBanco.UnidadEjecutoraId, depositoBanco.TipoDocumentoId);
                    depositoBanco.FechaRegistro = DateTime.Now;
                    depositoBanco.Estado = Definition.DEPOSITO_BANCO_ESTADO_EMITIDO;
                    depositoBanco.FechaCreacion = DateTime.Now;
                    depositoBanco = await _repository.Add(depositoBanco);

                    if (depositoBanco == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                        response.Success = false;
                        return response;
                    }

                    foreach (var item in request.FormDto.DepositoBancoDetalle)
                    {
                        var detalle = _mapper.Map<DepositoBancoDetalleFormDto, DepositoBancoDetalle>(item);
                        detalle.DepositoBancoId = depositoBanco.DepositoBancoId;
                        detalle.UsuarioCreador = depositoBanco.UsuarioCreador;
                        detalle.Utilizado = Definition.DEPOSITO_BANCO_DETALLE_UTILIZADO_NO;
                        detalle.Estado = "1";
                        detalle.FechaCreacion = DateTime.Now;
                        var detalleAdd = await _detalleReposiory.Add(detalle);
                        if (detalleAdd == null)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Detalle: " + Message.ERROR_INSERT_DB));
                            response.Success = false;
                            return response;
                        }
                    }

                    response.Data = depositoBanco;
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
