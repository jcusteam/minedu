using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiDepositoBanco.Application.Query.Dtos;
using RecaudacionApiDepositoBanco.DataAccess;
using MediatR;
using RecaudacionUtils;
using RecaudacionApiDepositoBanco.Clients;
using Microsoft.Extensions.Logging;

namespace RecaudacionApiDepositoBanco.Application.Query
{
    public class FindAllDepositoBancoHandler
    {
        public class StatusFindAllResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IDepositoBancoRepository _repository;
            private readonly IBancoAPI _bancoAPI;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            public Handler(IDepositoBancoRepository repository, IBancoAPI bancoAPI, IMapper mapper,
                ILogger<Handler> logger)
            {
                _mapper = mapper;
                _bancoAPI = bancoAPI;
                _repository = repository;
                _logger = logger;
            }

            public async Task<StatusFindAllResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllResponse();

                try
                {
                    var items = await _repository.FindAll();
                    response.Data = _mapper.Map<List<DepositoBancoDto>>(items);
                }
                catch (System.Exception e)
                {
                    _logger.LogError($"Error {e.Message}");
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
