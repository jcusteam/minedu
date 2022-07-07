using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiEstado.Application.Query.Dtos;
using RecaudacionApiEstado.DataAccess;
using RecaudacionApiEstado.Domain;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiEstado.Application.Query
{
    public class FindByIdEstadoHandler
    {
        public class StatusFindResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindResponse>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindResponse>
        {
            private readonly IEstadoRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IEstadoRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var estado = await _repository.FindById(request.Id);

                    if (estado == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        response.Data = _mapper.Map<Estado, EstadoDto>(estado);
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
