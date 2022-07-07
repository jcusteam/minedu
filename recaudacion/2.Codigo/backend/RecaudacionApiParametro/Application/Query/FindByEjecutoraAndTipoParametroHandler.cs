using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiParametro.Application.Query.Dtos;
using RecaudacionApiParametro.DataAccess;
using MediatR;
using RecaudacionApiParametro.Domain;
using RecaudacionUtils;

namespace RecaudacionApiParametro.Application.Query
{
    public class FindByEjecutoraAndTipoParametroHandler
    {
        public class StatusFindEjecutoraResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindEjecutoraResponse>
        {
            public int UnidadEjecutoraId { get; set; }
            public int TipoDocumentoId { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindEjecutoraResponse>
        {
            private readonly IParametroRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IParametroRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindEjecutoraResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindEjecutoraResponse();

                try
                {
                    var parametro = await _repository.FindByEjecutoraAndTipo(request.UnidadEjecutoraId, request.TipoDocumentoId);

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
