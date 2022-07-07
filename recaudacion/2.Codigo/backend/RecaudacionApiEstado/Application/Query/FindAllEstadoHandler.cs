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
    public class FindAllEstadoHandler
    {
        public class StatusFindAllResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IEstadoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IEstadoRepository repository, IMapper mapper)
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
                    response.Data = _mapper.Map<List<EstadoDto>>(items);
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
