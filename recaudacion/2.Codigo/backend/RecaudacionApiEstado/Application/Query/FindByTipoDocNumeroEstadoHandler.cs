using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiEstado.Application.Query.Dtos;
using RecaudacionApiEstado.DataAccess;
using RecaudacionApiEstado.Domain;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiEstado.Application.Query
{
    public class FindByTipoDocNumeroEstadoHandler
    {
        public class StatusFindTipoDocNumeroResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindTipoDocNumeroResponse>
        {
            public int TipoDocumentoId { get; set; }
            public int Numero { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindTipoDocNumeroResponse>
        {
            private readonly IEstadoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IEstadoRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindTipoDocNumeroResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindTipoDocNumeroResponse();

                try
                {
                    var estado = await _repository.FindByTipoDocAndNumero(request.TipoDocumentoId, request.Numero);

                    if (estado == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                        return response;
                    }

                    response.Data = _mapper.Map<Estado, EstadoDto>(estado);

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
