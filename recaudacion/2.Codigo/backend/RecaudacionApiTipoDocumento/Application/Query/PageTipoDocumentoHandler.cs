using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiTipoDocumento.Application.Query.Dtos;
using RecaudacionApiTipoDocumento.DataAccess;
using MediatR;
using RecaudacionApiTipoDocumento.Domain;
using RecaudacionUtils;

namespace RecaudacionApiTipoDocumento.Application.Query
{
    public class PageTipoDocumentoHandler
    {

        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public TipoDocumentoFilterDto TipoDocumentoFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly ITipoDocumentoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(ITipoDocumentoRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<TipoDocumentoFilter>(request.TipoDocumentoFilterDto);
                    var pagiantion = await _repository.FindPage(filter);
                    response.Data = _mapper.Map<Pagination<TipoDocumentoDto>>(pagiantion);

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
