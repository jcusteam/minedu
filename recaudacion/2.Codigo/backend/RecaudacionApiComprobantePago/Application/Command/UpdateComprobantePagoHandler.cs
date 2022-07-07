using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Application.Command.Dtos;
using RecaudacionApiComprobantePago.DataAccess;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiComprobantePago.Clients;
using RecaudacionApiComprobantePago.Helpers;
using System.Text.RegularExpressions;
using RecaudacionApiComprobantePago.Domain;

namespace RecaudacionApiComprobantePago.Application.Command
{
    public class UpdateComprobantePagoHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public ComprobantePagoFormDto FormDto { get; set; }
        }

        // Validación general
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

                RuleFor(x => x.FormDto.FechaVencimiento)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("Fecha Vencimiento es requerido")
                   .Custom((x, context) =>
                   {
                       if (x != null)
                           if (x.Date < DateTime.Now.Date)
                           {
                               var date = DateTime.Now.ToString("dd/MM/yyyy");
                               context.AddFailure($"Fecha Vencimiento no puede ser menor a {date}");
                           }
                   });

                RuleFor(x => x.FormDto.TipoAdquisicion)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Tipo Adquisición es requerido")
                    .Custom((x, context) =>
                    {
                        if (x != Definition.TIPO_ADQUISICION_BIEN && x != Definition.TIPO_ADQUISICION_SERVICIO)
                        {
                            context.AddFailure($"Tipo Adquisición debe ser Servicio o Bien");
                        }
                    });

                RuleFor(x => x.FormDto.TipoCondicionPago)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Condición de pago es requerido")
                  .Custom((x, context) =>
                  {
                      if (x != Definition.TIPO_CONDICION_PAGO_EFECTIVO && x != Definition.TIPO_CONDICION_PAGO_CREDITO)
                      {
                          context.AddFailure($"Condición de pago debe ser efectivo o crédito");
                      }
                  });

                RuleFor(x => x.FormDto.CodigoTipoOperacion)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Tipo de Operación es requerido")
                  .Custom((x, context) =>
                  {
                      if (!String.IsNullOrEmpty(x))
                          if (x != Definition.TIPO_OPERACION_VENTA_INTERNA)
                          {
                              context.AddFailure($"Tipo de Operación debe ser Venta Interna");
                          }
                  });


                RuleFor(x => x.FormDto.NumeroDeposito)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().When(x => x.FormDto.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA)
                   .MaximumLength(ComprobantePagoConsts.NumeroDepositoMaxLength)
                   .WithMessage($"La longitud del Numero Depósito debe tener {ComprobantePagoConsts.NumeroDepositoMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Numero Depósito contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.FechaDeposito)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().When(x => x.FormDto.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA)
                   .WithMessage("Fecha Depósito es requerido")
                   .Custom((x, context) =>
                   {
                       if (x != null)
                           if (x.Value.Date > DateTime.Now.Date)
                           {
                               var date = DateTime.Today.ToString("dd/MM/yyyy");
                               context.AddFailure($"Fecha Depósito no puede ser mayor a {date}");
                           }
                   });

                RuleFor(x => x.FormDto.ValidarDeposito)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().When(x => x.FormDto.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA)
                  .WithMessage("Validación del depósito de cuenta corriente es requerido")
                  .MaximumLength(ComprobantePagoConsts.ValidarDepositoMaxLength)
                  .WithMessage($"La longitud de validación del depósito debe tener {ComprobantePagoConsts.ValidarDepositoMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!String.IsNullOrEmpty(x))
                          if (x != Definition.VALIDAR_DEPOSITO_SI)
                          {
                              context.AddFailure("Se debe Validar el depósito de cuenta corriente");
                          }
                  });

                RuleFor(x => x.FormDto.NumeroCheque)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ComprobantePagoConsts.NumeroChequeMaxLength)
                   .WithMessage($"La longitud del Número Cheque debe tener {ComprobantePagoConsts.NumeroChequeMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Número Cheque contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.EncargadoTipoDocumento)
                  .Cascade(CascadeMode.Stop)
                  .Custom((x, context) =>
                  {
                      if (x != null)
                          if (x != Definition.TIPO_DOCUMENTO_IDENTIDAD_DNI && x != Definition.TIPO_DOCUMENTO_IDENTIDAD_CE)
                          {
                              context.AddFailure($"Tipo de documento de identidad del Encargado debe ser DNI ó Carne de Extranjería");
                          }
                  });

                RuleFor(x => x.FormDto.EncargadoNombre)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ComprobantePagoConsts.EncargadoNombreMaxLength)
                   .WithMessage($"La longitud del Nombre del Encargado debe tener {ComprobantePagoConsts.EncargadoNombreMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9' ]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Nombre del Encargado contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.EncargadoNumeroDocumento)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ComprobantePagoConsts.EncargadoNumeroDocumentoMaxLength)
                   .WithMessage($"La longitud del Número de documento del Encargado debe tener {ComprobantePagoConsts.EncargadoNumeroDocumentoMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Número de documento del Encargado contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.FuenteId)
                  .Cascade(CascadeMode.Stop)
                  .NotNull().When(x => x.FormDto.TipoComprobanteId == Definition.TIPO_COMPROBANTE_NOTA_CREDITO && x.FormDto.FuenteOrigen == Definition.FUENTE_ORIGEN_INTERNO)
                  .WithMessage("Id documento Fuente es requerido")
                  .Custom((x, context) =>
                  {
                      if (x <= 0)
                      {
                          context.AddFailure($"Id de documento Fuente no debe ser {x}");
                      }
                  });

                RuleFor(x => x.FormDto.FuenteTipoDocumento)
                  .Cascade(CascadeMode.Stop)
                  .NotNull().When(x => x.FormDto.TipoComprobanteId == Definition.TIPO_COMPROBANTE_NOTA_CREDITO && x.FormDto.FuenteOrigen == Definition.FUENTE_ORIGEN_INTERNO)
                  .WithMessage("Tipo de documento fuente es requerido")
                  .Custom((x, context) =>
                  {
                      if (x != null)
                          if (x != Definition.TIPO_COMPROBANTE_FACTURA && x != Definition.TIPO_COMPROBANTE_BOLETA)
                          {
                              context.AddFailure($"Tipo de documento fuente no debe ser {x}");
                          }
                  });

                RuleFor(x => x.FormDto.FuenteSerie)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().When(x => x.FormDto.TipoComprobanteId == Definition.TIPO_COMPROBANTE_NOTA_CREDITO && x.FormDto.FuenteOrigen == Definition.FUENTE_ORIGEN_INTERNO)
                  .WithMessage("Serie de documento fuente es requerido")
                  .Custom((x, context) =>
                  {
                      if (!String.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                          {
                              context.AddFailure("Serie de documento fuente contiene caracter no válido");
                          }
                  });

                RuleFor(x => x.FormDto.FuenteCorrelativo)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().When(x => x.FormDto.TipoComprobanteId == Definition.TIPO_COMPROBANTE_NOTA_CREDITO && x.FormDto.FuenteOrigen == Definition.FUENTE_ORIGEN_INTERNO)
                 .WithMessage("Número de documento fuente es requerido")
                 .Custom((x, context) =>
                 {
                     if (!String.IsNullOrEmpty(x))
                         if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                         {
                             context.AddFailure("Número de documento fuente contiene caracter no válido");
                         }
                 });

                RuleFor(x => x.FormDto.FuenteOrigen)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().When(x => x.FormDto.TipoComprobanteId == Definition.TIPO_COMPROBANTE_NOTA_CREDITO)
                  .WithMessage("Origen de documento fuente es requerido")
                  .Custom((x, context) =>
                  {
                      if (x != null)
                          if (x != Definition.FUENTE_ORIGEN_INTERNO && x != Definition.FUENTE_ORIGEN_EXTERNO)
                          {
                              context.AddFailure($"Origen de documento fuente debe ser interno o externo");
                          }
                  });

                RuleFor(x => x.FormDto.FuenteValidar)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().When(x => x.FormDto.TipoComprobanteId == Definition.TIPO_COMPROBANTE_NOTA_CREDITO && x.FormDto.FuenteOrigen == Definition.FUENTE_ORIGEN_INTERNO)
                  .WithMessage("La validación de documento fuente a modificar es requerido")
                  .Custom((x, context) =>
                  {
                      if (x != null)
                          if (x != Definition.VALIDAR_FUENTE_SI)
                          {
                              context.AddFailure($"Se debe validar el documento fuente a modificar");
                          }
                  });


                RuleFor(x => x.FormDto.Sustento)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ComprobantePagoConsts.SustentoMaxLength)
                   .WithMessage($"La longitud del Sustento debe tener {ComprobantePagoConsts.SustentoMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Sustento contiene caracter no válido");
                           }
                   });

                RuleFor(x => x.FormDto.Observacion)
                  .Cascade(CascadeMode.Stop)
                  .MaximumLength(ComprobantePagoConsts.ObservacionMaxLength)
                  .WithMessage($"La longitud de la Observación debe tener {ComprobantePagoConsts.ObservacionMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!String.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                          {
                              context.AddFailure("La Observación contiene caracter no válido");
                          }
                  });

                RuleFor(x => x.FormDto.CodigoTipoMoneda)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Código tipo moneda es requerido")
                   .MaximumLength(ComprobantePagoConsts.CodigoTipoMonedaMaxLength)
                   .WithMessage($"La longitud del ódigo tipo moneda debe tener {ComprobantePagoConsts.CodigoTipoMonedaMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Código tipo moneda contiene caracter no válido.");
                           }
                   });

                RuleFor(x => x.FormDto.ImporteBruto)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"Importe Bruto no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.ValorIGV)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"Valor IGV no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.IGVTotal)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"IGV Total no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.ISCTotal)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"ISC Total no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.OTRTotal)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"OTR Total no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.OTRCTotal)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"OTRC Total no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.ImporteTotal)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"Importe Total no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.TotalOpGravada)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"Total Operaciones Gravadas no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.TotalOpInafecta)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"Total Operaciones Inafectas no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.TotalOpExonerada)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"Total Operaciones Exoneradas no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.TotalOpGratuita)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"Total Operaciones Gratuitas no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.TotalDescuento)
                   .Cascade(CascadeMode.Stop)
                   .Custom((x, context) =>
                   {
                       if (x < 0)
                       {
                           context.AddFailure($"Total Descuento no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.OrdenCompra)
                  .Cascade(CascadeMode.Stop)
                  .MaximumLength(ComprobantePagoConsts.OrdenCompraMaxLength)
                  .WithMessage($"La longitud de la Orden de Compra debe tener {ComprobantePagoConsts.OrdenCompraMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!String.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                          {
                              context.AddFailure("La Orden de Compra contiene caracter no válido.");
                          }
                  });

                RuleFor(x => x.FormDto.GuiaRemision)
                  .Cascade(CascadeMode.Stop)
                  .MaximumLength(ComprobantePagoConsts.OrdenCompraMaxLength)
                  .WithMessage($"La longitud de la Guia Remisión debe tener {ComprobantePagoConsts.OrdenCompraMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!String.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                          {
                              context.AddFailure("La Guia Remisión contiene caracter no válido.");
                          }
                  });

                RuleFor(x => x.FormDto.CodigoTipoNota)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ComprobantePagoConsts.CodigoTipoNotaMaxLength)
                   .WithMessage($"La longitud del Código Tipo Nota debe tener {ComprobantePagoConsts.CodigoTipoNotaMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Código Tipo Nota contiene caracter no válido.");
                           }
                   });

                RuleFor(x => x.FormDto.CodigoMotivoNota)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(ComprobantePagoConsts.EncargadoNumeroDocumentoMaxLength)
                   .WithMessage($"La longitud del Código motivo Nota debe tener {ComprobantePagoConsts.EncargadoNumeroDocumentoMaxLength} caracteres o menos")
                   .Custom((x, context) =>
                   {
                       if (!String.IsNullOrEmpty(x))
                           if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                           {
                               context.AddFailure("El Código Motivo Nota contiene caracter no válido.");
                           }
                   });

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(ComprobantePagoConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {ComprobantePagoConsts.UsuarioModificadorMaxLength} caracteres o menos");

            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;

            public CommandValidator(IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;

                Include(new GeneralValidator());

                RuleFor(x => x.Id).NotEmpty().WithMessage("Id Comprobante Pago es requerido")
                 .Custom((x, context) =>
                 {
                     if (x < 1)
                     {
                         context.AddFailure($"Id Comprobante Pago no debe ser {x}");
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
                      var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_FACTURA, estado);
                      bool exists = response.Success;
                      return exists;
                  }).WithMessage("Número de estado no existe");

            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
        {
            private readonly IComprobantePagoRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(IComprobantePagoRepository repository,
                IEstadoAPI estadoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _estadoAPI = estadoAPI;
                _mapper = mapper;
            }

            public async Task<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {
                        var comprobantePagoForm = _mapper.Map<ComprobantePagoFormDto, ComprobantePago>(request.FormDto);

                        var comprobantePago = await _repository.FindById(request.Id);

                        if (comprobantePago == null || (request.Id != comprobantePagoForm.ComprobantePagoId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }

                        comprobantePago.EstadoSunat = comprobantePagoForm.EstadoSunat;
                        comprobantePago.UsuarioModificador = comprobantePagoForm.UsuarioModificador;
                        comprobantePago.FechaModificacion = DateTime.Now;

                        await _repository.Update(comprobantePago);

                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE));
                        response.Success = true;
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.ERROR_SERVER));
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
