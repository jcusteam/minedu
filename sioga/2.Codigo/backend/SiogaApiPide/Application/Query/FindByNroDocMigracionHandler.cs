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
    public class FindByNroDocMigracionHandler
    {
        public class StatusMigracionResponse : StatusApiResponse<object>
        {

        }
        public class Query : IRequest<StatusMigracionResponse>
        {
            public string Numero { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusMigracionResponse>
        {
            private readonly IMigracionService _migracionService;
            private readonly IMapper _mapper;

            public Handler(IMigracionService migracionService, IMapper mapper)
            {
                _migracionService = migracionService;
                _mapper = mapper;
            }

            public async Task<StatusMigracionResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusMigracionResponse();

                try
                {

                    if (!Validators.IsCE(request.Numero))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "El número de Carné de extranjería no es válido"));
                        response.Success = false;
                        return response;
                    }

                    var responseMigracion = await _migracionService.FindByNroDoc(request.Numero);

                    if (!responseMigracion.Success)
                    {
                        response.Messages = responseMigracion.Messages;
                        response.Success = false;
                    }

                    response.Data = _mapper.Map<Migracion, MigracionDto>(responseMigracion.Data);

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
