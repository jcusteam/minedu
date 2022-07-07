using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobanteEmisor.Application.Query.Dtos;
using RecaudacionApiComprobanteEmisor.DataAccess;
using RecaudacionApiComprobanteEmisor.Domain;
using MediatR;
using RecaudacionUtils;
using System;

namespace RecaudacionApiComprobanteEmisor.Application.Query
{
    public class FindByUnidadEjecutoraComprobanteEmisorHandler
    {
        public class StatusFindUnidadEjecutoraResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindUnidadEjecutoraResponse>
        {
            public int UnidadEjecutoraId { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindUnidadEjecutoraResponse>
        {
            private readonly IComprobanteEmisorRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IComprobanteEmisorRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindUnidadEjecutoraResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindUnidadEjecutoraResponse();

                try
                {
                    var comprobanteEmisor = await _repository.FindByUnidadEjecutoraId(request.UnidadEjecutoraId);

                    if (comprobanteEmisor == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        var cryptoAes = new CryptoAes();

                        if (!String.IsNullOrEmpty(comprobanteEmisor.ClaveOSE))
                        {
                            comprobanteEmisor.ClaveOSE = cryptoAes.Decrypt(comprobanteEmisor.ClaveOSE, comprobanteEmisor.NumeroRuc);
                        }

                        if (!String.IsNullOrEmpty(comprobanteEmisor.CorreoClave))
                        {
                            comprobanteEmisor.CorreoClave = cryptoAes.Decrypt(comprobanteEmisor.CorreoClave, comprobanteEmisor.NumeroRuc);

                        }
                        response.Data = _mapper.Map<ComprobanteEmisor, ComprobanteEmisorDto>(comprobanteEmisor);
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
