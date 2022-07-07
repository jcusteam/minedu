using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Application.Command.Dtos;
using RecaudacionApiPapeletaDeposito.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiPapeletaDeposito.Domain;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiPapeletaDeposito.Helpers;
using RecaudacionApiPapeletaDeposito.Clients;
using System.Text.RegularExpressions;

namespace RecaudacionApiPapeletaDeposito.Application.Command
{
    public class UpdatePapeletaDepositoHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public PapeletaDepositoFormDto FormDto { get; set; }
        }

        public class DetallesValidator : AbstractValidator<Command>
        {
            private readonly IReciboIngresoAPI _reciboIngresoAPI;
            public DetallesValidator(
                IReciboIngresoAPI reciboIngresoAPI)
            {
                _reciboIngresoAPI = reciboIngresoAPI;

                // Detalle
                RuleFor(x => x.FormDto.PapeletaDepositoDetalle)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("Detalle de Papeleta de Depósito es requerido")
                 .Custom((x, context) =>
                 {
                     if (x.Count == 0)
                     {
                         context.AddFailure($"Detalle de Papeleta de Depósito es requerido");
                     }
                 })
                 .ForEach(detalles =>
                 {
                     detalles.ChildRules(detalle =>
                     {
                         detalle.RuleFor(x => x.ReciboIngresoId)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Id Recibo Ingreso es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 1)
                                    {
                                        context.AddFailure($"Detalle: Id Recibo Ingreso no debe ser {x}");
                                    }
                                })
                                .MustAsync(async (id, cancellation) =>
                                {
                                    var response = await _reciboIngresoAPI.FindByIdAsync(id);
                                    bool exists = response.Success;
                                    return exists;
                                }).WithMessage("Detalle: Id Recibo Ingreso no existe ");

                         detalle.RuleFor(x => x.Monto)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Monto es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Monto no debe ser {x}");
                                    }
                                });

                     });
                 });

            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;
            public CommandValidator(IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;
                RuleFor(x => x.Id)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Papeleta depósito de banco es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Papeleta depósito de banco no debe ser {x}");
                        }
                    });

                RuleFor(x => x.FormDto.Descripcion)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Descripción es requerido")
                  .MaximumLength(PapeletaDepositoConsts.DescripcionMaxLength)
                  .WithMessage($"La longitud de la Descripción debe tener {PapeletaDepositoConsts.DescripcionMaxLength} caracteres o menos")
                  .Custom((x, context) =>
                  {
                      if (!string.IsNullOrEmpty(x))
                          if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]", string.Empty).Length > 0)
                          {
                              context.AddFailure("El Concepto contiene caracter no válido");
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
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_PAPELETA_DEPOSITO, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Numero de estado no existe");

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(PapeletaDepositoConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {PapeletaDepositoConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
        {
            private readonly IPapeletaDepositoRepository _repository;
            private readonly IPapeletaDepositoDetalleRepository _detalleRepository;
            private readonly IReciboIngresoAPI _reciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;

            public Handler(
                IPapeletaDepositoRepository repository,
                IPapeletaDepositoDetalleRepository detalleRepository,
                IReciboIngresoAPI reciboIngresoAPI,
                IEstadoAPI estadoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
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

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                    }



                    var papeletaDepositoEx = await _repository.FindById(request.Id);

                    if (papeletaDepositoEx == null || (request.Id != request.FormDto.PapeletaDepositoId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    var papeletaDeposito = _mapper.Map<PapeletaDepositoFormDto, PapeletaDeposito>(request.FormDto);

                    // Detalle validación
                    if (papeletaDeposito.Estado == Definition.PAPELETA_DEPOSITO_ESTADO_EMITIDO)
                    {
                        DetallesValidator detalleValidations = new DetallesValidator(_reciboIngresoAPI);
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

                        var detalles = await _detalleRepository.FindAll(papeletaDeposito.PapeletaDepositoId);

                        foreach (var item in detalles)
                        {
                            await _detalleRepository.Delete(item);
                        }


                        foreach (var item in request.FormDto.PapeletaDepositoDetalle)
                        {

                            var reciboIngresoResponse = await _reciboIngresoAPI.FindByIdAsync(item.ReciboIngresoId);

                            if (reciboIngresoResponse.Success)
                            {
                                if (reciboIngresoResponse.Data.Estado != Definition.RECIBO_INGRESO_ESTADO_TRANSMITIDO)
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"Detalle: El número '{reciboIngresoResponse.Data.Numero}' de recibo de ingreso debe estar en estado 'Transmitido'"));
                                    response.Success = false;
                                    return response;
                                }
                            }

                            var papeletaDepositoDetalle = await _detalleRepository.FindByReciboIngreso(item.ReciboIngresoId);

                            if (papeletaDepositoDetalle != null)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"Detalle: El número '{reciboIngresoResponse.Data.Numero}' de recibo de ingreso ya se encuentra registrado"));
                                response.Success = false;
                                return response;
                            }
                        }

                    }

                    papeletaDeposito.Estado = papeletaDepositoEx.Estado;
                    papeletaDeposito.FechaModificacion = DateTime.Now;
                    await _repository.Update(papeletaDeposito);

                    if (papeletaDeposito.Estado == Definition.PAPELETA_DEPOSITO_ESTADO_EMITIDO)
                    {
                        if (request.FormDto.PapeletaDepositoDetalle.Count > 0)
                        {

                            foreach (var item in request.FormDto.PapeletaDepositoDetalle)
                            {
                                var detalle = _mapper.Map<PapeletaDepositoDetalleFormDto, PapeletaDepositoDetalle>(item);
                                detalle.PapeletaDepositoId = papeletaDeposito.PapeletaDepositoId;
                                detalle.UsuarioCreador = papeletaDeposito.UsuarioModificador;
                                detalle.FechaCreacion = DateTime.Now;
                                await _detalleRepository.Add(detalle);
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
