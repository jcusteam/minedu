using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiTipoDocumento.Application.Command.Dtos;
using RecaudacionApiTipoDocumento.DataAccess;
using RecaudacionApiTipoDocumento.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiTipoDocumento.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiTipoDocumento.Application.Command
{
    public class AddTipoDocumentoHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddResponse>
        {
            public TipoDocumentoFormDto FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.FormDto.Nombre)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Nombre es requerido")
                    .MaximumLength(TipoDocumentoConsts.NombreMaxLength)
                    .WithMessage($"La longitud del Nombre debe tener {TipoDocumentoConsts.NombreMaxLength} caracteres o menos")
                     .Custom((x, context) =>
                     {
                         if (!String.IsNullOrEmpty(x))
                             if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                             {
                                 context.AddFailure("El Nombre contiene caracter no válido.");
                             }
                     });
                RuleFor(x => x.FormDto.Abreviatura)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Abreviatura es requerido")
                    .MaximumLength(TipoDocumentoConsts.NombreMaxLength)
                    .WithMessage($"La longitud de la Abreviatura debe tener {TipoDocumentoConsts.NombreMaxLength} caracteres o menos")
                     .Custom((x, context) =>
                     {
                         if (!String.IsNullOrEmpty(x))
                             if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#°().,_ -]", string.Empty).Length > 0)
                             {
                                 context.AddFailure("La Abreviatura contiene caracter no válido");
                             }
                     });
                RuleFor(x => x.FormDto.Estado).NotEmpty().WithMessage("Estado es requerido");
                RuleFor(x => x.FormDto.UsuarioCreador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Creador es requerido")
                    .MaximumLength(TipoDocumentoConsts.UsuarioCreadorMaxLength)
                    .WithMessage($"La longitud del Usuario Creador debe tener {TipoDocumentoConsts.UsuarioCreadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly ITipoDocumentoRepository _repository;
            private readonly IMapper _mapper;

            public Handler(ITipoDocumentoRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator();
                    var result = validations.Validate(request);

                    if (result.IsValid)
                    {
                        var tipoDocumento = _mapper.Map<TipoDocumentoFormDto, TipoDocumento>(request.FormDto);
                        tipoDocumento.FechaCreacion = DateTime.Now;

                        var exists = await _repository.VerifyExists(Definition.INSERT, tipoDocumento);

                        if (!exists)
                        {
                            tipoDocumento.TipoDocumentoId = await _repository.GenerateId();
                            tipoDocumento = await _repository.Add(tipoDocumento);

                            if (tipoDocumento != null)
                            {
                                response.Data = _mapper.Map<TipoDocumento, TipoDocumentoFormDto>(tipoDocumento); ;
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_INSERT));
                                response.Success = true;
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                                response.Success = false;
                            }
                        }
                        else
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                            response.Success = false;
                        }


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
