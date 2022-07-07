using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobantePago.Application.Query.Dtos;
using RecaudacionApiComprobantePago.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.Application.Query
{
    public class FindAllComprobantePagoHandler
    {
        public class StatusFindAllResponse : StatusResponse<List<ComprobantePagoDto>>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IComprobantePagoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IComprobantePagoRepository repository, IMapper mapper)
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
                    response.Data = _mapper.Map<List<ComprobantePagoDto>>(items);
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
