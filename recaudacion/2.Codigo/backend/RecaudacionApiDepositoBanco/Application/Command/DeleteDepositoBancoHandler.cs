using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiDepositoBanco.Application.Command
{
    public class DeleteDepositoBancoHandler
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
            private readonly IDepositoBancoRepository _repository;
            public Handler(IDepositoBancoRepository repository)
            {
                _repository = repository;
            }

            public async Task<StatusDeleteResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusDeleteResponse();
                try
                {
                    var depositoBanco = await _repository.FindById(request.Id);

                    if (depositoBanco == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    if (depositoBanco.Estado != Definition.DEPOSITO_BANCO_ESTADO_EMITIDO)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.WARNING_DELETE));
                        response.Success = false;
                        return response;
                    }

                    await _repository.Delete(depositoBanco);
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_DELETE));
                    response.Success = true;

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
