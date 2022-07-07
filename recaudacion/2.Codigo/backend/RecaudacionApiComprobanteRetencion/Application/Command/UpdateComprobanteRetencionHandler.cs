using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Application.Command.Dtos;
using RecaudacionApiComprobanteRetencion.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiComprobanteRetencion.Domain;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiComprobanteRetencion.Helpers;
using RecaudacionApiComprobanteRetencion.Clients;
using System.Text.RegularExpressions;

namespace RecaudacionApiComprobanteRetencion.Application.Command
{
    public class UpdateComprobanteRetencionHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
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
                         context.AddFailure($"Total Retenido no debe ser {x}");
                 });

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

                RuleFor(x => x.FormDto.Total)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Total Retenido es requerido")
                 .Custom((x, context) =>
                 {
                     if (x <= 0)
                         context.AddFailure($"Total Retenido no debe ser {x}");
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

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(ComprobanteRetencionConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {ComprobanteRetencionConsts.UsuarioModificadorMaxLength} caracteres o menos");

            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;

            public CommandValidator(
                 IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;

                Include(new GeneralValidator());

                RuleFor(x => x.Id).NotEmpty().WithMessage("Id Comprobante Retención es requerido")
                  .Custom((x, context) =>
                  {
                      if (x < 1)
                      {
                          context.AddFailure($"Id Comprobante Retención no debe ser {x}");
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

        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
        {
            private readonly IComprobanteRetencionRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(IComprobanteRetencionRepository repository,
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

                        var comprobanteRetencion = await _repository.FindById(request.Id);
                        var comprobanteRetencionForm = _mapper.Map<ComprobanteRetencionFormDto, ComprobanteRetencion>(request.FormDto);

                        if (comprobanteRetencion == null || (request.Id != comprobanteRetencionForm.ComprobanteRetencionId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }

                        comprobanteRetencion.EstadoSunat = comprobanteRetencionForm.EstadoSunat;
                        comprobanteRetencion.UsuarioModificador = comprobanteRetencionForm.UsuarioModificador;
                        comprobanteRetencion.FechaModificacion = DateTime.Now;

                        await _repository.Update(comprobanteRetencion);
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
