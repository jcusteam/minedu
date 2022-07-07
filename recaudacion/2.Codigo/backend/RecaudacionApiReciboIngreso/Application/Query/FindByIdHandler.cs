using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiReciboIngreso.Application.Query.Dtos;
using RecaudacionApiReciboIngreso.DataAccess;
using RecaudacionApiReciboIngreso.Domain;
using MediatR;
using RecaudacionApiReciboIngreso.Clients;
using RecaudacionUtils;

namespace RecaudacionApiReciboIngreso.Application.Query
{
    public class FindByIdHandler
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
            private readonly IReciboIngresoRepository _repository;
            private readonly IReciboIngresoDetalleRepository _detalleRepository;
            private readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;
            private readonly IMapper _mapper;

            public Handler(IReciboIngresoRepository repository,
                IReciboIngresoDetalleRepository detalleRepository,
                IClasificadorIngresoAPI clasificadorIngresoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _clasificadorIngresoAPI = clasificadorIngresoAPI;
                _mapper = mapper;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var reciboIngreso = await _repository.FindById(request.Id);

                    if (reciboIngreso == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        var detalles = await _detalleRepository.FindAll(reciboIngreso.ReciboIngresoId);
                        foreach (var item in detalles)
                        {
                            var clasificadorIngresoResponse = await _clasificadorIngresoAPI.FindByIdAsync(item.ClasificadorIngresoId);
                            if (clasificadorIngresoResponse.Success)
                            {
                                item.ClasificadorIngreso = clasificadorIngresoResponse.Data;
                            }
                        }

                        var reciboIngresoDto = _mapper.Map<ReciboIngreso, ReciboIngresoDto>(reciboIngreso);
                        reciboIngresoDto.ReciboIngresoDetalle = _mapper.Map<List<ReciboIngresoDetalleDto>>(detalles);
                        response.Data = reciboIngresoDto;
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
