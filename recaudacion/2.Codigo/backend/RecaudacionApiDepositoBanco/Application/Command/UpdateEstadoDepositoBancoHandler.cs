using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Application.Command.Dtos;
using RecaudacionApiDepositoBanco.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiDepositoBanco.Domain;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiDepositoBanco.Helpers;
using RecaudacionApiDepositoBanco.Clients;

namespace RecaudacionApiDepositoBanco.Application.Command
{
    public class UpdateEstadoDepositoBancoHandler
    {
        public class StatusUpdateEstadoResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateEstadoResponse>
        {
            public int Id { get; set; }
            public DepositoBancoEstadoFormDto FormDto { get; set; }
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
                    .NotEmpty().WithMessage("Id Depósito de banco es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Depósito de banco no debe ser {x}");
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
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_DEPOSITO_BANCO, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Numero de estado no existe");

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(DepositoBancoConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {DepositoBancoConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateEstadoResponse>
        {
            private readonly IDepositoBancoRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(IDepositoBancoRepository repository,
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

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        //return response;
                    }

                    var depositoBanco = await _repository.FindById(request.Id);
                    var depositoBancoForm = request.FormDto;

                    if (depositoBanco == null || (request.Id != depositoBancoForm.DepositoBancoId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    switch (depositoBancoForm.Estado)
                    {
                        case Definition.DEPOSITO_BANCO_ESTADO_EMITIDO:
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                            response.Success = false;
                            return response;
                        case Definition.DEPOSITO_BANCO_ESTADO_PROCESADO:
                            if (depositoBanco.Estado != Definition.DEPOSITO_BANCO_ESTADO_EMITIDO)
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_UPDATE_ESTADO));
                                response.Success = false;
                                return response;
                            }
                            break;
                        default:
                            break;
                    }

                    depositoBanco.Estado = depositoBancoForm.Estado;
                    depositoBanco.FechaModificacion = DateTime.Now;
                    depositoBanco.UsuarioModificador = depositoBancoForm.UsuarioModificador;

                    await _repository.Update(depositoBanco);
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
