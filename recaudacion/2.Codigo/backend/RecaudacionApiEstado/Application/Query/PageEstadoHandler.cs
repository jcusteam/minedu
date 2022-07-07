using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiEstado.Application.Query.Dtos;
using RecaudacionApiEstado.DataAccess;
using MediatR;
using RecaudacionApiEstado.Domain;
using RecaudacionUtils;

namespace RecaudacionApiEstado.Application.Query
{
    public class PageEstadoHandler
    {
        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public EstadoFilterDto EstadoFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IEstadoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IEstadoRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<EstadoFilter>(request.EstadoFilterDto);
                    var pagination = await _repository.FindPage(filter);

                    response.Data = _mapper.Map<Pagination<EstadoDto>>(pagination);
                }
                catch (System.Exception e)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
