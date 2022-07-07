using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Application.Command.Dtos;
using RecaudacionApiComprobanteRetencion.DataAccess;
using RecaudacionApiComprobanteRetencion.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiComprobanteRetencion.Helpers;
using RecaudacionApiComprobanteRetencion.Clients;
using System.Text.RegularExpressions;

namespace RecaudacionApiComprobanteRetencion.Application.Command
{
    public class AddComprobanteRetencionHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }

        public class Command : IRequest<StatusAddResponse>
        {
            public ComprobanteRetencionFormDto FormDto { get; set; }
        }

        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
                RuleFor(x => x.FormDto.FechaEmision)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("Fecha Emisión es requerido")
                   .Custom((x, context) =>
                   {
                       if (x != null)
                           if (x.Date < DateTime.Now.Date)
                           {
                               var date = DateTime.Now.ToString("dd/MM/yyyy");
                               context.AddFailure($"Fecha Emisión no puede ser menor a {date}");
                           }
                   });

                RuleFor(x => x.FormDto.Periodo)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("Periodo es requerido")
                   .Custom((x, context) =>
                   {
                       if (x != null)
                           if (x.Year != DateTime.Now.Year && x.Month != DateTime.Now.Month)
                           {
                               var date = x.ToString("MM-yyyy");
                               context.AddFailure($"Periodo no puede ser {date}");
                           }
                   });

                RuleFor(x => x.FormDto.RegimenRetencion)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Régimen Retención es requerido")
                   .MaximumLength(ComprobanteRetencionConsts.RegimenMaxLength)
                   .WithMessage($"La longitud del Régimen Retención debe tener {ComprobanteRetencionConsts.RegimenMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (x != Definition.TIPO_REGIMEN_RETENCION_01 && x != Definition.TIPO_REGIMEN_RETENCION_02)
                           {
                               context.AddFailure($"Régimen Retención no existe");
                           }
                   });

                RuleFor(x => x.FormDto.Total)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Total Retenido es requerido")
                 .Custom((x, context) =>
                 {
                     if (x <= 0)
                     {
                         context.AddFailure($"Total Retenido no debe ser {x}");
                     }
                 })
                 .Must(x => !(x > ComprobanteRetencionConsts.TotalRetenidoMax))
                 .WithMessage($"Total Retenido no debe ser mayor a {Tools.ToFormat(ComprobanteRetencionConsts.TotalRetenidoMax.ToString())}");


                RuleFor(x => x.FormDto.TotalPago)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Total Pago es requerido")
                 .Custom((x, context) =>
                 {
                     if (x <= 0)
                         context.AddFailure($"Total Pago no debe ser {x}");
                 });

                RuleFor(x => x.FormDto.Porcentaje)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Porcentaje de Retención es requerido")
                 .Custom((x, context) =>
                 {
                     if (x <= 0)
                         context.AddFailure($"Porcentaje de Retención no debe ser {x}");
                 });


                RuleFor(x => x.FormDto.Observacion)
                  .Cascade(CascadeMode.Stop)
                  .MaximumLength(ComprobanteRetencionConsts.ObservacionMaxLength)
                  .WithMessage($"La longitud de la Observación debe tener {ComprobanteRetencionConsts.ObservacionMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!String.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                          {
                              context.AddFailure("La Observación contiene caracter no válido.");
                          }
                  });

                RuleFor(x => x.FormDto.UsuarioCreador)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Usuario Creador es requerido")
                   .MaximumLength(ComprobanteRetencionConsts.UsuarioCreadorMaxLength)
                   .WithMessage($"La longitud del Usuario Creador debe tener {ComprobanteRetencionConsts.UsuarioCreadorMaxLength} caracteres o menos");

            }
        }

        public class DetallesValidator : AbstractValidator<Command>
        {
            public DetallesValidator()
            {

                RuleFor(x => x.FormDto.ComprobanteRetencionDetalle)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage("Detalle de Comprobante de Retención no puede ser nulo")
                    .Custom((x, context) =>
                    {
                        if (x.Count == 0)
                        {
                            context.AddFailure($"Detalle de Comprobante de Retención es requerido");
                        }
                    })
                    .ForEach(detalles =>
                    {
                        detalles.ChildRules(detalle =>
                        {
                            detalle.RuleFor(x => x.ComprobantePagoId)
                              .Cascade(CascadeMode.Stop)
                              .Custom((x, context) =>
                              {
                                  if (x != null)
                                      if (x <= 0)
                                      {
                                          context.AddFailure($"Detalle: Id Comprobante Pago no debe ser {x}");
                                      }
                              });

                            detalle.RuleFor(x => x.TipoDocumento)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Tipo Documento es requerido")
                               .MaximumLength(ComprobanteRetencionDetalleConsts.TipoDocumentoMaxLength)
                               .WithMessage($"Detalle: La longitud del Tipo Documento debe tener {ComprobanteRetencionDetalleConsts.TipoDocumentoMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: Tipo Documento debe ser númerico");
                                       }
                               });

                            detalle.RuleFor(x => x.Serie)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Serie es requerido")
                               .MaximumLength(ComprobanteRetencionDetalleConsts.SerieMaxLength)
                               .WithMessage($"Detalle: La longitud de la Serie debe tener {ComprobanteRetencionDetalleConsts.SerieMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("La Serie contiene caracter no válido");
                                       }
                               });

                            detalle.RuleFor(x => x.Correlativo)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Correlativo es requerido")
                               .MaximumLength(ComprobanteRetencionDetalleConsts.CorrelativoMaxLength)
                               .WithMessage($"Detalle: La longitud del Correlativo debe tener {ComprobanteRetencionDetalleConsts.CorrelativoMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: Correlativo tiene que ser númerico");
                                       }
                               });

                            detalle.RuleFor(x => x.FechaEmision)
                                 .Cascade(CascadeMode.Stop)
                                 .NotNull().WithMessage("Detalle: Fecha Emisión es requerido")
                                 .Custom((x, context) =>
                                 {
                                     if (x != null)
                                         if (x.Date > DateTime.Now.Date)
                                         {
                                             var date = x.ToString("dd/MM/yyyy");
                                             context.AddFailure($"Detalle: Fecha Emisión no puede ser {date}");
                                         }
                                 });

                            detalle.RuleFor(x => x.ImporteTotal)
                              .Cascade(CascadeMode.Stop)
                              .NotEmpty().WithMessage("Detalle: Importe Total es requerido")
                              .Custom((x, context) =>
                              {
                                  if (x <= 0)
                                  {
                                      context.AddFailure($"Detalle: Importe Total no debe ser {x}");
                                  }
                              });

                            detalle.RuleFor(x => x.TipoModena)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Tipo Modena es requerido")
                               .MaximumLength(ComprobanteRetencionDetalleConsts.TipoMonedaMaxLength)
                               .WithMessage($"Detalle: La longitud del Tipo Modena debe tener {ComprobanteRetencionDetalleConsts.TipoMonedaMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[a-zA-Z]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: Tipo Modena contiene caracter no válido");
                                       }
                               });

                            detalle.RuleFor(x => x.ImporteOperacion)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Importe Operación es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x <= 0)
                                    {
                                        context.AddFailure($"Detalle: Importe Operación no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.ModificaNotaCredito)
                                .Cascade(CascadeMode.Stop)
                                .NotNull().WithMessage("Detalle: Modifica Nota Crédito es requerido");

                            detalle.RuleFor(x => x.NumeroCorrelativoPago)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Numero Correlativo Pago es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x <= 0)
                                    {
                                        context.AddFailure($"Detalle: Numero Correlativo Pago no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.FechaPago)
                               .Cascade(CascadeMode.Stop)
                               .NotNull().WithMessage("Detalle: Fecha Pago es requerido")
                               .Custom((x, context) =>
                               {
                                   if (x != null)
                                       if (x.Date < DateTime.Now.Date)
                                       {
                                           var date = x.ToString("dd/MM/yyyy");
                                           context.AddFailure($"Detalle: Fecha Pago no puede ser {date}");
                                       }
                               });

                            detalle.RuleFor(x => x.TipoCambio)
                                .Cascade(CascadeMode.Stop)
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Tipo Cambio no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.ImportePago)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Importe Pago es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x <= 0)
                                    {
                                        context.AddFailure($"Detalle: Importe Pago no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.Tasa)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Tasa es requerido")
                               .Custom((x, context) =>
                               {
                                   if (x <= 0)
                                   {
                                       context.AddFailure($"Detalle: Tasa no debe ser {x}");
                                   }
                               });

                            detalle.RuleFor(x => x.ImporteRetenido)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Importe Retenido es requerido")
                               .Custom((x, context) =>
                               {
                                   if (x <= 0)
                                   {
                                       context.AddFailure($"Detalle: Importe Retenido no debe ser {x}");
                                   }
                               });

                            detalle.RuleFor(x => x.FechaRetencion)
                                 .Cascade(CascadeMode.Stop)
                                 .NotNull().WithMessage("Detalle: Fecha Retención es requerido")
                                 .Custom((x, context) =>
                                 {
                                     if (x != null)
                                         if (x.Date < DateTime.Now.Date)
                                         {
                                             var date = x.ToString("dd/MM/yyyy");
                                             context.AddFailure($"Detalle: Fecha Pago no puede ser {date}");
                                         }
                                 });

                            detalle.RuleFor(x => x.ImporteNetoPagado)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Importe Neto Pagado es requerido")
                               .Custom((x, context) =>
                               {
                                   if (x <= 0)
                                   {
                                       context.AddFailure($"Detalle: Importe Neto Pagado no debe ser {x}");
                                   }
                               });

                            detalle.RuleFor(x => x.Estado)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Estado es requerido")
                               .MaximumLength(ComprobanteRetencionDetalleConsts.EstadoMaxLength)
                               .WithMessage($"Detalle: La longitud del Estado debe tener {ComprobanteRetencionDetalleConsts.EstadoMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: Estado debe que ser númerico");
                                       }
                               });
                        });
                    });
            }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IClienteAPI _clienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;

            public CommandValidator(
                IClienteAPI clienteAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI)
            {
                _clienteAPI = clienteAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;

                Include(new GeneralValidator());

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

                RuleFor(x => x.FormDto.TipoDocumentoId)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Id Tipo Documento es requerido")
                  .Custom((x, context) =>
                  {
                      if (x != Definition.TIPO_COMPROBANTE_RETENCION)
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


                RuleFor(x => x.FormDto.TipoComprobanteId)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Id Tipo Comprobante es requerido")
                 .Custom((x, context) =>
                 {
                     if (x != Definition.TIPO_COMPROBANTE_RETENCION)
                     {
                         context.AddFailure($"Id Tipo Comprobante debe ser ser {x}");
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
                       var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_COMPROBANTE_RETENCION, estado);
                       bool exists = response.Success;
                       return exists;
                   }).WithMessage("Número de estado no existe");

            }
        }

        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IComprobanteRetencionRepository _repository;
            private readonly IComprobanteRetencionDetalleRepository _detalleRepository;
            private readonly IClienteAPI _clienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly ITipoComprobantePagoAPI _tipoComprobantePagoAPI;
            private readonly IComprobanteEmisorAPI _comprobanteEmisorAPI;
            private readonly IOseSunatAPI _oseSunatAPI;
            private readonly IMapper _mapper;

            public Handler(
                IComprobanteRetencionRepository repository,
                IComprobanteRetencionDetalleRepository detalleRepository,
                IClienteAPI clienteAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                ITipoComprobantePagoAPI tipoComprobantePagoAPI,
                IComprobanteEmisorAPI comprobanteEmisorAPI,
                IOseSunatAPI oseSunatAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _clienteAPI = clienteAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _tipoComprobantePagoAPI = tipoComprobantePagoAPI;
                _comprobanteEmisorAPI = comprobanteEmisorAPI;
                _oseSunatAPI = oseSunatAPI;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_clienteAPI, _estadoAPI, _unidadEjecutoraAPI, _tipoDocumentoAPI);
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

                    DetallesValidator detalleValidations = new DetallesValidator();
                    var detalleResult = detalleValidations.Validate(request);
                    if (!detalleResult.IsValid)
                    {
                        foreach (var item in detalleResult.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                            response.Success = false;
                            return response;
                        }
                    }

                    var comprobanteRetencion = _mapper.Map<ComprobanteRetencionFormDto, ComprobanteRetencion>(request.FormDto);

                    comprobanteRetencion.TipoDocumentoId = Definition.TIPO_DOCUMENTO_COMPROBANTE_RETENCION;
                    comprobanteRetencion.TipoComprobanteId = Definition.TIPO_COMPROBANTE_RETENCION;
                    var parametro = await _repository.FindParametro(comprobanteRetencion.UnidadEjecutoraId, comprobanteRetencion.TipoDocumentoId);
                    comprobanteRetencion.Serie = parametro.Serie;
                    comprobanteRetencion.Correlativo = parametro.Correlativo;

                    var codigoTipoComprobante = "";
                    var tipoComprobanteResponse = await _tipoComprobantePagoAPI.FindByIdAsync(comprobanteRetencion.TipoComprobanteId);
                    if (tipoComprobanteResponse.Success)
                    {
                        codigoTipoComprobante = tipoComprobanteResponse.Data.Codigo.Trim();
                    }

                    // Obtener datos del Emisor
                    var comprobanteEmisorResponse = await _comprobanteEmisorAPI.FindByIdEjecutoraAsync(comprobanteRetencion.UnidadEjecutoraId);
                    var emisor = comprobanteEmisorResponse.Data;

                    var nombreArchivo = $"{emisor.NumeroRuc.Trim()}-{codigoTipoComprobante}-{comprobanteRetencion.Serie.Trim()}-{comprobanteRetencion.Correlativo.Trim()}";
                    comprobanteRetencion.NombreArchivo = nombreArchivo;
                    comprobanteRetencion.Estado = Definition.COMPROBANTE_RETENCION_ESTADO_ACEPTADO;
                    comprobanteRetencion.FechaCreacion = DateTime.Now;
                    comprobanteRetencion = await _repository.Add(comprobanteRetencion);

                    if (comprobanteRetencion == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                        response.Success = false;
                        return response;
                    }

                    foreach (var item in request.FormDto.ComprobanteRetencionDetalle)
                    {
                        var comprobanteRetencionDetalle = _mapper.Map<ComprobanteRetencionDetalleFormDto, ComprobanteRetencionDetalle>(item);

                        if (comprobanteRetencion.RegimenRetencion == Definition.TIPO_REGIMEN_RETENCION_01)
                        {
                            comprobanteRetencionDetalle.Tasa = Definition.TIPO_REGIMEN_RETENCION_TASA_PORCENTAJE_3;
                        }
                        else
                        {
                            comprobanteRetencionDetalle.Tasa = Definition.TIPO_REGIMEN_RETENCION_TASA_PORCENTAJE_6;
                        }

                        comprobanteRetencionDetalle.ComprobanteRetencionId = comprobanteRetencion.ComprobanteRetencionId;
                        comprobanteRetencionDetalle.Estado = "1";
                        comprobanteRetencionDetalle.FechaCreacion = DateTime.Now;
                        comprobanteRetencionDetalle.UsuarioCreador = comprobanteRetencion.UsuarioCreador;
                        await _detalleRepository.Add(comprobanteRetencionDetalle);
                    }


                    var documento = new Retencion();

                    var clienteResponse = await _clienteAPI.FindByIdAsync(comprobanteRetencion.ClienteId);
                    var cliente = clienteResponse.Data;

                    // Enviar a OSE SUNAT

                    documento.FirmanteEmisor = emisor.Firmante;
                    documento.NombreCertEmisor = emisor.NombreArchivoCer;
                    documento.NombreKeyEmisior = emisor.NombreArchivoKey;
                    documento.CorreoEnvioEmisor = emisor.CorreoEnvio;
                    documento.CorreoKeyEmisor = emisor.CorreoClave;
                    documento.ServerMailEmisor = emisor.ServerMail;
                    documento.ServerPortEmisor = emisor.ServerPort;
                    documento.RUCEmisor = emisor.NumeroRuc;
                    documento.TipoDocumentoEmisor = emisor.TipoDocumento;
                    documento.NombreComercialEmisor = emisor.NombreComercial;
                    documento.RazonSocialEmisor = emisor.RazonSocial;
                    documento.UbigeoEmisor = "";
                    documento.DireccionEmisor = emisor.Direccion;
                    documento.UrbanizacionEmisor = emisor.Urbanizacion;
                    documento.DepartamentoEmisor = emisor.Departamento;
                    documento.ProvinciaEmisor = emisor.Provincia;
                    documento.DistritoEmisor = emisor.Distrito;
                    documento.CodigoPaisEmisor = emisor.CodigoPais;
                    documento.TelefonoEmisor = emisor.Telefono;
                    documento.DireccionAlternativaEmisor = emisor.DireccionAlternativa;
                    documento.DireccionAlternativaSegEmisor = "";
                    documento.NumeroResolucionEmisor = emisor.NumeroResolucion;
                    documento.UsuarioOseEmisor = emisor.UsuarioOSE;
                    documento.ClaveOseEmisor = emisor.ClaveOSE;
                    documento.Observaciones = "";
                    documento.CodigoTipoComprobante = codigoTipoComprobante;
                    documento.SerieComprobante = comprobanteRetencion.Serie;
                    documento.NumeroComprobante = comprobanteRetencion.Correlativo;
                    documento.FechaEmision = comprobanteRetencion.FechaEmision.ToString("yyyy-MM-dd");
                    documento.RUCAdquirente = cliente.NumeroDocumento;

                    if (cliente.TipoDocumentoIdentidadId == Definition.TIPO_DOCUMENTO_IDENTIDAD_DNI)
                    {
                        documento.TipoDocumentoAdquirente = Definition.TIPO_DOCUMENTO_SUNAT_DNI;
                    }
                    else if (cliente.TipoDocumentoIdentidadId == Definition.TIPO_DOCUMENTO_IDENTIDAD_RUC)
                    {
                        documento.TipoDocumentoAdquirente = Definition.TIPO_DOCUMENTO_SUNAT_RUC;
                    }
                    else if (cliente.TipoDocumentoIdentidadId == Definition.TIPO_DOCUMENTO_IDENTIDAD_CE)
                    {
                        documento.TipoDocumentoAdquirente = Definition.TIPO_DOCUMENTO_SUNAT_CARNE_EXT;
                    }

                    documento.RazonSocialAdquirente = cliente.Nombre;
                    documento.UbigeoAdquirente = "";
                    documento.DireccionAdquirente = cliente.Direccion;
                    documento.LocalidadAdquirente = "";
                    documento.UrbanizacionAdquiriente = "";
                    documento.DepartamentoAdquiriente = "";
                    documento.ProvinciaAdquiriente = "";
                    documento.DistritoAdquiriente = "";
                    documento.CodigoPaisAdquiriente = Definition.SUNAT_CODIGO_PAIS;
                    documento.CodigoTipoMoneda = Definition.SUNAT_CODIGO_TIPO_MONEDA_SOL;
                    documento.CodigoRetencion = comprobanteRetencion.RegimenRetencion;
                    documento.PorcentajeRetencion = comprobanteRetencion.Porcentaje;
                    documento.MontoTotalRetencion = comprobanteRetencion.Total;
                    documento.MontoTotalPago = comprobanteRetencion.TotalPago;

                    documento.DigestValue = "";
                    documento.SignatureValue = "";

                    foreach (var item in request.FormDto.ComprobanteRetencionDetalle)
                    {
                        var detalle = new DetalleRetencion();

                        detalle.CodigoTipoComprobante = item.TipoDocumento;
                        detalle.SerieComprobanteRef = item.Serie;
                        detalle.NumeroComprobanteRef = item.Correlativo;
                        detalle.FechaEmisionComprobante = item.FechaEmision.ToString("yyyy-MM-dd");
                        detalle.MonedaComprobante = Definition.SUNAT_CODIGO_TIPO_MONEDA_SOL;
                        detalle.ImporteTotalComprobante = item.ImporteTotal;
                        detalle.MontoTotalPago = item.ImporteNetoPagado;
                        detalle.FechaPago = item.FechaPago.ToString("yyyy-MM-dd");
                        detalle.MontoRetencion = item.ImporteRetenido;
                        detalle.FechaRetencion = item.FechaRetencion.ToString("yyyy-MM-dd");
                        detalle.MonedaTCambioOrigen = Definition.SUNAT_CODIGO_TIPO_MONEDA_SOL;
                        detalle.MonedaTCambioDestino = Definition.SUNAT_CODIGO_TIPO_MONEDA_SOL;
                        detalle.TipoCambio = 1;
                        detalle.FechaTipoCambio = DateTime.Now.ToString("yyyy-MM-dd");
                        documento.DetalleRetencion.Add(detalle);
                    }

                    //response.Data = documento;
                    var documentoResponse = await _oseSunatAPI.SendAsync(documento);
                    var messageSunat = "";

                    if (!documentoResponse.Success)
                    {
                        await _repository.Delete(comprobanteRetencion);
                        response.Messages = documentoResponse.Messages;
                        response.Success = false;
                        return response;
                    }

                    comprobanteRetencion.Estado = Definition.COMPROBANTE_PAGO_ESTADO_ACEPTADO;
                    comprobanteRetencion.FechaModificacion = DateTime.Now;
                    comprobanteRetencion.UsuarioModificador = comprobanteRetencion.UsuarioCreador;
                    await _repository.Update(comprobanteRetencion);
                    if (!String.IsNullOrEmpty(documentoResponse.Data.MensajeResultadoSunat))
                    {
                        messageSunat = documentoResponse.Data.MensajeResultadoSunat;
                    }

                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, $"{Message.COMPROBANTE_RETENCION_SUCCESS_INSERT}. {messageSunat}"));

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
