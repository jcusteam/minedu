using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiIngresoPecosa.Application.Query.Dtos;
using RecaudacionApiIngresoPecosa.DataAccess;
using MediatR;
using RecaudacionApiIngresoPecosa.Clients;
using RecaudacionUtils;

namespace RecaudacionApiIngresoPecosa.Application.Query
{
    public class FindByCatalogoIngresoPecosaDetalleHandler
    {
        public class StatusSaldoResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusSaldoResponse>
        {
            public int CatalogoBienId { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusSaldoResponse>
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

            public async Task<StatusSaldoResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusSaldoResponse();

                try
                {
                    var items = await _detalleRepository.FindByCatalogoBienSaldo(request.CatalogoBienId);


                    if (items.Count == 0)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        foreach (var item in items)
                        {
                            var ingresoPecosa = await _repository.FindById(item.IngresoPecosaId);
                            item.AnioPecosa = ingresoPecosa.AnioPecosa;
                            item.NumeroPecosa = ingresoPecosa.NumeroPecosa;
                            item.Saldo = item.Cantidad - item.CantidadSalida;
                        }

                        response.Data = _mapper.Map<List<IngresoPecosaDetalleDto>>(items);
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
