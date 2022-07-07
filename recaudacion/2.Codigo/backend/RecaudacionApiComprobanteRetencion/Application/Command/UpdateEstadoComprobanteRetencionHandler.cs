using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Application.Command.Dtos;
using RecaudacionApiComprobanteRetencion.DataAccess;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiComprobanteRetencion.Helpers;
using RecaudacionApiComprobanteRetencion.Clients;

namespace RecaudacionApiComprobanteRetencion.Application.Command
{
    public class UpdateEstadoComprobanteRetencionHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public ComprobanteRetencionEstadoFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;

            public CommandValidator(
                 IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;
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


                RuleFor(x => x.FormDto.UsuarioModificador)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(ComprobanteRetencionConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {ComprobanteRetencionConsts.UsuarioModificadorMaxLength} caracteres o menos");

            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
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

            public async Task<StatusUpdateEstadoResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateEstadoResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {

                        var comprobanteRetencion = await _repository.FindById(request.Id);

                        if (comprobanteRetencion == null || (request.Id != request.FormDto.ComprobanteRetencionId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }

                        var comprobanteRetencionForm = request.FormDto;

                        switch (comprobanteRetencionForm.Estado)
                        {
                            case Definition.COMPROBANTE_RETENCION_ESTADO_EMITIDO:
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            case Definition.COMPROBANTE_RETENCION_ESTADO_ACEPTADO:
                                if (comprobanteRetencion.Estado != Definition.COMPROBANTE_RETENCION_ESTADO_EMITIDO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                    response.Success = false;
                                    return response;
                                }
                                break;
                            case Definition.COMPROBANTE_RETENCION_ESTADO_ACEPTADO_OBS:
                                if (comprobanteRetencion.Estado != Definition.COMPROBANTE_RETENCION_ESTADO_EMITIDO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                    response.Success = false;
                                    return response;
                                }
                                break;
                            case Definition.COMPROBANTE_RETENCION_ESTADO_RECHAZADO:
                                if (comprobanteRetencion.Estado != Definition.COMPROBANTE_RETENCION_ESTADO_EMITIDO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                    response.Success = false;
                                    return response;
                                }
                                break;
                            default:
                                break;
                        }

                        comprobanteRetencion.Estado = comprobanteRetencionForm.Estado;
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
