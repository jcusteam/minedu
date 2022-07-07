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
    public class UpdateLiquidacionHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public LiquidacionFormDto FormDto { get; set; }
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

        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
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

            public async Task<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {

                        var liquidacion = await _repository.FindById(request.Id);

                        if (liquidacion == null || (request.Id != request.FormDto.LiquidacionId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }


                        var liquidacionForm = _mapper.Map<LiquidacionFormDto, Liquidacion>(request.FormDto);

                        liquidacion.Estado = liquidacionForm.Estado;
                        liquidacion.UsuarioModificador = liquidacionForm.UsuarioModificador;
                        liquidacion.FechaModificacion = DateTime.Now;

                        if (liquidacion.Estado == Definition.LIQUIDACION_ESTADO_EMITIR_RI)
                        {
                            var reciboIngreso = new ReciboIngreso();

                            reciboIngreso.UnidadEjecutoraId = liquidacion.UnidadEjecutoraId;
                            reciboIngreso.TipoReciboIngresoId = 1;
                            reciboIngreso.ClienteId = liquidacion.ClienteId;
                            reciboIngreso.CuentaCorrienteId = liquidacion.CuentaCorrienteId;
                            reciboIngreso.FuenteFinanciamientoId = 1;
                            reciboIngreso.RegistroLineaId = 0;
                            reciboIngreso.TipoDocumentoId = Definition.TIPO_DOCUMENTO_RECIBO_INGRESO;
                            reciboIngreso.Numero = "";
                            reciboIngreso.FechaEmision = DateTime.Now;
                            reciboIngreso.TipoCaptacionId = Definition.TIPO_CAPTACION_VARIOS;
                            reciboIngreso.DepositoBancoDetalleId = 0;
                            reciboIngreso.ImporteTotal = liquidacion.Total;
                            reciboIngreso.NumeroDeposito = "";
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

                            var detalles = await _repository.FindDetalleById(liquidacion.LiquidacionId);

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

                            if (reciboIngresoResponse.Success)
                            {
                                liquidacion.ReciboIngresoId = reciboIngresoResponse.Data.ReciboIngresoId;
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error al momento de emitir el Recibo de Ingreso"));
                                response.Success = false;
                                return response;
                            }
                        }

                        await _repository.Update(liquidacion);

                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE));

                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
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
