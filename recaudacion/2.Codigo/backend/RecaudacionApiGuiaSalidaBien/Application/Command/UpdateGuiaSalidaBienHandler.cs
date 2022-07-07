using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Application.Command.Dtos;
using RecaudacionApiGuiaSalidaBien.DataAccess;
using FluentValidation;
using MediatR;
using RecaudacionApiGuiaSalidaBien.Domain;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiGuiaSalidaBien.Helpers;
using RecaudacionApiGuiaSalidaBien.Clients;

namespace RecaudacionApiGuiaSalidaBien.Application.Command
{
    public class UpdateGuiaSalidaBienHandler
    {
        public class StatusUpdateResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusUpdateResponse>
        {
            public int Id { get; set; }
            public GuiaSalidaBienFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;
            public CommandValidator(
                IEstadoAPI estadoAPI)
            {
                _estadoAPI = estadoAPI;

                RuleFor(x => x.Id).NotEmpty().WithMessage("Id Guia de Salida Bien es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Id Guia de Salida Bien no debe ser {x}");
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
                       var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_GUIA_SALIDA_BIEN, estado);
                       bool exists = response.Success;
                       return exists;
                   }).WithMessage("Número de estado no existe");

                RuleFor(x => x.FormDto.UsuarioModificador)
                    .NotEmpty().WithMessage("Usuario Modificador es requerido")
                    .MaximumLength(GuiaSalidaBienDetalleConsts.UsuarioModificadorMaxLength)
                    .WithMessage($"La longitud del Usuario Modificador debe tener {GuiaSalidaBienDetalleConsts.UsuarioModificadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusUpdateResponse>
        {
            private readonly IGuiaSalidaBienRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(IGuiaSalidaBienRepository repository,
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

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    var guiaSalidaBien = await _repository.FindById(request.Id);

                    if (guiaSalidaBien == null || (request.Id != request.FormDto.GuiaSalidaBienId))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    var guiaSalidaBienForm = _mapper.Map<GuiaSalidaBienFormDto, GuiaSalidaBien>(request.FormDto);

                    guiaSalidaBien.Justificacion = guiaSalidaBienForm.Justificacion;
                    guiaSalidaBien.UsuarioModificador = guiaSalidaBienForm.UsuarioModificador;
                    guiaSalidaBien.FechaModificacion = DateTime.Now;

                    await _repository.Update(guiaSalidaBien);
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
