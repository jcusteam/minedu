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
    public class FindByIdIngresoPecosaHandler
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
            private readonly IIngresoPecosaRepository _repository;
            private readonly IIngresoPecosaDetalleRepository _detalleRepository;
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly IMapper _mapper;

            public Handler(IIngresoPecosaRepository repository,
                 IIngresoPecosaDetalleRepository detalleRepository,
                 ICatalogoBienAPI catalogoBienAPI,
                IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
                _detalleRepository = detalleRepository;
                _catalogoBienAPI = catalogoBienAPI;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var ingresoPecosa = await _repository.FindById(request.Id);

                    if (ingresoPecosa == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        var detalle = await _detalleRepository.FindAll(ingresoPecosa.IngresoPecosaId);
                        var ingresoPecosaDto = _mapper.Map<IngresoPecosa, IngresoPecosaDto>(ingresoPecosa);
                        ingresoPecosaDto.IngresoPecosaDetalle = _mapper.Map<List<IngresoPecosaDetalleDto>>(detalle);
                        response.Data = ingresoPecosaDto;
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
