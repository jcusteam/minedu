using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiPedidoPecosa.Application.Query.Dtos;
using RecaudacionApiPedidoPecosa.DataAccess;
using MediatR;
using RecaudacionUtils;
using System.Text.RegularExpressions;
using System;
using RecaudacionApiIngresoPecosa.Helpers;

namespace RecaudacionApiPedidoPecosa.Application.Query
{
    public class FindAllPedidoPecosaHandler
    {
        public class StatusFindAllResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
            public ConsultaDto ConsultaDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IPedidoPecosaRepository _repository;
            public Handler(IPedidoPecosaRepository repository)
            {
                _repository = repository;
            }

            public async Task<StatusFindAllResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllResponse();

                try
                {
                    var consulta = request.ConsultaDto;
                    if (Regex.Replace(consulta.Ejecutora, @"[0-9]", string.Empty).Length > 0)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, "La secuencia de la Unidad Ejecutora debe ser número"));
                        response.Success = false;
                        return response;
                    }

                    if (consulta.NumeroPecosa <= 0)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, $"El Número de Pecosa no pude ser {consulta.NumeroPecosa}"));
                        response.Success = false;
                        return response;
                    }

                    var year = DateTime.Now.Year;

                    if (consulta.AnioEje < PedidoPecosaConsts.AnioPecosaMin || consulta.AnioEje > year)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, $"El Año de Pecosa debe ser entre {PedidoPecosaConsts.AnioPecosaMin} al {year} "));
                        response.Success = false;
                        return response;
                    }

                    var items = await _repository.FindAll(consulta.Ejecutora, consulta.AnioEje, consulta.NumeroPecosa);

                    if (items.Count == 0)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.PEDIDO_PECOSA_INFO_NOT_EXISTS_DATA_DOCUMENTO_FUENTE + " " + consulta.NumeroPecosa));
                        response.Success = false;
                        return response;

                    }

                    response.Data = items;

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
