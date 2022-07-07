using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SiogaApiPide.Application.Query.Dtos;
using SiogaApiPide.Domain;
using SiogaApiPide.Service.Contracts;
using SiogaUtils;

namespace SiogaApiPide.Application.Query
{
    public class FindByNroRucSunatHandler
    {
        public class StatusSunatResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusSunatResponse>
        {
            public string NumeroRuc { get; set; }
            public string CodigoModulo { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusSunatResponse>
        {
            private readonly ISunatService _sunatService;
            private readonly IMapper _mapper;

            public Handler(ISunatService sunatService,
                IMapper mapper)
            {
                _sunatService = sunatService;
                _mapper = mapper;
            }

            public async Task<StatusSunatResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusSunatResponse();

                try
                {

                    if (!Validators.IsRUC(request.NumeroRuc))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "El número de RUC no es válido"));
                        response.Success = false;
                        return response;
                    }

                    var responseSunat = await _sunatService.FindByNroRuc(request.NumeroRuc);

                    if (!responseSunat.Success)
                    {
                        response.Messages = responseSunat.Messages;
                        response.Success = false;
                        return response;
                    }

                    response.Data = response.Data = _mapper.Map<Sunat, SunatDto>(responseSunat.Data);

                    if (request.CodigoModulo == Definition.MODULO_SUBVENCIONES)
                    {
                        response.Data = _mapper.Map<Sunat, SunatSubvencionDto>(responseSunat.Data);
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
