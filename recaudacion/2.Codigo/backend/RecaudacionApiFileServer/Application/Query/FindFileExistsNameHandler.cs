using MediatR;
using Microsoft.Extensions.Logging;
using RecaudacionApiFileServer.Application.Query.Validation;
using RecaudacionApiFileServer.Domain;
using RecaudacionApiFileServer.Services.Contracts;
using RecaudacionUtils;
using System.Threading;
using System.Threading.Tasks;

namespace RecaudacionApiFileServer.Application.Query
{
    public class FindFileExistsNameHandler
    {
        public class StatusFindFileResponse : StatusResponse<FileContent>
        {
        }
        public class Query : IRequest<StatusFindFileResponse>
        {
            public string SubDirectory { get; set; }
            public string FileName { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindFileResponse>
        {
            private readonly IFileService _filService;
            private readonly ILogger<Handler> _logger;
            public Handler(IFileService filService, ILogger<Handler> logger)
            {
                _filService = filService;
                _logger = logger;
            }

            public async Task<StatusFindFileResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindFileResponse();

                try
                {
                    if (!ValidationQuery.Subdirectory(request.SubDirectory))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Sub directorio no es válido"));
                        response.Success = false;
                        return response;
                    }

                    if (!ValidationQuery.FileName(request.FileName))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Nombre de Archivo no es válido"));
                        response.Success = false;
                        return response;
                    }

                    var fileReponse = await _filService.FindByNameFile(request.SubDirectory, request.FileName);

                    if (!fileReponse.Success)
                    {
                        response.Messages = fileReponse.Messages;
                        response.Success = false;
                        return response;
                    }

                    response.Success = true;
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
