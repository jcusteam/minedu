using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using RecaudacionApiFileServer.Services.Contracts;
using RecaudacionUtils;
using RecaudacionApiFileServer.Application.Command.Validation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RecaudacionApiFileServer.Application.Command
{
    public class AddFileHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {

        }

        public class Command : IRequest<StatusAddResponse>
        {
            public string SubDirectory { get; set; }
            public IFormFile File { get; set; }
        }

        public class FileValidatorFile : AbstractValidator<IFormFile>
        {
            public FileValidatorFile()
            {
                RuleFor(x => x.Length).NotNull().LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("Tamaño máximo: 5 MB");
                RuleFor(x => x.ContentType).NotNull().Must(x => Tools.GetMimeTypes().ContainsValue(x)).WithMessage("Extesiones permitidas: TXT, DOC, DOCX, PDF, JPEG, JPG ó PNG");
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.SubDirectory).NotEmpty().WithMessage("Sub directorio es requerido")
                .MaximumLength(50).WithMessage("Sub directorio no debe contener mas de 50 caracteres")
                .Custom((x, context) =>
                {
                    if (Regex.Replace(x, @"[A-Za-z0-9_-]", string.Empty).Length > 0)
                    {
                        context.AddFailure("Sub Directorio contiene caracter no válido");
                    }
                }); ;
                RuleFor(x => x.File).SetValidator(new FileValidatorFile());
            }
        }

        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IFileService _filService;
            private readonly ILogger<Handler> _logger;
            public Handler(IFileService filService, ILogger<Handler> logger)
            {
                _filService = filService;
                _logger = logger;
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
                        var fileResponse = await _filService.SaveFile(request.File, request.SubDirectory);
                        if (!fileResponse.Success)
                        {
                            response.Messages = fileResponse.Messages;
                            response.Success = false;
                        }
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPLOAD));
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
                catch (System.Exception e)
                {
                    _logger.LogError($"Error {e.Message}");
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
