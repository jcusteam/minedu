using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiParametro.Application.Query.Dtos;
using RecaudacionApiParametro.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiParametro.Application.Query
{
    public class FindAllParametroHandler
    {
        public class StatusFindAllResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IParametroRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IParametroRepository repository, IMapper mapper)
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
                    response.Data = _mapper.Map<List<ParametroDto>>(items);
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
