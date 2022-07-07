using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Application.Command.Dtos;
using RecaudacionApiComprobantePago.DataAccess;
using RecaudacionApiComprobantePago.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionApiComprobantePago.Clients;
using RecaudacionUtils;
using System.Text.RegularExpressions;
using RecaudacionApiComprobantePago.Helpers;

namespace RecaudacionApiComprobantePago.Application.Command
{
    public class AddComprobantePagoHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusAddResponse>
        {
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
                           if (x.Date > DateTime.Now.Date || x.Date < DateTime.Now.Date.AddDays(-2))
                           {
                               var dateIn = DateTime.Now.Date.AddDays(-2).ToString("dd/MM/yyyy");
                               var dateFin = DateTime.Now.ToString("dd/MM/yyyy");
                               context.AddFailure($"Fecha Emisión debe ser entre {dateIn} y {dateFin}");
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
                       if (x <= 0)
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
                       if (x <= 0)
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

                RuleFor(x => x.FormDto.UsuarioCreador)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Usuario Creador es requerido")
                   .MaximumLength(ComprobantePagoConsts.UsuarioCreadorMaxLength)
                   .WithMessage($"La longitud del Usuario Creador debe tener {ComprobantePagoConsts.UsuarioCreadorMaxLength} caracteres o menos");

            }
        }

        public class DetallesValidator : AbstractValidator<Command>
        {
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly ITarifarioAPI _tarifarioAPI;
            public DetallesValidator(ICatalogoBienAPI catalogoBienAPI,
                ITarifarioAPI tarifarioAPI)
            {
                _catalogoBienAPI = catalogoBienAPI;
                _tarifarioAPI = tarifarioAPI;

                RuleFor(x => x.FormDto.ComprobantePagoDetalle)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage("Detalle de Guia Salida Bien no puede ser nulo")
                    .Custom((x, context) =>
                    {
                        if (x.Count == 0)
                        {
                            context.AddFailure($"Detalle de Guia Salida Bien es requerido");
                        }
                    })
                    .ForEach(detalles =>
                    {
                        detalles.ChildRules(detalle =>
                        {
                            detalle.RuleFor(x => x.TipoAdquisicion)
                              .Cascade(CascadeMode.Stop)
                              .NotEmpty().WithMessage("Detalle: Tipo Adquisición es requerido")
                              .Custom((x, context) =>
                               {
                                   if (x != Definition.TIPO_ADQUISICION_BIEN && x != Definition.TIPO_ADQUISICION_SERVICIO)
                                   {
                                       context.AddFailure($"Tipo Adquisición debe ser Servicio o Bien");
                                   }
                               });

                            detalle.RuleFor(x => x.UnidadMedida)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: La Unidad Medida es requerido")
                               .MaximumLength(ComprobantePagoDetalleConsts.UnidadMedidaMaxLength)
                               .WithMessage($"Detalle: La longitud de la Unidad Medida debe tener {ComprobantePagoDetalleConsts.UnidadMedidaMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9/\n_ -]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: La Unidad Medida contiene caracter no válido");
                                       }
                               });

                            detalle.RuleFor(x => x.Cantidad)
                              .Cascade(CascadeMode.Stop)
                              .NotEmpty().WithMessage("Detalle: La cantidad es requerido")
                              .Custom((x, context) =>
                              {
                                  if (x < 0)
                                  {
                                      context.AddFailure($"Detalle: Cantidad no debe ser {x}");
                                  }
                              });

                            detalle.RuleFor(x => x.Codigo)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Código es requerido")
                               .MaximumLength(ComprobantePagoDetalleConsts.CodigoMaxLength)
                               .WithMessage($"Detalle: La longitud del codigo debe tener {ComprobantePagoDetalleConsts.CodigoMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: El código debe ser númerico");
                                       }
                               });

                            detalle.RuleFor(x => x.Descripcion)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: La Descripción es requerido")
                               .MaximumLength(ComprobantePagoDetalleConsts.DescripcionMaxLength)
                               .WithMessage($"Detalle: La longitud de la Descripción debe tener {ComprobantePagoDetalleConsts.DescripcionMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: La Descripción contiene caracter no válido");
                                       }
                               });

                            detalle.RuleFor(x => x.CodigoTipoMoneda)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty().WithMessage("Detalle: Código tipo moneda es requerido")
                               .MaximumLength(ComprobantePagoDetalleConsts.CodigoTipoMonedaMaxLength)
                               .WithMessage($"Detalle: La longitud del Código tipo moneda debe tener {ComprobantePagoDetalleConsts.CodigoTipoMonedaMaxLength} caracteres o menos")
                               .Custom((x, context) =>
                               {
                                   if (!String.IsNullOrEmpty(x))
                                       if (Regex.Replace(x, @"[A-Za-z]", string.Empty).Length > 0)
                                       {
                                           context.AddFailure("Detalle: El Código tipo moneda contiene caracter no válido");
                                       }
                               });

                            detalle.RuleFor(x => x.PrecioUnitario)
                                 .Cascade(CascadeMode.Stop)
                                 .Custom((x, context) =>
                                 {
                                     if (x < 0)
                                     {
                                         context.AddFailure($"Detalle: Precio Unitario no debe ser {x}");
                                     }
                                 });

                            detalle.RuleFor(x => x.CodigoTipoPrecio)
                                 .Cascade(CascadeMode.Stop)
                                 .NotEmpty().WithMessage("Detalle: Código tipo precio es requerido")
                                 .MaximumLength(ComprobantePagoDetalleConsts.CodigoTipoPrecioMaxLength)
                                 .WithMessage($"Detalle: La longitud del Código tipo precio debe tener {ComprobantePagoDetalleConsts.CodigoTipoPrecioMaxLength} caracteres o menos")
                                 .Custom((x, context) =>
                                 {
                                     if (!String.IsNullOrEmpty(x))
                                         if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                                         {
                                             context.AddFailure("Detalle: El Código tipo precio debe ser númerico");
                                         }
                                 });

                            detalle.RuleFor(x => x.AfectoIGV)
                                 .Cascade(CascadeMode.Stop)
                                 .NotNull().WithMessage("Detalle: Afecto IGV es requerido");

                            detalle.RuleFor(x => x.IGVItem)
                                 .Cascade(CascadeMode.Stop)
                                 .Custom((x, context) =>
                                 {
                                     if (x < 0)
                                     {
                                         context.AddFailure($"Detalle: IGV Item no debe ser {x}");
                                     }
                                 });

                            detalle.RuleFor(x => x.CodigoTipoIGV)
                                 .Cascade(CascadeMode.Stop)
                                 .NotEmpty().WithMessage("Detalle: Código Tipo IGV es requerido")
                                 .Custom((x, context) =>
                                 {
                                     if (x != Definition.TIPO_IGV_INAFECTA)
                                     {
                                         context.AddFailure($"Detalle: Código Tipo IGV no debe ser {x}");
                                     }
                                 });

                            detalle.RuleFor(x => x.DescuentoItem)
                                 .Cascade(CascadeMode.Stop)
                                 .Custom((x, context) =>
                                 {
                                     if (x < 0)
                                     {
                                         context.AddFailure($"Detalle: Descuento item no debe ser {x}");
                                     }
                                 });

                            detalle.RuleFor(x => x.DescuentoTotal)
                                 .Cascade(CascadeMode.Stop)
                                 .Custom((x, context) =>
                                 {
                                     if (x < 0)
                                     {
                                         context.AddFailure($"Detalle: Descuento total no debe ser {x}");
                                     }
                                 });

                            detalle.RuleFor(x => x.FactorDescuento)
                                .Cascade(CascadeMode.Stop)
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Factor descuento no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.FactorDescuento)
                                .Cascade(CascadeMode.Stop)
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Factor descuento no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.SubTotal)
                                .Cascade(CascadeMode.Stop)
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Sub Total no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.ValorVenta)
                                .Cascade(CascadeMode.Stop)
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Valor Venta no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.IngresoPecosaDetalleId)
                                .Cascade(CascadeMode.Stop)
                                .NotNull().When(x => x.TipoAdquisicion == Definition.TIPO_ADQUISICION_BIEN).WithMessage("Detalle: Id Ingreso pecosa detalle es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x != null)
                                        if (x < 0)
                                        {
                                            context.AddFailure($"Detalle: Id Ingreso pecosa detalle no debe ser {x}");
                                        }
                                });

                            detalle.RuleFor(x => x.SerieFormato)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().When(x => x.TipoAdquisicion == Definition.TIPO_ADQUISICION_BIEN).WithMessage("Detalle: Serie Formato es requerido")
                                .Custom((x, context) =>
                                {
                                    if (!String.IsNullOrEmpty(x))
                                        if (Regex.Replace(x, @"[A-Za-z0-9]", string.Empty).Length > 0)
                                        {
                                            context.AddFailure("Detalle: Serie Formato contiene caracter no válido");
                                        }
                                });

                            detalle.RuleFor(x => x.SerieDel)
                                .Cascade(CascadeMode.Stop)
                                .NotNull().When(x => x.TipoAdquisicion == Definition.TIPO_ADQUISICION_BIEN).WithMessage("Detalle: Serie Del es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Serie Del no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.SerieAl)
                                .Cascade(CascadeMode.Stop)
                                .NotNull().When(x => x.TipoAdquisicion == Definition.TIPO_ADQUISICION_BIEN).WithMessage("Detalle: Id Ingreso pecosa detalle es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Id Ingreso pecosa detalle no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.CatalogoBienId)
                               .Cascade(CascadeMode.Stop)
                               .NotNull().When(x => x.TipoAdquisicion == Definition.TIPO_ADQUISICION_BIEN).WithMessage("Id Catalogo Bien es requerido")
                               .Custom((x, context) =>
                               {
                                   if (x != null)
                                       if (x < 1)
                                       {
                                           context.AddFailure($"Detalle: Id Catalogo Bien no debe ser {x}");
                                       }
                               })
                               .MustAsync(async (id, cancellation) =>
                               {
                                   if (id == null)
                                       return true;

                                   var response = await _catalogoBienAPI.FindByIdAsync(id ?? 0);
                                   bool exists = response.Success;
                                   return exists;
                               }).When(x => x.TipoAdquisicion == Definition.TIPO_ADQUISICION_BIEN).WithMessage("Detalle: Id Catalogo Bien no existe");


                            detalle.RuleFor(x => x.TarifarioId)
                             .Cascade(CascadeMode.Stop)
                             .NotNull().When(x => x.TipoAdquisicion == Definition.TIPO_ADQUISICION_SERVICIO).WithMessage("Detalle: Id Tarifario es requerido")
                             .Custom((x, context) =>
                             {
                                 if (x != null)
                                     if (x < 1)
                                     {
                                         context.AddFailure($"Detalle: Id Tarifario no debe ser {x}");
                                     }
                             })
                             .MustAsync(async (id, cancellation) =>
                             {
                                 if (id == null)
                                     return true;

                                 var response = await _tarifarioAPI.FindByIdAsync(id ?? 0);
                                 bool exists = response.Success;
                                 return exists;
                             }).When(x => x.TipoAdquisicion == Definition.TIPO_ADQUISICION_SERVICIO).WithMessage("Detalle: Id Tarifario no existe");
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
            private readonly ITipoComprobantePagoAPI _tipoComprobantePagoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;

            public CommandValidator(
                IClienteAPI clienteAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                ITipoComprobantePagoAPI tipoComprobantePagoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI
                )
            {
                _clienteAPI = clienteAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _tipoComprobantePagoAPI = tipoComprobantePagoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;

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
                     }).WithMessage("Id Cliente no existe.");

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
                     if (x != Definition.TIPO_DOCUMENTO_FACTURA && x != Definition.TIPO_DOCUMENTO_BOLETA &&
                         x != Definition.TIPO_DOCUMENTO_NOTA_CREDITO && x != Definition.TIPO_DOCUMENTO_NOTA_DEBITO)
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
                      if (x != Definition.TIPO_COMPROBANTE_FACTURA && x != Definition.TIPO_COMPROBANTE_BOLETA &&
                         x != Definition.TIPO_COMPROBANTE_NOTA_CREDITO && x != Definition.TIPO_COMPROBANTE_NOTA_DEBITO)
                      {
                          context.AddFailure($"Id Tipo Documento no debe ser {x}");
                      }
                  })
                  .MustAsync(async (id, cancellation) =>
                  {
                      var response = await _tipoComprobantePagoAPI.FindByIdAsync(id);
                      bool exists = response.Success;
                      return exists;
                  }).WithMessage("Id Tipo Comprobante no existe");


                RuleFor(x => x.FormDto.CuentaCorrienteId)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().When(x => x.FormDto.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA).WithMessage("Id Cuenta Corriente es requerido")
                  .Custom((x, context) =>
                  {
                      if (x != null)
                          if (x < 1)
                          {
                              context.AddFailure($"Id Cuenta Corriente no debe ser {x}");
                          }
                  })
                  .MustAsync(async (id, cancellation) =>
                  {
                      bool exists = true;
                      var response = await _cuentaCorrienteAPI.FindByIdAsync(id ?? 0);
                      if (id != null)
                          exists = response.Success;
                      return exists;
                  }).WithMessage("Id Cuenta Corriente no existe");

                RuleFor(x => x.FormDto.Estado)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Estado es requerido")
                   .Custom((x, context) =>
                   {
                       if (x != Definition.COMPROBANTE_PAGO_ESTADO_EMITIDO)
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
        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IComprobantePagoRepository _repository;
            private readonly IComprobantePagoDetalleRepository _detalleRepository;
            private readonly IClienteAPI _clienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoComprobantePagoAPI _tipoComprobantePagoAPI;
            private readonly IOseSunatAPI _oseSunatAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IComprobanteEmisorAPI _comprobanteEmisorAPI;
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly ITarifarioAPI _tarifarioAPI;
            private readonly IDepositoBancoAPI _depositoBancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IPideAPI _pideAPI;
            private readonly IMapper _mapper;

            public Handler(IComprobantePagoRepository repository,
                IComprobantePagoDetalleRepository detalleRepository,
                IClienteAPI clienteAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                ITipoComprobantePagoAPI tipoComprobantePagoAPI,
                IOseSunatAPI oseSunatAPI,
                IComprobanteEmisorAPI comprobanteEmisorAPI,
                ICatalogoBienAPI catalogoBienAPI,
                ITarifarioAPI tarifarioAPI,
                IDepositoBancoAPI depositoBancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IPideAPI pideAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _clienteAPI = clienteAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoComprobantePagoAPI = tipoComprobantePagoAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _oseSunatAPI = oseSunatAPI;
                _comprobanteEmisorAPI = comprobanteEmisorAPI;
                _catalogoBienAPI = catalogoBienAPI;
                _tarifarioAPI = tarifarioAPI;
                _depositoBancoAPI = depositoBancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _pideAPI = pideAPI;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_clienteAPI, _estadoAPI, _unidadEjecutoraAPI, _tipoDocumentoAPI, _tipoComprobantePagoAPI, _cuentaCorrienteAPI);
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

                    DetallesValidator detalleValidations = new DetallesValidator(_catalogoBienAPI, _tarifarioAPI);
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


                    var comprobantePago = _mapper.Map<ComprobantePagoFormDto, ComprobantePago>(request.FormDto);

                    if (comprobantePago.FechaEmision.Date > comprobantePago.FechaVencimiento)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "La Fecha de Emisión no debe se mayor a la Fecha de Vencimiento"));
                        response.Success = false;
                        return response;
                    }

                    var comprobanteFuente = new ComprobantePago();
                    comprobanteFuente.FechaEmision = DateTime.Now;
                    var codigoTipoComprobante = "";
                    var importeTotalMax = 100000;

                    switch (comprobantePago.TipoComprobanteId)
                    {
                        case Definition.TIPO_COMPROBANTE_FACTURA:
                            importeTotalMax = ComprobantePagoConsts.ImporteTotalFacturaMax;
                            break;
                        case Definition.TIPO_COMPROBANTE_BOLETA:
                            importeTotalMax = ComprobantePagoConsts.ImporteTotalBoletaMax;
                            break;
                        case Definition.TIPO_COMPROBANTE_NOTA_CREDITO:
                            importeTotalMax = ComprobantePagoConsts.ImporteTotalNotaCreditoMax;
                            break;
                        case Definition.TIPO_COMPROBANTE_NOTA_DEBITO:
                            importeTotalMax = ComprobantePagoConsts.ImporteTotalNotaDebitoMax;
                            break;
                    }

                    if (comprobantePago.ImporteTotal > importeTotalMax)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El importe total del comprobante no debe ser mayor a {Tools.ToFormat(importeTotalMax.ToString())}"));
                        response.Success = false;
                        return response;
                    }

                    if (comprobantePago.EncargadoTipoDocumento != null)
                    {
                        switch (comprobantePago.EncargadoTipoDocumento)
                        {
                            case Definition.TIPO_DOCUMENTO_IDENTIDAD_DNI:
                                var reniecResponse = await _pideAPI.FindReniecByDniAsync(comprobantePago.EncargadoNumeroDocumento);
                                if (!reniecResponse.Success)
                                {
                                    response.Messages.Add(new GenericMessage(reniecResponse.Messages[0].Type, $"Servicio de Reniec: {reniecResponse.Messages[0].Message}"));
                                    response.Success = false;
                                    return response;
                                }
                                comprobantePago.EncargadoNombre = reniecResponse.Data.nombreCompleto;

                                break;
                            case Definition.TIPO_DOCUMENTO_IDENTIDAD_CE:
                                var migracionResponse = await _pideAPI.FindMigracionByNumeroAsync(comprobantePago.EncargadoNumeroDocumento);
                                if (!migracionResponse.Success)
                                {
                                    response.Messages.Add(new GenericMessage(migracionResponse.Messages[0].Type, $"Servicio de Migraciones: {migracionResponse.Messages[0].Message}"));
                                    response.Success = false;
                                    return response;
                                }
                                comprobantePago.EncargadoNombre = migracionResponse.Data.strNombreCompleto;
                                break;
                            default:
                                // code block
                                break;
                        }
                    }

                    if (comprobantePago.TipoDocumentoId == Definition.TIPO_DOCUMENTO_NOTA_CREDITO && comprobantePago.FuenteOrigen == Definition.FUENTE_ORIGEN_INTERNO)
                    {

                        comprobanteFuente.UnidadEjecutoraId = comprobantePago.UnidadEjecutoraId;
                        comprobanteFuente.TipoComprobanteId = comprobantePago.FuenteTipoDocumento ?? 0;
                        comprobanteFuente.Serie = comprobantePago.FuenteSerie;
                        comprobanteFuente.Correlativo = comprobantePago.FuenteCorrelativo;
                        comprobanteFuente = await _repository.FindByFuente(comprobanteFuente);
                        if (comprobanteFuente == null)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.COMPROBANTE_INFO_NOT_EXISTS_DATA_DOCUMENTO_FUENTE));
                            response.Success = false;
                            return response;
                        }

                        if (comprobanteFuente.ComprobantePagoId != comprobantePago.FuenteId)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.COMPROBANTE_INFO_NOT_EXISTS_DATA_DOCUMENTO_FUENTE));
                            response.Success = false;
                            return response;
                        }

                        if (comprobantePago.ImporteTotal > comprobanteFuente.ImporteTotal)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "El importe total del comprobante es mayor al importe total del documento fuente."));
                            response.Success = false;
                            return response;
                        }

                        var totalFuente = await _repository.CountByFuente(comprobanteFuente.ComprobantePagoId);

                        if (totalFuente > 0)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.COMPROBANTE_INFO_EXISTS_DATA_DOCUMENTO_FUENTE));
                            response.Success = false;
                            return response;
                        }

                    }

                    // Obtener datos del Emisor
                    var comprobanteEmisorResponse = await _comprobanteEmisorAPI.FindByIdEjecutoraAsync(comprobantePago.UnidadEjecutoraId);
                    var emisor = comprobanteEmisorResponse.Data;

                    var tipoComprobanteResponse = await _tipoComprobantePagoAPI.FindByIdAsync(comprobantePago.TipoComprobanteId);
                    if (tipoComprobanteResponse.Success)
                    {
                        codigoTipoComprobante = tipoComprobanteResponse.Data.Codigo.Trim();
                    }

                    // Obtener Correlativo
                    var comprobantePagoParametro = await _repository.FindParametro(comprobantePago.UnidadEjecutoraId, comprobantePago.TipoComprobanteId);
                    comprobantePago.Correlativo = comprobantePagoParametro.Correlativo;
                    comprobantePago.Serie = comprobantePagoParametro.Serie;
                    var nombreArchivo = $"{emisor.NumeroRuc.Trim()}-{codigoTipoComprobante}-{comprobantePago.Serie.Trim()}-{comprobantePago.Correlativo.Trim()}";
                    comprobantePago.ComprobanteEmisorId = emisor.ComprobanteEmisorId;
                    comprobantePago.NombreArchivo = nombreArchivo;
                    comprobantePago.ImporteTotalLetra = "-";
                    comprobantePago.FechaCreacion = DateTime.Now;
                    comprobantePago.Estado = Definition.COMPROBANTE_PAGO_ESTADO_EMITIDO;

                    var exists = await _repository.VerifyExists(Definition.INSERT, comprobantePago);

                    if (exists)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                    }

                    //Add Comprobante
                    comprobantePago = await _repository.Add(comprobantePago);

                    if (comprobantePago == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                        response.Success = false;
                        return response;
                    }

                    // Add Detalle
                    foreach (var item in request.FormDto.ComprobantePagoDetalle)
                    {
                        var comprobantePagoDetalle = _mapper.Map<ComprobantePagoDetalleFormDto, ComprobantePagoDetalle>(item);
                        comprobantePagoDetalle.FechaCreacion = DateTime.Now;
                        comprobantePagoDetalle.ComprobantePagoId = comprobantePago.ComprobantePagoId;
                        comprobantePagoDetalle.UsuarioCreador = comprobantePago.UsuarioCreador;
                        await _detalleRepository.Add(comprobantePagoDetalle);
                    }

                    if (comprobantePago.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA && comprobantePago.ValidarDeposito == Definition.VALIDAR_DEPOSITO_SI)
                    {
                        var responseDepositoBancoDetalle = await _depositoBancoAPI.FindDetalleByIdAsync(comprobantePago.DepositoBancoDetalleId ?? 0);
                        var depositoBancoDetalle = responseDepositoBancoDetalle.Data;
                        depositoBancoDetalle.TipoDocumento = comprobantePago.TipoDocumentoId;
                        depositoBancoDetalle.NumeroDocumento = $"{comprobantePago.Serie}-{comprobantePago.Correlativo}";
                        depositoBancoDetalle.FechaDocumento = comprobantePago.FechaEmision;
                        depositoBancoDetalle.Utilizado = Definition.DEPOSITO_BANCO_DETALLE_UTILIZADO_SI;
                        depositoBancoDetalle.UsuarioModificador = comprobantePago.UsuarioCreador;
                        var depositoBancResponse = await _depositoBancoAPI.UpdateDetalleAsync(depositoBancoDetalle.DepositoBancoDetalleId, depositoBancoDetalle);

                        if (!depositoBancResponse.Success)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, $"servicio de depósito de banco: {depositoBancResponse.Messages[0].Message}"));
                        }
                    }

                    Documento documento = new Documento();

                    var clienteResponse = await _clienteAPI.FindByIdAsync(comprobantePago.ClienteId);
                    var cliente = clienteResponse.Data;
                    var serieNumeroDocAfectado = "";
                    var codigoTipoDocAfectado = "";
                    var fechaEmisionDocAfectado = "";

                    if (comprobantePago.TipoDocumentoId == Definition.TIPO_DOCUMENTO_NOTA_CREDITO || comprobantePago.TipoDocumentoId == Definition.TIPO_DOCUMENTO_NOTA_DEBITO)
                    {
                        serieNumeroDocAfectado = comprobantePago.FuenteSerie + "-" + comprobantePago.FuenteCorrelativo;
                        fechaEmisionDocAfectado = comprobanteFuente.FechaEmision.ToString("yyyy-MM-dd");
                        if (comprobantePago.FuenteTipoDocumento == Definition.TIPO_FUENTE_FACTURA)
                        {
                            codigoTipoDocAfectado = Definition.SUNAT_TIPO_COMPROBANTE_FACTURA;
                        }
                        else
                        {
                            codigoTipoDocAfectado = Definition.SUNAT_TIPO_COMPROBANTE_BOLETA;
                        }

                    }

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
                    documento.NumeroComprobante = $"{comprobantePago.Serie.Trim()}-{comprobantePago.Correlativo.Trim()}";
                    documento.FechaEmision = comprobantePago.FechaEmision.ToString("yyyy-MM-dd");
                    documento.HoraEmision = "12:00:00";
                    documento.CodigoTipoComprobante = codigoTipoComprobante;
                    documento.CodigoTipoMoneda = comprobantePago.CodigoTipoMoneda.Trim();
                    documento.SerieNumeroCorrelativo = $"{comprobantePago.Serie.Trim()}-{comprobantePago.Correlativo.Trim()}";
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


                    documento.RazonSocialAdquirente = cliente.Nombre.Trim();
                    documento.CondicionPagoEmisor = Definition.CONDICION_PAGO_EFECTIVO;
                    documento.DireccionAdquirente = cliente.Direccion;
                    documento.ImporteBrutoComprobante = comprobantePago.ImporteBruto;
                    documento.ValorIGV = Definition.IGV_PORCETAJE;
                    documento.DigestValue = null;
                    documento.SignatureValue = null;

                    documento.IGVTotalComprobante = comprobantePago.IGVTotal;
                    documento.OTRCTotalComprobante = comprobantePago.OTRCTotal;
                    documento.ImporteTotalComprobante = comprobantePago.ImporteTotal;
                    documento.MontoEnLetras = comprobantePago.ImporteTotalLetra;
                    documento.TotalVentaOpGravadas = comprobantePago.TotalOpGravada;
                    documento.TotalVentaOpInafectas = comprobantePago.TotalOpInafecta;
                    documento.TotalVentaOpExoneradas = comprobantePago.TotalOpExonerada;
                    documento.TotalVentaOpGratuitas = comprobantePago.TotalOpGratuita;
                    documento.TotalDescuentos = comprobantePago.TotalDescuento;
                    documento.OrdenCompra = comprobantePago.OrdenCompra;
                    documento.GuiaRemision = comprobantePago.GuiaRemision;
                    documento.sDocAdicional = "";
                    documento.sOrdenCompra = "";
                    documento.nOrdenCompra = "";
                    int numeroItem = 0;
                    foreach (var comprobantePagoDetalleDto in request.FormDto.ComprobantePagoDetalle)
                    {
                        numeroItem++;
                        var documentoDetalle = new DocumentoDetalle();
                        documentoDetalle.NumeroItem = numeroItem.ToString("D3");
                        documentoDetalle.UnidadMedida = comprobantePagoDetalleDto.UnidadMedida;
                        documentoDetalle.Cantidad = comprobantePagoDetalleDto.Cantidad;
                        documentoDetalle.CodigoProducto = comprobantePagoDetalleDto.Codigo;
                        documentoDetalle.DescripcionProducto = comprobantePagoDetalleDto.Descripcion;
                        documentoDetalle.ValorUnitarioxItem = comprobantePagoDetalleDto.PrecioUnitario;
                        documentoDetalle.CodigoTipoMoneda = comprobantePagoDetalleDto.CodigoTipoMoneda;
                        documentoDetalle.PrecioUnitario = comprobantePagoDetalleDto.PrecioUnitario;
                        documentoDetalle.CodigoTipoPrecio = Definition.TIPO_PRECIO_UNITARIO;
                        documentoDetalle.IGVxItem = comprobantePagoDetalleDto.IGVItem;
                        documentoDetalle.CodigoTipoIGV = comprobantePagoDetalleDto.CodigoTipoIGV;
                        documentoDetalle.ValorVentaxItem = comprobantePagoDetalleDto.ValorVenta;
                        documentoDetalle.DescuentoxItem = comprobantePagoDetalleDto.DescuentoItem;
                        documentoDetalle.DescuentoTotal = comprobantePagoDetalleDto.DescuentoTotal;

                        if (comprobantePagoDetalleDto.AfectoIGV)
                        {
                            documentoDetalle.PrecioSinIGV = Tools.basePrecio(documentoDetalle.PrecioUnitario, Definition.IGV_VALOR);
                            documentoDetalle.TotalItemSinIGV = comprobantePagoDetalleDto.SubTotal;
                        }
                        else
                        {
                            documentoDetalle.PrecioSinIGV = comprobantePagoDetalleDto.PrecioUnitario;
                            documentoDetalle.TotalItemSinIGV = comprobantePagoDetalleDto.PrecioUnitario * comprobantePagoDetalleDto.Cantidad;
                        }

                        documentoDetalle.sFactorDescuento = comprobantePagoDetalleDto.FactorDescuento;
                        documento.DetalleDocumento.Add(documentoDetalle);
                    }

                    documento.sFactorDescuento = 0;
                    documento.CodigoTipoNotaCredito = comprobantePago.CodigoTipoNota;
                    documento.CodigoMotivoNotaCredito = "";
                    documento.CodigoTipoNotaDebito = comprobantePago.CodigoTipoNota;
                    documento.CodigoMotivoNotaDebito = "";
                    documento.MotivoSustentoDocumento = comprobantePago.Observacion;
                    if (String.IsNullOrEmpty(comprobantePago.Observacion))
                    {
                        documento.MotivoSustentoDocumento = "NINGUNO";
                    }

                    documento.SerieNumeroDocAfectado = serieNumeroDocAfectado;
                    documento.CodigoTipoDocAfectado = codigoTipoDocAfectado;
                    documento.FechaEmisionDocAfectado = fechaEmisionDocAfectado;
                    documento.sEstado = "";


                    DocumentoAdicional adicional = new DocumentoAdicional();
                    adicional.NumeroDocumento = "";
                    adicional.TipoDocumento = "";
                    documento.DetalleAdicionales = null;
                    //response.Data = documento;
                    var documentoResponse = await _oseSunatAPI.SendAsync(documento);
                    var messageSunat = "";

                    if (!documentoResponse.Success)
                    {
                        response.Messages = documentoResponse.Messages;
                        response.Success = false;
                        await _repository.Delete(comprobantePago);
                        return response;
                    }

                    comprobantePago.Estado = Definition.COMPROBANTE_PAGO_ESTADO_ACEPTADO;
                    comprobantePago.FechaModificacion = DateTime.Now;
                    comprobantePago.UsuarioModificador = comprobantePago.UsuarioCreador;
                    await _repository.Update(comprobantePago);
                    if (!String.IsNullOrEmpty(documentoResponse.Data.MensajeResultadoSunat))
                    {
                        messageSunat = documentoResponse.Data.MensajeResultadoSunat;
                    }

                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, $"{Message.COMPROBANTE_SUCCESS_INSERT}. {messageSunat}"));

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
