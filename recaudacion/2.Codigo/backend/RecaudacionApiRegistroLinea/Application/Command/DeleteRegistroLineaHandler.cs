using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiRegistroLinea.Application.Command
{
    public class DeleteRegistroLineaHandler
    {
        public class StatusDeleteResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusDeleteResponse>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, StatusDeleteResponse>
        {
            private readonly IRegistroLineaRepository _repository;
            public Handler(IRegistroLineaRepository repository)
            {
                _repository = repository;
            }

            public async Task<StatusDeleteResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusDeleteResponse();
                try
                {
                    var registroLinea = await _repository.FindById(request.Id);
                    if (registroLinea == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    if (registroLinea.Estado != Definition.REGISTRO_LINEA_ESTADO_EMITIDO)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_DELETE));
                        response.Success = false;
                        return response;
                    }

                    await _repository.Delete(registroLinea);
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_DELETE));

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
