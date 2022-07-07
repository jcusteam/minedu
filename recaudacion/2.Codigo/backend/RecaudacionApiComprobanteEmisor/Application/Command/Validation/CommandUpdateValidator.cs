using FluentValidation;
using RecaudacionApiComprobanteEmisor.Application.Command.Request;
using RecaudacionApiComprobanteEmisor.Clients;
using RecaudacionApiComprobanteEmisor.Helpers;
using RecaudacionUtils;
using System;
using System.Text.RegularExpressions;

namespace RecaudacionApiComprobanteEmisor.Application.Command.Validation
{

    public class GeneralUpdateValidator : AbstractValidator<CommandUpdate>
    {
        public GeneralUpdateValidator()
        {

            RuleFor(x => x.FormDto.Firmante)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Nombre de Firmante es requerido")
               .MaximumLength(ComprobanteEmisorConsts.EmisorFirmamteMaxLength)
               .WithMessage($"La longitud del Nombre de Firmante debe tener {ComprobanteEmisorConsts.EmisorFirmamteMaxLength} caracteres o menos")
               .Custom((x, context) =>
               {
                   if (!String.IsNullOrEmpty(x))
                       if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                       {
                           context.AddFailure("Nombre de Firmante contiene caracter no válido");
                       }
               });

            RuleFor(x => x.FormDto.NumeroRuc)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Número RUC es requerido")
                .MaximumLength(ComprobanteEmisorConsts.RucMaxLength)
                .WithMessage($"La longitud del Número RUC debe tener {ComprobanteEmisorConsts.RucMaxLength} caracteres o menos")
                .Custom((x, context) =>
                {
                    if (!Validators.IsNroRUC(x))
                    {
                        context.AddFailure("El Número RUC no se válido");
                    }
                });

            RuleFor(x => x.FormDto.TipoDocumento)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Tipo Documento es requerido")
                .MaximumLength(ComprobanteEmisorConsts.TipoDocumentoMaxLength)
                .WithMessage($"La longitud de Tipo Documento debe tener {ComprobanteEmisorConsts.TipoDocumentoMaxLength} caracteres o menos")
                 .Custom((x, context) =>
                 {
                     if (!String.IsNullOrEmpty(x))
                         if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                         {
                             context.AddFailure("Tipo Documento debe ser numérico");
                         }
                 });

            RuleFor(x => x.FormDto.NombreComercial)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Nombre Comercial es requerido")
               .MaximumLength(ComprobanteEmisorConsts.NombreComercialMaxLength)
               .WithMessage($"La longitud del Nombre Comercial debe tener {ComprobanteEmisorConsts.NombreComercialMaxLength} caracteres o menos")
               .Custom((x, context) =>
               {
                   if (!String.IsNullOrEmpty(x))
                       if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                       {
                           context.AddFailure("Nombre Comercial contiene caracter no válido");
                       }
               });

            RuleFor(x => x.FormDto.RazonSocial)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Razón Social es requerido")
               .MaximumLength(ComprobanteEmisorConsts.RazonSocialMaxLength)
               .WithMessage($"La longitud de la Razón Social debe tener {ComprobanteEmisorConsts.RazonSocialMaxLength} caracteres o menos")
               .Custom((x, context) =>
               {
                   if (!String.IsNullOrEmpty(x))
                       if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                       {
                           context.AddFailure("Razón Social contiene caracter no válido");
                       }
               });

            RuleFor(x => x.FormDto.Ubigeo)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Ubigeo es requerido")
              .MaximumLength(ComprobanteEmisorConsts.UbigeoMaxLength)
              .WithMessage($"La longitud del Ubigeo debe tener {ComprobanteEmisorConsts.UbigeoMaxLength} caracteres o menos")
               .Custom((x, context) =>
               {
                   if (!String.IsNullOrEmpty(x))
                       if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                       {
                           context.AddFailure("El Ubigeo debe ser numérico");
                       }
               });

            RuleFor(x => x.FormDto.Direccion)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Dirección es requerido")
               .MaximumLength(ComprobanteEmisorConsts.DireccionMaxLength)
               .WithMessage($"La longitud de la Dirección debe tener {ComprobanteEmisorConsts.DireccionMaxLength} caracteres o menos");

            RuleFor(x => x.FormDto.Urbanizacion)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Urbanización es requerido")
               .MaximumLength(ComprobanteEmisorConsts.UrbanizacionMaxLength)
               .WithMessage($"La longitud de la Urbanización debe tener {ComprobanteEmisorConsts.UrbanizacionMaxLength} caracteres o menos");

            RuleFor(x => x.FormDto.Departamento)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Departamento es requerido")
               .MaximumLength(ComprobanteEmisorConsts.DepartamentoMaxLength)
               .WithMessage($"La longitud del Departamento debe tener {ComprobanteEmisorConsts.DepartamentoMaxLength} caracteres o menos")
                .Custom((x, context) =>
                {
                    if (!String.IsNullOrEmpty(x))
                        if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                        {
                            context.AddFailure("Departamento contiene caracter no válido.");
                        }
                });
            RuleFor(x => x.FormDto.Provincia)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Provincia es requerido")
               .MaximumLength(ComprobanteEmisorConsts.ProvinciaMaxLength)
               .WithMessage($"La longitud de la Provincia debe tener {ComprobanteEmisorConsts.ProvinciaMaxLength} caracteres o menos")
                .Custom((x, context) =>
                {
                    if (!String.IsNullOrEmpty(x))
                        if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                        {
                            context.AddFailure("Provincia contiene caracter no válido.");
                        }
                });

            RuleFor(x => x.FormDto.Distrito)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Distrito es requerido")
               .MaximumLength(ComprobanteEmisorConsts.DistritoMaxLength)
               .WithMessage($"La longitud del Distrito debe tener {ComprobanteEmisorConsts.DistritoMaxLength} caracteres o menos")
                .Custom((x, context) =>
                {
                    if (!String.IsNullOrEmpty(x))
                        if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                        {
                            context.AddFailure("Distrito contiene caracter no válido.");
                        }
                });

            RuleFor(x => x.FormDto.CodigoPais)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Código de país es requerido")
               .MaximumLength(ComprobanteEmisorConsts.CodigoPaisMaxLength)
               .WithMessage($"La longitud del Código de país debe tener {ComprobanteEmisorConsts.CodigoPaisMaxLength} caracteres o menos")
                .Custom((x, context) =>
                {
                    if (!String.IsNullOrEmpty(x))
                        if (Regex.Replace(x, @"[A-Za-z]", string.Empty).Length > 0)
                        {
                            context.AddFailure("Código de país contiene caracter no válido.");
                        }
                });

            RuleFor(x => x.FormDto.Telefono)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Teléfono es requerido")
              .MaximumLength(ComprobanteEmisorConsts.TelefonoMaxLength)
              .WithMessage($"La longitud del Teléfono debe tener {ComprobanteEmisorConsts.TelefonoMaxLength} caracteres o menos")
               .Custom((x, context) =>
               {
                   if (!String.IsNullOrEmpty(x))
                       if (Regex.Replace(x, @"[A-Za-z0-9-]", string.Empty).Length > 0)
                       {
                           context.AddFailure("Teléfono contiene caracter no válido.");
                       }
               });

            RuleFor(x => x.FormDto.DireccionAlternativa)
               .Cascade(CascadeMode.Stop)
               .MaximumLength(ComprobanteEmisorConsts.DireccionAlternativaMaxLength)
               .WithMessage($"La longitud de la Dirección debe tener {ComprobanteEmisorConsts.DireccionAlternativaMaxLength} caracteres o menos");

            RuleFor(x => x.FormDto.NumeroResolucion)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Número resolución es requerido")
              .MaximumLength(ComprobanteEmisorConsts.RazonSocialMaxLength)
              .WithMessage($"La longitud del Número resolución debe tener {ComprobanteEmisorConsts.RazonSocialMaxLength} caracteres o menos")
              .Custom((x, context) =>
              {
                  if (!String.IsNullOrEmpty(x))
                      if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                      {
                          context.AddFailure("Número resolución contiene caracter no válido");
                      }
              });

            RuleFor(x => x.FormDto.UsuarioOSE)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Usuario OSE es requerido")
               .MaximumLength(ComprobanteEmisorConsts.UsuarioOseMaxLength)
               .WithMessage($"La longitud del Usuario OSE debe tener {ComprobanteEmisorConsts.UsuarioOseMaxLength} caracteres o menos")
               .Custom((x, context) =>
               {
                   if (!String.IsNullOrEmpty(x))
                       if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n._@*-]", string.Empty).Length > 0)
                       {
                           context.AddFailure("Usuario OSE contiene caracter no válido");
                       }
               });


            RuleFor(x => x.FormDto.ClaveOSE)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Clave OSE es requerido")
              .MaximumLength(ComprobanteEmisorConsts.ClaveOseMaxLength)
              .WithMessage($"La longitud de la Clave OSE debe tener {ComprobanteEmisorConsts.ClaveOseMaxLength} caracteres o menos")
              .Custom((x, context) =>
              {
                  if (!String.IsNullOrEmpty(x))
                      if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()._@*-]", string.Empty).Length > 0)
                      {
                          context.AddFailure("Usuario OSE contiene caracter no válido");
                      }
              });

            RuleFor(x => x.FormDto.CorreoEnvio)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Correo envio es requerido")
              .MaximumLength(ComprobanteEmisorConsts.ClaveOseMaxLength)
              .WithMessage($"La longitud del Correo envio debe tener {ComprobanteEmisorConsts.ClaveOseMaxLength} caracteres o menos")
              .EmailAddress().WithMessage("La dirección de correo envio no es válida");

            RuleFor(x => x.FormDto.CorreoClave)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Contraseña de correo es requerido")
              .MaximumLength(ComprobanteEmisorConsts.ClaveOseMaxLength)
              .WithMessage($"La longitud de la Contraseña de correo debe tener {ComprobanteEmisorConsts.ClaveOseMaxLength} caracteres o menos")
              .Custom((x, context) =>
              {
                  if (!String.IsNullOrEmpty(x))
                      if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()._-]", string.Empty).Length > 0)
                      {
                          context.AddFailure("la Contraseña de correo contiene caracter no válido");
                      }
              });

            RuleFor(x => x.FormDto.ServerMail)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Server Mail es requerido")
              .MaximumLength(ComprobanteEmisorConsts.ServeMailMaxLength)
              .WithMessage($"La longitud de Server Mail debe tener {ComprobanteEmisorConsts.ServeMailMaxLength} caracteres o menos")
               .Custom((x, context) =>
               {
                   if (!String.IsNullOrEmpty(x))
                       if (Regex.Replace(x, @"[A-Za-z0-9.]", string.Empty).Length > 0)
                       {
                           context.AddFailure("El Server Mail contiene caracter no válido");
                       }
               });

            RuleFor(x => x.FormDto.ServerPort)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Server Port es requerido")
              .MaximumLength(ComprobanteEmisorConsts.ServePortMaxLength)
              .WithMessage($"La longitud de Server Port debe tener {ComprobanteEmisorConsts.ServePortMaxLength} caracteres o menos")
              .Custom((x, context) =>
              {
                  if (!String.IsNullOrEmpty(x))
                      if (Regex.Replace(x, @"[0-9]", string.Empty).Length > 0)
                      {
                          context.AddFailure("Server Port debe ser numérico");
                      }
              });

            RuleFor(x => x.FormDto.NombreArchivoCer)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Nombre de archivo certificado es requerido")
              .MaximumLength(ComprobanteEmisorConsts.NombreCertificadoMaxLength)
              .WithMessage($"La longitud del Nombre de archivo Certificado debe tener {ComprobanteEmisorConsts.NombreCertificadoMaxLength} caracteres o menos")
              .Custom((x, context) =>
              {
                  if (!String.IsNullOrEmpty(x))
                      if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#._-]", string.Empty).Length > 0)
                      {
                          context.AddFailure("Nombre de archivo certificado contiene caracter no válido");
                      }
              });

            RuleFor(x => x.FormDto.NombreArchivoKey)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("Nombre de archivo key del certificado es requerido")
              .MaximumLength(ComprobanteEmisorConsts.NombreKeyMaxLength)
              .WithMessage($"La longitud del Nombre de archivo key del certificado debe tener {ComprobanteEmisorConsts.NombreKeyMaxLength} caracteres o menos")
              .Custom((x, context) =>
              {
                  if (!String.IsNullOrEmpty(x))
                      if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°._-]", string.Empty).Length > 0)
                      {
                          context.AddFailure("Nombre de archivo key contiene caracter no válido");
                      }
              });


        }
    }

    public class CommandUpdateValidator : AbstractValidator<CommandUpdate>
    {
        private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
        public CommandUpdateValidator(IUnidadEjecutoraAPI unidadEjecutoraAPI)
        {
            _unidadEjecutoraAPI = unidadEjecutoraAPI;

            Include(new GeneralUpdateValidator());

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

            RuleFor(x => x.FormDto.Estado).NotNull().WithMessage("Estado es requerido");

            RuleFor(x => x.FormDto.UsuarioModificador)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Usuario Modificador es requerido")
                .MaximumLength(ComprobanteEmisorConsts.UsuarioModificadorMaxLength)
                .WithMessage($"La longitud del Usuario Modificador debe tener {ComprobanteEmisorConsts.UsuarioModificadorMaxLength} caracteres o menos");

        }
    }

}
