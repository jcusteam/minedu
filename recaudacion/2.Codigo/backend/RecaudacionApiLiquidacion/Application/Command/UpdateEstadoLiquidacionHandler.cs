using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Application.Command.Dtos;
using RecaudacionApiLiquidacion.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiLiquidacion.Domain;
using AutoMapper;
using RecaudacionApiLiquidacion.Clients;
using RecaudacionUtils;
using RecaudacionApiLiquidacion.Helpers;

namespace RecaudacionApiLiquidacion.Application.Command
{
    public class UpdateEstadoLiquidacionHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public LiquidacionEstadoFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;
            public CommandValidator(
                IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;

                RuleFor(x => x.Id)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Id Liquidación es requerido")
                   .Custom((x, context) =>
                   {
                       if (x < 1)
                       {
                           context.AddFailure($"Id Liquidación no debe ser {x}");
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
                       var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_LIQUIDACION, estado);
                       bool exists = response.Success;
                       return exists;
                   }).WithMessage("Número de estado no existe");

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(LiquidacionConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {LiquidacionConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
        {
            private readonly ILiquidacionRepository _repository;
            private readonly IReciboIngresoAPI _reciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(ILiquidacionRepository repository,
                 IReciboIngresoAPI reciboIngresoAPI,
                 IEstadoAPI estadoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _reciboIngresoAPI = reciboIngresoAPI;
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

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    var liquidacion = await _repository.FindById(request.Id);

                    if (liquidacion == null || (request.Id != request.FormDto.LiquidacionId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }


                    var liquidacionForm = request.FormDto;

                    switch (liquidacionForm.Estado)
                    {
                        case Definition.LIQUIDACION_ESTADO_EMITIDO:
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                            response.Success = false;
                            return response;
                        case Definition.LIQUIDACION_ESTADO_PROCESADO:
                            if (liquidacion.Estado != Definition.LIQUIDACION_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            }
                            break;
                        case Definition.LIQUIDACION_ESTADO_EMITIR_RI:
                            if (liquidacion.Estado != Definition.LIQUIDACION_ESTADO_PROCESADO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            }

                            var reciboIngreso = new ReciboIngreso();

                            reciboIngreso.UnidadEjecutoraId = liquidacion.UnidadEjecutoraId;
                            reciboIngreso.TipoReciboIngresoId = Definition.TIPO_RECIBO_INGRESO_CAPTACION_VENTANILLA;
                            reciboIngreso.ClienteId = liquidacion.ClienteId;
                            reciboIngreso.CuentaCorrienteId = liquidacion.CuentaCorrienteId;
                            reciboIngreso.FuenteFinanciamientoId = liquidacion.FuenteFinanciamientoId;
                            reciboIngreso.RegistroLineaId = null;
                            reciboIngreso.TipoDocumentoId = Definition.TIPO_DOCUMENTO_RECIBO_INGRESO;
                            reciboIngreso.Numero = "0000";
                            reciboIngreso.FechaEmision = DateTime.Now;
                            reciboIngreso.TipoCaptacionId = Definition.TIPO_CAPTACION_VARIOS;
                            reciboIngreso.DepositoBancoDetalleId = null;
                            reciboIngreso.ImporteTotal = liquidacion.Total;
                            reciboIngreso.NumeroDeposito = null;
                            reciboIngreso.FechaDeposito = null;
                            reciboIngreso.NumeroCheque = "";
                            reciboIngreso.NumeroOficio = "";
                            reciboIngreso.NumeroComprobantePago = "";
                            reciboIngreso.ExpedienteSiaf = "";
                            reciboIngreso.NumeroResolucion = "";
                            reciboIngreso.CartaOrden = "";
                            reciboIngreso.LiquidacionIngreso = "";
                            reciboIngreso.PapeletaDeposito = "";
                            reciboIngreso.Concepto = "";
                            reciboIngreso.Referencia = "";
                            reciboIngreso.Estado = Definition.RECIBO_INGRESO_ESTADO_EMITIDO;
                            reciboIngreso.LiquidacionId = liquidacion.LiquidacionId;
                            reciboIngreso.UsuarioCreador = liquidacion.UsuarioModificador;

                            var detalles = await _repository.GrupDetalleByClasificador(liquidacion.LiquidacionId);

                            foreach (var item in detalles)
                            {
                                var reciboIngresoDetalle = new ReciboIngresoDetalle();
                                reciboIngresoDetalle.ClasificadorIngresoId = item.ClasificadorIngresoId;
                                reciboIngresoDetalle.Importe = item.ImporteParcial;
                                reciboIngresoDetalle.Referencia = "";
                                reciboIngresoDetalle.Estado = "1";
                                reciboIngresoDetalle.UsuarioCreador = liquidacion.UsuarioModificador;
                                reciboIngreso.ReciboIngresoDetalle.Add(reciboIngresoDetalle);

                            }

                            var reciboIngresoResponse = await _reciboIngresoAPI.AddAsync(reciboIngreso);

                            if (!reciboIngresoResponse.Success)
                            {
                                response.Messages.Add(new GenericMessage(reciboIngresoResponse.Messages[0].Type, $"Servicio de Recibo de Ingreso: {reciboIngresoResponse.Messages[0].Message}"));
                                response.Success = false;
                                return response;
                            }

                            liquidacion.ReciboIngresoId = reciboIngresoResponse.Data.ReciboIngresoId;

                            break;
                        default:
                            break;
                    }

                    liquidacion.Estado = liquidacionForm.Estado;
                    liquidacion.UsuarioModificador = liquidacionForm.UsuarioModificador;
                    liquidacion.FechaModificacion = DateTime.Now;
                    await _repository.Update(liquidacion);
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
