using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiTipoDocumento.Application.Query.Dtos;
using RecaudacionApiTipoDocumento.DataAccess;
using RecaudacionApiTipoDocumento.Domain;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiTipoDocumento.Application.Query
{
    public class FindByIdTipoDocumentoHandler
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
            private readonly ITipoDocumentoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(ITipoDocumentoRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var tipoDocumento = await _repository.FindById(request.Id);

                    if (tipoDocumento == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        response.Data = _mapper.Map<TipoDocumento, TipoDocumentoDto>(tipoDocumento);
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
