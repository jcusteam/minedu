using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiIngresoPecosa.Application.Query.Dtos;
using RecaudacionApiIngresoPecosa.DataAccess;
using RecaudacionApiIngresoPecosa.Domain;
using MediatR;
using RecaudacionApiIngresoPecosa.Clients;
using RecaudacionUtils;

namespace RecaudacionApiIngresoPecosa.Application.Query
{
    public class FindByIdIngresoPecosaDetalleHandler
    {
        public class StatusFindDetalleResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindDetalleResponse>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindDetalleResponse>
        {
            private readonly IIngresoPecosaDetalleRepository _detalleRepository;
            private readonly IMapper _mapper;

            public Handler(IIngresoPecosaDetalleRepository detalleRepository,
                IMapper mapper)
            {
                _mapper = mapper;
                _detalleRepository = detalleRepository;
            }

            public async Task<StatusFindDetalleResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindDetalleResponse();

                try
                {
                    var ingresoPecosaDetalle = await _detalleRepository.FindById(request.Id);

                    if (ingresoPecosaDetalle == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                        return response;
                    }

                    response.Data = _mapper.Map<IngresoPecosaDetalle, IngresoPecosaDetalleDto>(ingresoPecosaDetalle);
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
