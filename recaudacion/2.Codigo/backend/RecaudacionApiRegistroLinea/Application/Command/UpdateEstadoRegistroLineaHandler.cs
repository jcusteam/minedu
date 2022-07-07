using AutoMapper;
using FluentValidation;
using MediatR;
using RecaudacionApiRegistroLinea.Application.Command.Dtos;
using RecaudacionApiRegistroLinea.Clients;
using RecaudacionApiRegistroLinea.DataAccess;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionApiRegistroLinea.Helpers;
using RecaudacionUtils;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RecaudacionApiRegistroLinea.Application.Command
{
    public class UpdateEstadoRegistroLineaHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public RegistroLineaEstadoFormDto FormDto { get; set; }
        }

        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("Id Registro Linea es requerido")
                   .Custom((x, context) =>
                   {
                       if (x < 1)
                       {
                           context.AddFailure($"Id Registro Linea no debe ser {x}");
                       }
                   });

                RuleFor(x => x.FormDto.Observacion)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().When(x => x.FormDto.Estado == Definition.REGISTRO_LINEA_ESTADO_OBSERVADO).WithMessage("La Observación es requerido")
                 .MaximumLength(RegistroLineaConsts.ObservacionMaxLength)
                 .WithMessage($"La longitud de la Observación debe tener {RegistroLineaConsts.ObservacionMaxLength} caracteres o menos")
                 .Custom((x, context) =>
                 {
                     if (!string.IsNullOrEmpty(x))
                         if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]", string.Empty).Length > 0)
                         {
                             context.AddFailure("La Observación contiene caracter no válido");
                         }
                 });

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(RegistroLineaConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {RegistroLineaConsts.UsuarioModificadorMaxLength} caracteres o menos");

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
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_REGISTRO_LINEA, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Número de estado no existe");

            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
        {
            private readonly IRegistroLineaRepository _repository;
            private readonly IRegistroLineaDetalleRepository _detalleRepository;
            private readonly IReciboIngresoAPI _reciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            public Handler(IRegistroLineaRepository repository,
                IReciboIngresoAPI reciboIngresoAPI,
                IRegistroLineaDetalleRepository detalleRepository,
                IEstadoAPI estadoAPI)
            {
                _repository = repository;
                _reciboIngresoAPI = reciboIngresoAPI;
                _detalleRepository = detalleRepository;
                _estadoAPI = estadoAPI;

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
                        var registroLineaForm = request.FormDto;
                        var registroLinea = await _repository.FindById(request.Id);

                        if (registroLinea == null || (request.Id != request.FormDto.RegistroLineaId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }

                        switch (registroLineaForm.Estado)
                        {
                            case Definition.REGISTRO_LINEA_ESTADO_EMITIDO:
                                if (registroLinea.Estado == Definition.REGISTRO_LINEA_ESTADO_EMITIDO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro"));
                                    response.Success = false;
                                    return response;
                                }
                                break;
                            case Definition.REGISTRO_LINEA_ESTADO_EN_PROCESO:
                                if (registroLinea.Estado != Definition.REGISTRO_LINEA_ESTADO_EMITIDO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Emitido'"));
                                    response.Success = false;
                                    return response;
                                }
                                break;
                            case Definition.REGISTRO_LINEA_ESTADO_DERIVADO:
                                if (registroLinea.Estado != Definition.REGISTRO_LINEA_ESTADO_EN_PROCESO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'En Proceso'"));
                                    response.Success = false;
                                    return response;
                                }
                                break;
                            case Definition.REGISTRO_LINEA_ESTADO_DESESTIMADO:
                                if (registroLinea.Estado != Definition.REGISTRO_LINEA_ESTADO_EN_PROCESO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'En Proceso'"));
                                    response.Success = false;
                                    return response;
                                }
                                break;
                            case Definition.REGISTRO_LINEA_ESTADO_OBSERVADO:

                                if (registroLinea.Estado != Definition.REGISTRO_LINEA_ESTADO_EN_PROCESO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'En Proceso'"));
                                    response.Success = false;
                                    return response;
                                }

                                if (String.IsNullOrEmpty(registroLineaForm.Observacion))
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "La Observación es requerido"));
                                    response.Success = false;
                                    return response;
                                }

                                break;
                            case Definition.REGISTRO_LINEA_ESTADO_AUTORIZAR:
                                if (registroLinea.Estado != Definition.REGISTRO_LINEA_ESTADO_DERIVADO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Derivado'"));
                                    response.Success = false;
                                    return response;
                                }
                                break;
                            case Definition.REGISTRO_LINEA_ESTADO_EMITIR_RI:

                                if (registroLinea.Estado != Definition.REGISTRO_LINEA_ESTADO_AUTORIZAR)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "No se puede actualizar el estado del Registro, debe estar en estado 'Autorizado'"));
                                    response.Success = false;
                                    return response;
                                }

                                var reciboIngreso = new ReciboIngreso();

                                reciboIngreso.UnidadEjecutoraId = registroLinea.UnidadEjecutoraId;
                                reciboIngreso.TipoReciboIngresoId = registroLinea.TipoReciboIngresoId;
                                reciboIngreso.ClienteId = registroLinea.ClienteId;
                                reciboIngreso.CuentaCorrienteId = registroLinea.CuentaCorrienteId;
                                reciboIngreso.FuenteFinanciamientoId = Definition.FUENTE_FINANCIAMIENTO_RECURSO_ORDINARIO;
                                reciboIngreso.RegistroLineaId = registroLinea.RegistroLineaId;
                                reciboIngreso.TipoDocumentoId = Definition.TIPO_DOCUMENTO_RECIBO_INGRESO;
                                reciboIngreso.Numero = "";
                                reciboIngreso.FechaEmision = DateTime.Now;
                                reciboIngreso.TipoCaptacionId = Definition.TIPO_CAPTACION_DEPOSITO_CUENTA;
                                reciboIngreso.DepositoBancoDetalleId = registroLinea.DepositoBancoDetalleId;
                                reciboIngreso.ImporteTotal = registroLinea.ImporteDeposito;
                                reciboIngreso.NumeroDeposito = registroLinea.NumeroDeposito;
                                reciboIngreso.FechaDeposito = registroLinea.FechaDeposito;
                                reciboIngreso.ValidarDeposito = registroLinea.ValidarDeposito;
                                reciboIngreso.NumeroOficio = registroLinea.NumeroOficio;
                                reciboIngreso.NumeroComprobantePago = registroLinea.NumeroComprobantePago;
                                reciboIngreso.ExpedienteSiaf = registroLinea.ExpedienteSiaf;
                                reciboIngreso.NumeroResolucion = registroLinea.NumeroResolucion;
                                reciboIngreso.LiquidacionId = null;
                                reciboIngreso.Estado = Definition.RECIBO_INGRESO_ESTADO_EMITIDO;
                                reciboIngreso.UsuarioCreador = registroLinea.UsuarioModificador;

                                var detalles = await _detalleRepository.FindAll(registroLinea.RegistroLineaId);

                                foreach (var item in detalles)
                                {
                                    var reciboIngresoDetalle = new ReciboIngresoDetalle();
                                    reciboIngresoDetalle.ClasificadorIngresoId = item.ClasificadorIngresoId;
                                    reciboIngresoDetalle.Importe = item.Importe;
                                    reciboIngresoDetalle.Referencia = "";
                                    reciboIngresoDetalle.UsuarioCreador = registroLinea.UsuarioModificador;
                                    reciboIngresoDetalle.Estado = "1";
                                    reciboIngreso.ReciboIngresoDetalle.Add(reciboIngresoDetalle);
                                }

                                var reciboIngresoResponse = await _reciboIngresoAPI.AddAsync(reciboIngreso);

                                if (!reciboIngresoResponse.Success)
                                {
                                    response.Messages.Add(new GenericMessage(reciboIngresoResponse.Messages[0].Type, $"Servicio de Recibo de Ingreso: {reciboIngresoResponse.Messages[0].Message}"));
                                    response.Success = false;
                                    return response;
                                }

                                registroLinea.ReciboIngresoId = reciboIngresoResponse.Data.ReciboIngresoId;

                                break;
                            default:
                                // code block
                                break;
                        }

                        registroLinea.Estado = registroLineaForm.Estado;
                        registroLinea.UsuarioModificador = registroLineaForm.UsuarioModificador;
                        registroLinea.FechaModificacion = DateTime.Now;
                        await _repository.Update(registroLinea);

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
