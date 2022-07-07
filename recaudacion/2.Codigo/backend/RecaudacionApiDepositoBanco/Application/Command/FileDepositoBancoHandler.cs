using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RecaudacionApiDepositoBanco.Clients;
using RecaudacionUtils;
using RecaudacionApiDepositoBanco.Application.Command.Dtos;
using FluentValidation;
using RecaudacionApiDepositoBanco.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiDepositoBanco.Application.Command
{
    public class FileDepositoBancoHandler
    {
        public class StatusFileResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusFileResponse>
        {
            public DepositoBancoFileDto FileDto { get; set; }
        }

        public class CommandFileValidator : AbstractValidator<Command>
        {
            private readonly ITipoDocIdentidadAPI _tipoDocIdentidadAPI;
            public CommandFileValidator(ITipoDocIdentidadAPI tipoDocIdentidadAPI)
            {
                _tipoDocIdentidadAPI = tipoDocIdentidadAPI;

                RuleFor(x => x.FileDto.Detalles)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("Detalle del depósito de banco es requerido")
                   .Custom((x, context) =>
                   {
                       if (x.Count == 0)
                       {
                           context.AddFailure($"Detalle del depósito de banco es requerido");
                       }
                   })
                   .ForEach(detalles =>
                   {
                       detalles.ChildRules(detalle =>
                       {
                           detalle.RuleFor(x => x.Cliente).NotNull().WithMessage("Cliente es requerido");

                           detalle.RuleFor(x => x.Cliente.TipoDocumentoIdentidadId)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Cliente: Id Tipo Documento Identidad es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 1)
                                    {
                                        context.AddFailure($"Cliente: Id Tipo Documento Identidad no debe ser {x}");
                                    }
                                })
                                .MustAsync(async (id, cancellation) =>
                                {
                                    var response = await _tipoDocIdentidadAPI.FindByIdAsync(id);
                                    bool exists = response.Success;
                                    return exists;
                                }).WithMessage("Cliente: Id Tipo Documento Identidad no existe");

                           detalle.RuleFor(x => x.Cliente.NumeroDocumento)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Cliente: Número Documento es requerido")
                                .MaximumLength(DepositoBancoDetalleConsts.NumeroDocumentoClienteMaxLength)
                                .WithMessage($"Cliente: La longitud del Número Documento debe tener {DepositoBancoDetalleConsts.NumeroDocumentoClienteMaxLength} caracteres o menos")
                                .Custom((x, context) =>
                                {
                                    if (!string.IsNullOrEmpty(x))
                                        if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                                        {
                                            context.AddFailure("Cliente: El Número de documento contiene caracter no válido");
                                        }
                                });

                       });
                   });
            }
        }

        public class Handler : IRequestHandler<Command, StatusFileResponse>
        {
            private readonly IPideAPI _pideAPI;
            private readonly ITipoDocIdentidadAPI _tipoDocIdentidadAPI;
            private readonly IClienteAPI _clienteAPI;
            public Handler(IPideAPI pideAPI,
                ITipoDocIdentidadAPI tipoDocIdentidadAPI,
                IClienteAPI clienteAPI)
            {
                _pideAPI = pideAPI;
                _tipoDocIdentidadAPI = tipoDocIdentidadAPI;
                _clienteAPI = clienteAPI;

            }

            public async Task<StatusFileResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusFileResponse();

                try
                {
                    CommandFileValidator validations = new CommandFileValidator(_tipoDocIdentidadAPI);
                    var result = await validations.ValidateAsync(request);

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                            response.Success = false;
                            return response;
                        }

                    }

                    var detalles = request.FileDto.Detalles;

                    foreach (var item in detalles)
                    {

                        item.Cliente.UsuarioCreador = request.FileDto.UsuarioCreador;
                        var clienteResponse = await _clienteAPI.FindByTipoNroDocAsync(item.Cliente.TipoDocumentoIdentidadId, item.Cliente.NumeroDocumento);

                        if (clienteResponse.Success)
                        {
                            item.ClienteId = clienteResponse.Data.ClienteId;
                            item.Cliente = clienteResponse.Data;
                        }
                        else
                        {
                            switch (item.Cliente.TipoDocumentoIdentidadId)
                            {
                                case Definition.TIPO_DOCUMENTO_IDENTIDAD_DNI:
                                    var reniecResponse = await _pideAPI.FindReniecByDniAsync(item.Cliente.NumeroDocumento);
                                    if (!reniecResponse.Success)
                                    {
                                        response.Messages.Add(new GenericMessage(reniecResponse.Messages[0].Type, $"Servicio de Reniec: {reniecResponse.Messages[0].Message}"));
                                        response.Success = false;
                                        return response;
                                    }

                                    item.Cliente.Nombre = reniecResponse.Data.nombreCompleto;
                                    item.Cliente.Direccion = reniecResponse.Data.domicilioApp;
                                    item.Cliente.Estado = true;
                                    break;

                                case Definition.TIPO_DOCUMENTO_IDENTIDAD_CE:
                                    var migracionResponse = await _pideAPI.FindMigracionByNumeroAsync(item.Cliente.NumeroDocumento);
                                    if (!migracionResponse.Success)
                                    {
                                        response.Messages.Add(new GenericMessage(migracionResponse.Messages[0].Type, $"Servicio de Migraciones: {migracionResponse.Messages[0].Message}"));
                                        response.Success = false;
                                        return response;
                                    }

                                    item.Cliente.Nombre = migracionResponse.Data.strNombreCompleto;
                                    item.Cliente.Direccion = "-";
                                    item.Cliente.Estado = true;
                                    break;

                                case Definition.TIPO_DOCUMENTO_IDENTIDAD_RUC:
                                    var sunatResponse = await _pideAPI.FindSunatByRucAsync(item.Cliente.NumeroDocumento);
                                    if (sunatResponse.Success)
                                    {
                                        response.Messages.Add(new GenericMessage(sunatResponse.Messages[0].Type, $"Servicio SUNAT: {sunatResponse.Messages[0].Message}"));
                                        response.Success = false;
                                        return response;
                                    }

                                    item.Cliente.Nombre = sunatResponse.Data.ddp_nombre;
                                    item.Cliente.Direccion = sunatResponse.Data.desc_domi_fiscal;
                                    item.Cliente.Estado = true;
                                    break;

                                default:
                                    break;
                            }

                            var clienteAddReponse = await _clienteAPI.AddAsync(item.Cliente);
                            if (clienteAddReponse.Success)
                            {
                                item.ClienteId = clienteAddReponse.Data.ClienteId;
                                item.Cliente = clienteAddReponse.Data;
                            }



                        }

                        var tipoDocResponse = await _tipoDocIdentidadAPI.FindByIdAsync(item.Cliente.TipoDocumentoIdentidadId);
                        if (tipoDocResponse.Success)
                        {
                            item.Cliente.TipoDocumentoNombre = tipoDocResponse.Data.Nombre;
                        }
                    }

                    response.Data = detalles;
                }
                catch (System.Exception e)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
