using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobanteEmisor.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteEmisor.Application.Command
{
    public class DeleteComprobanteEmisorHandler
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
            private readonly IComprobanteEmisorRepository _repository;
            public Handler(IComprobanteEmisorRepository repository)
            {
                _repository = repository;
            }

            public async Task<StatusDeleteResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusDeleteResponse();
                try
                {
                    var comprobanteEmisor = await _repository.FindById(request.Id);
                    if (comprobanteEmisor == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                    }
                    else
                    {
                        await _repository.Delete(comprobanteEmisor);
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_DELETE));
                        response.Success = true;
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
