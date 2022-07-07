using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiTipoDocumento.Application.Query.Dtos;
using RecaudacionApiTipoDocumento.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiTipoDocumento.Application.Query
{
    public class FindAllTipoDocumentoHandler
    {
        public class StatusFindAllResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly ITipoDocumentoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(ITipoDocumentoRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindAllResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllResponse();

                try
                {
                    var items = await _repository.FindAll();
                    response.Data = _mapper.Map<List<TipoDocumentoDto>>(items);
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
