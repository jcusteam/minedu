using System;
using System.Collections.Generic;
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
    public class PageParametroHandler
    {
        public class PageParametro
        {
            public Object Items { get; set; }
            public int Total { get; set; }
        }

        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public ParametroFilterDto ParametroFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IParametroRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IParametroRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();
                var page = new PageParametro();

                try
                {
                    var filter = _mapper.Map<ParametroFilter>(request.ParametroFilterDto);
                    var items = await _repository.FindAll(filter);
                    var total = await _repository.Count(filter);
                    page.Items = _mapper.Map<List<ParametroDto>>(items);
                    page.Total = total;

                    response.Data = page;
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
