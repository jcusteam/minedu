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

namespace RecaudacionApiComprobantePago.Application.Command
{
    public class UpdateEstadoComprobantePagoHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public ComprobantePagoEstadoFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;

            public CommandValidator(IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;

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

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(ComprobantePagoConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {ComprobantePagoConsts.UsuarioModificadorMaxLength} caracteres o menos");


            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
        {
            private readonly IComprobantePagoRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            public Handler(IComprobantePagoRepository repository,
                IEstadoAPI estadoAPI)
            {
                _repository = repository;
                _estadoAPI = estadoAPI;
            }

            public async Task<StatusUpdateEstadoResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateEstadoResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.ERROR_SERVER));
                        }
                        response.Success = false;
                    }

                    var comprobantePagoForm = request.FormDto;
                    var comprobantePago = await _repository.FindById(request.Id);

                    if (comprobantePago == null || (request.Id != comprobantePagoForm.ComprobantePagoId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    switch (comprobantePagoForm.Estado)
                    {
                        case Definition.COMPROBANTE_PAGO_ESTADO_EMITIDO:
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                            response.Success = false;
                            return response;
                        case Definition.COMPROBANTE_PAGO_ESTADO_ACEPTADO:
                            if (comprobantePago.Estado != Definition.COMPROBANTE_PAGO_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.COMPROBANTE_PAGO_ESTADO_ACEPTADO_OBS:
                            if (comprobantePago.Estado != Definition.COMPROBANTE_PAGO_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.COMPROBANTE_PAGO_ESTADO_RECHAZADO:
                            if (comprobantePago.Estado != Definition.COMPROBANTE_PAGO_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            }
                            break;
                        default:
                            break;
                    }

                    comprobantePago.Estado = comprobantePagoForm.Estado;
                    comprobantePago.UsuarioModificador = comprobantePagoForm.UsuarioModificador;
                    comprobantePago.FechaModificacion = DateTime.Now;

                    await _repository.Update(comprobantePago);

                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE));
                    response.Success = true;

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
