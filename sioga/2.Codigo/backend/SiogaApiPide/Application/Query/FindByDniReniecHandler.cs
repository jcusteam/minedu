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
    public class FindByDniReniecHandler
    {
        public class StatusReniecResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusReniecResponse>
        {
            public string Dni { get; set; }
            public string CodigoModulo { get; set; }
            public bool MenorEdad { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusReniecResponse>
        {
            private readonly IReniecService _reniecService;
            private readonly IMapper _mapper;

            public Handler(
                IReniecService reniecService,
                IMapper mapper)
            {
                _reniecService = reniecService;
                _mapper = mapper;
            }

            public async Task<StatusReniecResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusReniecResponse();
                try
                {

                    if (!Validators.IsDNI(request.Dni))
                    {
                        response.Success = false;
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "El número de DNI no es válido"));
                        return response;
                    }

                    var reniecResponse = await _reniecService.FindByDni(request.Dni, request.MenorEdad);

                    if (!reniecResponse.Success)
                    {
                        response.Messages = reniecResponse.Messages;
                        response.Success = false;
                        return response;
                    }

                    response.Data = _mapper.Map<Reniec, ReniecDto>(reniecResponse.Data);

                    if (request.CodigoModulo == Definition.MODULO_SUBVENCIONES)
                    {
                        response.Data = _mapper.Map<Reniec, ReniecSubvencionDto>(reniecResponse.Data);
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
