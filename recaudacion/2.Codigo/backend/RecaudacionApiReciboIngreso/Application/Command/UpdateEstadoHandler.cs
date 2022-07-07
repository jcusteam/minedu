using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Application.Command.Dtos;
using RecaudacionApiReciboIngreso.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiReciboIngreso.Domain;
using AutoMapper;
using RecaudacionApiReciboIngreso.Clients;
using RecaudacionUtils;
using RecaudacionApiReciboIngreso.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiReciboIngreso.Application.Command
{
    public class UpdateEstadoHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public ReciboIngresoEstadoFormDto FormDto { get; set; }
        }

        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
                RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Id Recibo Ingreso es requerido")
                .Custom((x, context) =>
                {
                    if (x < 1)
                    {
                        context.AddFailure($"Id Recibo Ingreso no debe ser {x}");
                    }
                });

                RuleFor(x => x.FormDto.UsuarioModificador)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Usuario Modificador es requerido")
                   .MaximumLength(ReciboIngresoConsts.UsuarioModificadorMaxLength)
                   .WithMessage($"La longitud del Usuario Modificador debe tener {ReciboIngresoConsts.UsuarioModificadorMaxLength} caracteres o menos");

            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;
            public CommandValidator(IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;

                Include(new GeneralValidator());

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
                       var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_RECIBO_INGRESO, estado);
                       bool exists = response.Success;
                       return exists;
                   }).WithMessage("Número de estado no existe");


            }
        }



        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
        {
            private readonly IReciboIngresoRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IDepositoBancoAPI _depositoBancoAPI;

            public Handler(IReciboIngresoRepository repository,
                IEstadoAPI estadoAPI,
                IDepositoBancoAPI depositoBancoAPI)
            {
                _repository = repository;
                _estadoAPI = estadoAPI;
                _depositoBancoAPI = depositoBancoAPI;
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
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    var reciboIngresoForm = request.FormDto;

                    var reciboIngreso = await _repository.FindById(request.Id);

                    if (reciboIngreso == null || (request.Id != request.FormDto.ReciboIngresoId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }


                    switch (reciboIngresoForm.Estado)
                    {
                        case Definition.RECIBO_INGRESO_ESTADO_EMITIDO:
                            break;
                        case Definition.RECIBO_INGRESO_ESTADO_PROCESADO:
                            if (reciboIngreso.Estado != Definition.RECIBO_INGRESO_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Emitido'"));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.RECIBO_INGRESO_ESTADO_CONFIRMADO:
                            if (reciboIngreso.Estado != Definition.RECIBO_INGRESO_ESTADO_PROCESADO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Procesado'"));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.RECIBO_INGRESO_ESTADO_ENVIADO_SIAF:
                            if (reciboIngreso.Estado != Definition.RECIBO_INGRESO_ESTADO_CONFIRMADO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Confirmado'"));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.RECIBO_INGRESO_ESTADO_TRANSMITIDO:
                            if (reciboIngreso.Estado != Definition.RECIBO_INGRESO_ESTADO_ENVIADO_SIAF)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Enviado SIAF'"));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.RECIBO_INGRESO_ESTADO_RECHAZADO:
                            if (reciboIngreso.Estado != Definition.RECIBO_INGRESO_ESTADO_TRANSMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Transmitido'"));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.RECIBO_INGRESO_ESTADO_ANULADO:
                            if (reciboIngreso.Estado != Definition.RECIBO_INGRESO_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Emitido'"));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.RECIBO_INGRESO_ESTADO_ANULACION_POSTERIOR:
                            break;
                        default:
                            // code block
                            break;
                    }

                    reciboIngreso.Estado = reciboIngresoForm.Estado;
                    reciboIngreso.UsuarioModificador = reciboIngresoForm.UsuarioModificador;
                    reciboIngreso.FechaModificacion = DateTime.Now;
                    await _repository.Update(reciboIngreso);

                    if (reciboIngreso.Estado == Definition.RECIBO_INGRESO_ESTADO_EMITIDO)
                    {
                        if (reciboIngreso.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA)
                        {
                            var responseDepositoBancoDetalle = await _depositoBancoAPI.FindDetalleByIdAsync(reciboIngreso.DepositoBancoDetalleId ?? 0);

                            if (responseDepositoBancoDetalle.Success)
                            {
                                var depositoBancoDetalle = responseDepositoBancoDetalle.Data;
                                depositoBancoDetalle.TipoDocumento = null;
                                depositoBancoDetalle.NumeroDocumento = null;
                                depositoBancoDetalle.FechaDocumento = null;
                                depositoBancoDetalle.Utilizado = Definition.DEPOSITO_BANCO_DETALLE_UTILIZADO_NO;
                                depositoBancoDetalle.UsuarioModificador = reciboIngreso.UsuarioModificador;
                                await _depositoBancoAPI.UpdateDetalleAsync(depositoBancoDetalle.DepositoBancoDetalleId, depositoBancoDetalle);

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
