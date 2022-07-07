using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiComprobanteEmisor.Application.Command.Dtos;
using RecaudacionApiComprobanteEmisor.DataAccess;
using RecaudacionApiComprobanteEmisor.Domain;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiComprobanteEmisor.Clients;
using RecaudacionApiComprobanteEmisor.Application.Command.Request;
using RecaudacionApiComprobanteEmisor.Application.Command.Response;
using RecaudacionApiComprobanteEmisor.Application.Command.Validation;

namespace RecaudacionApiComprobanteEmisor.Application.Command
{
    public class AddComprobanteEmisorHandler : IRequestHandler<CommandAdd, StatusAddResponse>
    {

        private readonly IComprobanteEmisorRepository _repository;
        private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
        private readonly IPideAPI _pideAPI;
        private readonly IMapper _mapper;

        public AddComprobanteEmisorHandler(IComprobanteEmisorRepository repository,
            IUnidadEjecutoraAPI unidadEjecutoraAPI,
             IPideAPI pideAPI,
            IMapper mapper)
        {
            _repository = repository;
            _unidadEjecutoraAPI = unidadEjecutoraAPI;
            _pideAPI = pideAPI;
            _mapper = mapper;
        }

        public async Task<StatusAddResponse> Handle(CommandAdd request, CancellationToken cancellationToken)
        {

            var response = new StatusAddResponse();
            try
            {
                CommandAddValidator validations = new CommandAddValidator(_unidadEjecutoraAPI);
                var result = await validations.ValidateAsync(request);

                if (result.IsValid)
                {
                    var comprobanteEmisor = _mapper.Map<ComprobanteEmisorFormDto, ComprobanteEmisor>(request.FormDto);
                    comprobanteEmisor.Estado = true;
                    comprobanteEmisor.FechaCreacion = DateTime.Now;

                    var unidadEjecutoraResponse = await _unidadEjecutoraAPI.FindByIdAsync(comprobanteEmisor.UnidadEjecutoraId);

                    var sunatResponse = await _pideAPI.FindSunatByRucAsync(unidadEjecutoraResponse.Data.NumeroRuc);
                    if (sunatResponse.Success)
                    {
                        var sunat = sunatResponse.Data;
                        comprobanteEmisor.NumeroRuc = unidadEjecutoraResponse.Data.NumeroRuc;
                        comprobanteEmisor.TipoDocumento = Definition.TIPO_DOCUMENTO_SUNAT_RUC;
                        comprobanteEmisor.RazonSocial = sunat.ddp_nombre.Trim();
                        comprobanteEmisor.NombreComercial = sunat.ddp_nombre.Trim();
                        comprobanteEmisor.Ubigeo = sunat.ddp_ubigeo.Trim();
                        comprobanteEmisor.Direccion = sunat.desc_domi_fiscal;
                        comprobanteEmisor.Urbanizacion = sunat.ddp_nomzon;
                        comprobanteEmisor.Departamento = sunat.desc_dep;
                        comprobanteEmisor.Provincia = sunat.desc_prov;
                        comprobanteEmisor.Distrito = sunat.desc_dist;
                        comprobanteEmisor.CodigoPais = Definition.SUNAT_CODIGO_PAIS;
                    }
                    else
                    {
                        response.Messages = sunatResponse.Messages;
                        response.Success = false;
                        return response;
                    }

                    var exists = await _repository.VerifyExists(Definition.INSERT, comprobanteEmisor);
                    if (!exists)
                    {
                        var cryptoAes = new CryptoAes();

                        if (!String.IsNullOrEmpty(comprobanteEmisor.ClaveOSE))
                        {
                            comprobanteEmisor.ClaveOSE = cryptoAes.Encrypt(comprobanteEmisor.ClaveOSE, comprobanteEmisor.NumeroRuc);
                        }

                        if (!String.IsNullOrEmpty(comprobanteEmisor.CorreoClave))
                        {
                            comprobanteEmisor.CorreoClave = cryptoAes.Encrypt(comprobanteEmisor.CorreoClave, comprobanteEmisor.NumeroRuc);
                        }

                        comprobanteEmisor.ComprobanteEmisorId = await _repository.GenerateId();
                        comprobanteEmisor = await _repository.Add(comprobanteEmisor);

                        if (comprobanteEmisor != null)
                        {
                            response.Data = _mapper.Map<ComprobanteEmisor, ComprobanteEmisorFormDto>(comprobanteEmisor);
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_INSERT));
                            response.Success = true;
                        }
                        else
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                            response.Success = false;
                        }
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_EXISTS_DATA));
                        response.Success = false;
                    }


                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                    }
                    response.Success = false;
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
