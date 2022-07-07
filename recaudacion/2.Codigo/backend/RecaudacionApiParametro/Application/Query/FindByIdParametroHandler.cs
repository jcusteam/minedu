using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiParametro.Application.Query.Dtos;
using RecaudacionApiParametro.DataAccess;
using RecaudacionApiParametro.Domain;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiParametro.Application.Query
{
    public class FindByIdParametroHandler
    {
        public class StatusFindResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindResponse>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindResponse>
        {
            private readonly IParametroRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IParametroRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var parametro = await _repository.FindById(request.Id);

                    if (parametro == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        response.Data = _mapper.Map<Parametro, ParametroDto>(parametro);
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
