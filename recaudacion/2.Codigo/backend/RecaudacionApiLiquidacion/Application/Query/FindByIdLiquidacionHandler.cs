using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiLiquidacion.Application.Query.Dtos;
using RecaudacionApiLiquidacion.DataAccess;
using RecaudacionApiLiquidacion.Domain;
using MediatR;
using RecaudacionApiLiquidacion.Clients;
using RecaudacionUtils;

namespace RecaudacionApiLiquidacion.Application.Query
{
    public class FindByIdLiquidacionHandler
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
            private readonly ILiquidacionRepository _repository;
            public readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;
            public readonly ITipoCaptacionAPI _tipoCaptacionAPI;
            private readonly IMapper _mapper;
            public Handler(ILiquidacionRepository repository,
                IClasificadorIngresoAPI clasificadorIngresoAPI,
                ITipoCaptacionAPI tipoCaptacionAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _clasificadorIngresoAPI = clasificadorIngresoAPI;
                _tipoCaptacionAPI = tipoCaptacionAPI;
                _repository = repository;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var liquidacion = await _repository.FindById(request.Id);

                    if (liquidacion == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                        return response;
                    }

                    var detalles = await _repository.FindDetalleById(liquidacion.LiquidacionId);

                    foreach (var item in detalles)
                    {
                        var clasificadorResponse = await _clasificadorIngresoAPI.FindByIdAsync(item.ClasificadorIngresoId);

                        if (clasificadorResponse.Success)
                        {
                            item.ClasificadorIngreso = clasificadorResponse.Data;
                        }

                        var tipoCaptacionResponse = await _tipoCaptacionAPI.FindByIdAsync(item.TipoCaptacionId);

                        if (tipoCaptacionResponse.Success)
                        {
                            item.TipoCaptacion = tipoCaptacionResponse.Data;
                        }
                    }

                    var liquidacionDto = _mapper.Map<Liquidacion, LiquidacionDto>(liquidacion);
                    liquidacionDto.LiquidacionDetalle = _mapper.Map<List<LiquidacionDetalleDto>>(detalles);

                    response.Data = liquidacionDto;

                }
                catch (System.Exception e)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
