using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobanteEmisor.Application.Query.Dtos;
using RecaudacionApiComprobanteEmisor.DataAccess;
using MediatR;
using RecaudacionUtils;
using System;

namespace RecaudacionApiComprobanteEmisor.Application.Query
{
    public class FindAllComprobanteEmisorHandler
    {
        public class StatusFindAllResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IComprobanteEmisorRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IComprobanteEmisorRepository repository, IMapper mapper)
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

                    foreach (var item in items)
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

                    response.Data = _mapper.Map<List<ComprobanteEmisorDto>>(items);
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
