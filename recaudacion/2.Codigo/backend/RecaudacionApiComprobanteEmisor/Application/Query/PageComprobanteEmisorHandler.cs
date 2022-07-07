using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobanteEmisor.Application.Query.Dtos;
using RecaudacionApiComprobanteEmisor.DataAccess;
using MediatR;
using RecaudacionApiComprobanteEmisor.Domain;
using RecaudacionUtils;
using Microsoft.Extensions.Logging;

namespace RecaudacionApiComprobanteEmisor.Application.Query
{
    public class PageComprobanteEmisorHandler
    {
        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public ComprobanteEmisorFilterDto ComprobanteEmisorFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IComprobanteEmisorRepository _repository;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            public Handler(IComprobanteEmisorRepository repository,
                IMapper mapper,
                ILogger<Handler> logger)
            {
                _mapper = mapper;
                _repository = repository;
                _logger = logger;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<ComprobanteEmisorFilter>(request.ComprobanteEmisorFilterDto);
                    var pagination = await _repository.FindPage(filter);

                    foreach (var item in pagination.Items)
                    {
                        var cryptoAes = new CryptoAes();

                        if (!String.IsNullOrEmpty(item.ClaveOSE))
                        {
                            item.ClaveOSE = cryptoAes.Decrypt(item.ClaveOSE, item.NumeroRuc);
                        }

                        if (!String.IsNullOrEmpty(item.CorreoClave))
                        {
                            item.CorreoClave = cryptoAes.Decrypt(item.CorreoClave, item.NumeroRuc);
                        }
                    }

                    response.Data = _mapper.Map<Pagination<ComprobanteEmisorDto>>(pagination);
                    response.Success = true;
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Error " + e.Message);
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
