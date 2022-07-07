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
    public class UpdateDepositoBancoDetalleHandler
    {
        public class StatusUpdateDetalleResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateDetalleResponse>
        {
            public int Id { get; set; }
            public DepositoBancoDetalleFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            public CommandValidator(
                ITipoDocumentoAPI tipoDocumentoAPI)
            {
                _tipoDocumentoAPI = tipoDocumentoAPI;

                RuleFor(x => x.Id)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Depósito de banco detalle es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Depósito de banco detalle no debe ser {x}");
                        }
                    });

                RuleFor(x => x.FormDto.Secuencia)
                    .MaximumLength(DepositoBancoDetalleConsts.SecuenciaMaxLength)
                    .WithMessage($"La longitud de la Secuencia tener {DepositoBancoDetalleConsts.SecuenciaMaxLength} caracteres o menos");

                //RuleFor(x => x.FormDto.TipoDocumento)
                //     .Custom((x, context) =>
                //     {
                //         if (x != null)
                //             if (x < 1)
                //                 context.AddFailure($"Tipo Documento no debe ser {x}");

                //     }).MustAsync(async (id, cancellation) =>
                //     {
                //         bool exists = true;
                //         if (id != null)
                //         {
                //             var response = await _tipoDocumentoAPI.FindByIdAsync(id ?? 0);
                //             exists = response.Success;
                //         }
                //         return exists;

                //     }).WithMessage("Tipo Documento no existe");

                RuleFor(x => x.FormDto.SerieDocumento)
                   .MaximumLength(DepositoBancoDetalleConsts.SirieDocumentoMaxLength)
                   .WithMessage($"La longitud de la Serie de documento debe tener {DepositoBancoDetalleConsts.SirieDocumentoMaxLength} caracteres o menos");

                RuleFor(x => x.FormDto.NumeroDocumento)
                   .MaximumLength(DepositoBancoDetalleConsts.NumeroDocumentoMaxLength)
                   .WithMessage($"La longitud del Número de documento debe tener {DepositoBancoDetalleConsts.NumeroDocumentoMaxLength} caracteres o menos");

                RuleFor(x => x.FormDto.FechaDocumento)
                  .Cascade(CascadeMode.Stop)
                  .NotNull().When(x => x.FormDto.Utilizado == true).WithMessage("Detalle: Fecha Documento es requerido")
                  .Custom((x, context) =>
                  {
                      if (x != null)
                          if (DateTime.Compare(new DateTime(x.Value.Year, x.Value.Month, x.Value.Day, 0, 0, 0), DateTime.Today) > 0)
                          {
                              var date = DateTime.Today.ToString("dd/MM/yyyy");
                              context.AddFailure($"Detalle: Fecha Depósito no puede ser mayor a {date}");
                          }
                  });

                RuleFor(x => x.FormDto.Utilizado).NotNull().WithMessage("Utilizado es requerido");


                RuleFor(x => x.FormDto.UsuarioModificador)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(DepositoBancoDetalleConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {DepositoBancoDetalleConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateDetalleResponse>
        {
            private readonly IDepositoBancoDetalleReposiory _detalleReposiory;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IMapper _mapper;

            public Handler(IDepositoBancoDetalleReposiory detalleReposiory,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IMapper mapper)
            {
                _detalleReposiory = detalleReposiory;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _mapper = mapper;
            }

            public async Task<StatusUpdateDetalleResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusUpdateDetalleResponse();

                try
                {
                    CommandValidator validations = new CommandValidator(_tipoDocumentoAPI);
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
                    var depositoBancoDetalle = await _detalleReposiory.FindById(request.Id);

                    if (depositoBancoDetalle == null || (request.Id != request.FormDto.DepositoBancoDetalleId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    var depositoBancoDetalleForm = _mapper.Map<DepositoBancoDetalleFormDto, DepositoBancoDetalle>(request.FormDto);

                    depositoBancoDetalle.Secuencia = depositoBancoDetalleForm.Secuencia;
                    depositoBancoDetalle.TipoDocumento = depositoBancoDetalleForm.TipoDocumento;
                    depositoBancoDetalle.SerieDocumento = depositoBancoDetalleForm.SerieDocumento;
                    depositoBancoDetalle.NumeroDocumento = depositoBancoDetalleForm.NumeroDocumento;
                    depositoBancoDetalle.Utilizado = depositoBancoDetalleForm.Utilizado;
                    depositoBancoDetalle.FechaDocumento = depositoBancoDetalleForm.FechaDocumento;
                    depositoBancoDetalle.UsuarioModificador = depositoBancoDetalleForm.UsuarioModificador;
                    depositoBancoDetalle.FechaModificacion = DateTime.Now;
                    await _detalleReposiory.Update(depositoBancoDetalle);
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
