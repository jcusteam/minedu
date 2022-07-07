using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobanteEmisor.DataAccess;
using MediatR;
using RecaudacionUtils;
using System;
using RecaudacionApiComprobanteEmisor.Application.Command.Request;
using RecaudacionApiComprobanteEmisor.Application.Command.Response;
using RecaudacionApiComprobanteEmisor.Application.Command.Validation;

namespace RecaudacionApiComprobanteEmisor.Application.Command
{
    public class UpdateEstadoComprobanteEmisorHandler : IRequestHandler<CommandUpdateEstado, StatusUpdateEstadoResponse>
    {
        private readonly IComprobanteEmisorRepository _repository;

        public UpdateEstadoComprobanteEmisorHandler(IComprobanteEmisorRepository repository)
        {
            _repository = repository;
        }
        public async Task<StatusUpdateEstadoResponse> Handle(CommandUpdateEstado request, CancellationToken cancellationToken)
        {
            var response = new StatusUpdateEstadoResponse();
            try
            {
                CommandUpdateEstadoValidator validations = new CommandUpdateEstadoValidator();
                var result = validations.Validate(request);

                if (!result.IsValid)
                {
                    foreach (var item in result.Errors)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                    }
                    response.Success = false;
                    return response;
                }

                var comprobanteEmisor = await _repository.FindById(request.Id);
                var comprobanteEmisorForm = request.FormDto;

                if (comprobanteEmisor == null || (request.Id != comprobanteEmisorForm.ComprobanteEmisorId))
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                    response.Success = false;
                    return response;
                }

                comprobanteEmisor.Estado = comprobanteEmisorForm.Estado;
                comprobanteEmisor.UsuarioModificador = comprobanteEmisorForm.UsuarioModificador;
                comprobanteEmisor.FechaModificacion = DateTime.Now;
                await _repository.Update(comprobanteEmisor);

                if (comprobanteEmisor.Estado)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE_ACTIVO));
                }
                else
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_UPDATE_INACTIVO));
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
