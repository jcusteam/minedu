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

namespace RecaudacionApiPapeletaDeposito.Application.Command
{
    public class UpdateEstadoPapeletaDepositoHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public PapeletaDepositoEstadoFormDto FormDto { get; set; }
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

        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
        {
            private readonly IPapeletaDepositoRepository _repository;
            private readonly IPapeletaDepositoDetalleRepository _detalleRepository;
            private readonly IEstadoAPI _estadoAPI;

            public Handler(
                IPapeletaDepositoRepository repository,
                IPapeletaDepositoDetalleRepository detalleRepository,
                IEstadoAPI estadoAPI)
            {
                _repository = repository;
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

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    var papeletaDeposito = await _repository.FindById(request.Id);
                    var papeletaDepositoForm = request.FormDto;

                    if (papeletaDeposito == null || (request.Id != papeletaDepositoForm.PapeletaDepositoId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    switch (papeletaDepositoForm.Estado)
                    {
                        case Definition.PAPELETA_DEPOSITO_ESTADO_EMITIDO:
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                            response.Success = false;
                            return response;
                        case Definition.PAPELETA_DEPOSITO_ESTADO_PROCESADO:
                            if (papeletaDeposito.Estado != Definition.PAPELETA_DEPOSITO_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            }
                            break;
                        default:
                            break;
                    }

                    papeletaDeposito.Estado = papeletaDepositoForm.Estado;
                    papeletaDeposito.UsuarioModificador = papeletaDepositoForm.UsuarioModificador;
                    papeletaDeposito.FechaModificacion = DateTime.Now;
                    await _repository.Update(papeletaDeposito);

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
