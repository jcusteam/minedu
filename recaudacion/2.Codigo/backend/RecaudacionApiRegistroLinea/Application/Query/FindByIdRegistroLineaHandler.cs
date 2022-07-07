using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiRegistroLinea.Application.Query.Dtos;
using RecaudacionApiRegistroLinea.DataAccess;
using RecaudacionApiRegistroLinea.Domain;
using MediatR;
using RecaudacionApiRegistroLinea.Clients;
using RecaudacionUtils;

namespace RecaudacionApiRegistroLinea.Application.Query
{
    public class FindByIdRegistroLineaHandler
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
            private readonly IRegistroLineaRepository _repository;
            private readonly IRegistroLineaDetalleRepository _detalleRepository;
            private readonly IMapper _mapper;
            private readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;

            public Handler(IRegistroLineaRepository repository,
            IRegistroLineaDetalleRepository detalleRepository,
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
                    var registroLinea = await _repository.FindById(request.Id);

                    if (registroLinea == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        var detalles = await _detalleRepository.FindAll(registroLinea.RegistroLineaId);

                        foreach (var item in detalles)
                        {
                            var clasificadorIngresoResponse = await _clasificadorIngresoAPI.FindByIdAsync(item.ClasificadorIngresoId);
                            if (clasificadorIngresoResponse.Success)
                            {
                                item.ClasificadorIngreso = clasificadorIngresoResponse.Data;
                            }
                        }

                        var registroLineaDto = _mapper.Map<RegistroLinea, RegistroLineaDto>(registroLinea);
                        registroLineaDto.RegistroLineaDetalle = _mapper.Map<List<RegistroLineaDetalleDto>>(detalles);
                        response.Data = registroLineaDto;
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
