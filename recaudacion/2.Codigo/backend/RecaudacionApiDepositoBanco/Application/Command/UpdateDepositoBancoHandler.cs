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
using System.Text.RegularExpressions;

namespace RecaudacionApiDepositoBanco.Application.Command
{
    public class UpdateDepositoBancoHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public DepositoBancoFormDto FormDto { get; set; }
        }

        // General validación
        public class GeneralValidator : AbstractValidator<Command>
        {
            public GeneralValidator()
            {
                RuleFor(x => x.FormDto.NombreArchivo)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Nombre de Archivo es requerido")
                    .MaximumLength(DepositoBancoConsts.NombreArchivoMaxLength)
                    .WithMessage($"La longitud del Nombre de Archivo debe tener {DepositoBancoConsts.NombreArchivoMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (!String.IsNullOrEmpty(x))
                            if (Regex.Replace(x, @"[A-Za-z0-9._-]", string.Empty).Length > 0)
                            {
                                context.AddFailure("El Nombre de Archivo contiene caracter no válido");
                            }
                    });

                RuleFor(x => x.FormDto.FechaDeposito)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("Fecha Depósito es requerido")
                 .Custom((x, context) =>
                 {
                     if (x != null)
                         if (x.Date > DateTime.Now.Date)
                         {
                             var date = DateTime.Now.ToString("dd/MM/yyyy");
                             context.AddFailure($"Fecha Depósito no puede ser mayor {date}");
                         }
                 });

                RuleFor(x => x.FormDto.UsuarioModificador)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Usuario Modificador es requerido")
                  .MaximumLength(DepositoBancoConsts.UsuarioModificadorMaxLength)
                  .WithMessage($"La longitud del Usuario Modificador debe tener {DepositoBancoConsts.UsuarioModificadorMaxLength} caracteres o menos");

            }
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

            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
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

            public async Task<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {
                        var depositoBanco = await _repository.FindById(request.Id);

                        if (depositoBanco == null || (request.Id != request.FormDto.DepositoBancoId))
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                            response.Success = false;
                            return response;
                        }

                        var depositoBancoForm = _mapper.Map<DepositoBancoFormDto, DepositoBanco>(request.FormDto);

                        depositoBanco.FechaModificacion = DateTime.Now;
                        depositoBanco.UsuarioModificador = depositoBancoForm.UsuarioModificador;
                        await _repository.Update(depositoBanco);
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
