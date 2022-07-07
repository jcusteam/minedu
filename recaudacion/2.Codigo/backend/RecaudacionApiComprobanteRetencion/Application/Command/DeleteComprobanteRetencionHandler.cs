using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteRetencion.Application.Command
{
    public class DeleteComprobanteRetencionHandler
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
            private readonly IComprobanteRetencionRepository _repository;
            public Handler(IComprobanteRetencionRepository repository)
            {
                _repository = repository;
            }

            public async Task<StatusDeleteResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusDeleteResponse();
                try
                {
                    var comprobanteRetencion = await _repository.FindById(request.Id);

                    if (comprobanteRetencion == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    if (comprobanteRetencion.Estado != Definition.COMPROBANTE_RETENCION_ESTADO_EMITIDO)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_DELETE));
                        response.Success = false;
                        return response;
                    }

                    await _repository.Delete(comprobanteRetencion);
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
