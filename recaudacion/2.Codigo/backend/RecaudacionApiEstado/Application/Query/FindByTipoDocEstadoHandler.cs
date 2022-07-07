using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiEstado.Application.Query.Dtos;
using RecaudacionApiEstado.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiEstado.Application.Query
{
    public class FindByTipoDocEstadoHandler
    {
        public class StatusFindTipoDocResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindTipoDocResponse>
        {
            public int TipoDocumentoId { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindTipoDocResponse>
        {
            private readonly IEstadoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IEstadoRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindTipoDocResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindTipoDocResponse();

                try
                {
                    var estados = await _repository.FindByTipoDoc(request.TipoDocumentoId);
                    response.Data = _mapper.Map<List<EstadoDto>>(estados);
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
