
using Newtonsoft.Json;
using SiogaApiPide.Clients;
using SiogaApiPide.Clients.Request;
using SiogaApiPide.Clients.Response;
using SiogaApiPide.Domain;
using SiogaApiPide.Service.Contracts;
using SiogaUtils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiogaApiPide.Service.Implementation
{
    public class SunatService : ISunatService
    {
        private readonly ISunatAPI _sunatAPI;
        private readonly AppSettings _appSettings;

        public SunatService(ISunatAPI sunatAPI,
            AppSettings appSettings)
        {
            _sunatAPI = sunatAPI;
            _appSettings = appSettings;
        }

        public async Task<StatusApiResponse<Sunat>> FindByNroRuc(string numeroRuc)
        {
            var response = new StatusApiResponse<Sunat>();


            try
            {
                var loginResponse = await Login(_appSettings.Sunat.IdServicio);

                if (!loginResponse.Success)
                {
                    response.Messages = loginResponse.Messages;
                    response.Success = false;
                    return response;
                }

                var aes = new AES256();
                var sunatDto = new SunatRequest();
                var secretKey = _appSettings.Sunat.SecretKey;
                sunatDto.token = loginResponse.Data;
                sunatDto.id_sistema = _appSettings.Sunat.IdServicio;
                sunatDto.email_sistema = _appSettings.Sunat.EmailSistema;
                sunatDto.usuario_sistema = _appSettings.Sunat.UsuarioSistema;
                sunatDto.tipo_documento = _appSettings.Sunat.TipoDocumento;
                sunatDto.nro_documento = aes.Encrypt(numeroRuc, _appSettings.Sunat.SecretKey);
                sunatDto.id_servicio = _appSettings.Sunat.Empresa.IdServicio;
                sunatDto.ip_usuario_sistema = _appSettings.Sunat.IpUsuarioSistema;

                var sunatResponse = await _sunatAPI.FindByNroRuc(sunatDto);

                if (!sunatResponse.success)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, sunatResponse.message));
                    response.Success = false;
                    return response;
                }

                if (sunatResponse.code.Equals("0000"))
                {
                    var decriptSunatData = aes.Decrypt(sunatResponse.message, secretKey);
                    var sunatData = JsonConvert.DeserializeObject<Sunat>("{" + decriptSunatData.Substring(40, decriptSunatData.Length - 41).Replace("{}", "\"\""));

                    if (sunatData.ddp_numruc != null && sunatData.ddp_numruc.Length > 0)
                    {
                        if (!sunatData.esActivo)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número de RUC '{numeroRuc}' no esta activo"));
                            response.Success = false;
                            return response;
                        }

                        if (!sunatData.esHabido)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número de RUC '{numeroRuc}' no esta habido"));
                            response.Success = false;
                            return response;
                        }

                        //Representante
                        var sunatRepresentanteResponse = await FindRegresetanteByNroDoc(numeroRuc);

                        if (sunatRepresentanteResponse.Success)
                        {
                            sunatData.representante = sunatRepresentanteResponse.Data;
                        }

                        var ddp_refer1 = "";
                        var desc_tipzon = "";
                        var ddp_nomzon = "";

                        if (!String.IsNullOrEmpty(sunatData.ddp_refer1))
                        {
                            if (sunatData.ddp_refer1 != "-")
                                ddp_refer1 = $" ({sunatData.ddp_refer1}) ";
                        }

                        if (!String.IsNullOrEmpty(sunatData.desc_tipzon))
                        {
                            if (sunatData.desc_tipzon != "-")
                                desc_tipzon = sunatData.desc_tipzon;
                        }

                        if (!String.IsNullOrEmpty(sunatData.ddp_nomzon))
                        {
                            if (sunatData.ddp_nomzon != "-")
                                ddp_nomzon = sunatData.ddp_nomzon;
                        }

                        var desc_domi_fiscal = $"{sunatData.desc_tipvia} {sunatData.ddp_nomvia} NRO. {sunatData.ddp_numer1} {desc_tipzon} {ddp_nomzon} {ddp_refer1} {sunatData.desc_dep} - {sunatData.desc_prov} - {sunatData.desc_dist} ";
                        sunatData.desc_domi_fiscal = Regex.Replace(desc_domi_fiscal, @"\s+", " ");
                        sunatData.ddp_nombre = Regex.Replace(sunatData.ddp_nombre, @"\s+", " ");

                        foreach (var item in sunatData.representante)
                        {
                            item.desc_docide = Regex.Replace(item.desc_docide, @"\s+", " ");
                            item.rso_nombre = Regex.Replace(item.rso_nombre, @"\s+", " ");
                        }

                        response.Data = sunatData;
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"No se ha podido encontrar el número de RUC '{numeroRuc}' en el registro de la SUNAT"));
                        response.Success = false;
                    }
                }
                else
                {
                    if (Convert.ToInt32(sunatResponse.code) < 10)
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, CodigoErrores.MensajeSunat(sunatResponse.code)));

                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, sunatResponse.message));
                    }
                    response.Success = false;
                }
            }
            catch (System.Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_SUNAT));
                response.Success = false;
            }



            return response;
        }

        public async Task<StatusApiResponse<List<MultiRef>>> FindRegresetanteByNroDoc(string numeroRuc)
        {
            var response = new StatusApiResponse<List<MultiRef>>();

            try
            {
                var aes = new AES256();
                var id_servicio = _appSettings.Sunat.Representante.IdServicio;
                var secretKey = _appSettings.Sunat.SecretKey;
                var loginResponse = await Login(id_servicio);

                if (!loginResponse.Success)
                {
                    response.Messages = loginResponse.Messages;
                    response.Success = false;
                    return response;
                }

                var sunatDto = new SunatRequest();
                sunatDto.token = loginResponse.Data;
                sunatDto.id_sistema = _appSettings.Sunat.IdServicio;
                sunatDto.email_sistema = _appSettings.Sunat.EmailSistema;
                sunatDto.usuario_sistema = _appSettings.Sunat.UsuarioSistema;
                sunatDto.tipo_documento = _appSettings.Sunat.TipoDocumento;
                sunatDto.nro_documento = aes.Encrypt(numeroRuc, _appSettings.Sunat.SecretKey);
                sunatDto.id_servicio = id_servicio;
                sunatDto.ip_usuario_sistema = _appSettings.Sunat.IpUsuarioSistema;

                List<MultiRef> listaRepresentante = new List<MultiRef>();

                var responseRepresentante = await _sunatAPI.FindRegresetanteByNroDoc(sunatDto);

                if (!responseRepresentante.success)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, responseRepresentante.message));
                    response.Success = false;
                    return response;
                }

                if (responseRepresentante.code.Equals("0000"))
                {
                    var decriptSunatRepreData = aes.Decrypt(responseRepresentante.message, secretKey);

                    bool isList = false;

                    try
                    {
                        var sunatRepresentante = JsonConvert.DeserializeObject<RootRepresentanteLegalSunat>(decriptSunatRepreData.Replace("\"cod_cargo\":{}", "\"cod_cargo\":''"));
                        if (sunatRepresentante.multiRef != null)
                        {
                            listaRepresentante.Add(sunatRepresentante.multiRef);
                        }
                    }
                    catch
                    {
                        isList = true;
                    };

                    if (isList)
                    {
                        var sunatRepresentante = JsonConvert.DeserializeObject<ListaRepresentanteLegalSunat>(decriptSunatRepreData.Replace("\"cod_cargo\":{}", "\"cod_cargo\":''"));

                        listaRepresentante.AddRange(sunatRepresentante.multiRef);
                    }
                }
                else
                {
                    if (Convert.ToInt32(responseRepresentante.code) < 10)
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, CodigoErrores.MensajeSunat(responseRepresentante.code)));

                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, responseRepresentante.message));
                    }
                    response.Success = false;
                    return response;
                }

                response.Data = listaRepresentante;
            }
            catch (System.Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrío un error en el servicio de la SUNAT"));
                response.Success = false;
            }

            return response;
        }

        public async Task<StatusApiResponse<string>> Login(string idServicio)
        {
            var response = new StatusApiResponse<string>();

            try
            {
                // Login
                var loginDto = new LoginSunatRequest();
                loginDto.email = _appSettings.Sunat.Email;
                loginDto.password = _appSettings.Sunat.Password;
                loginDto.id_servicio = idServicio;
                var loginResponse = await _sunatAPI.Login(loginDto);

                if (String.IsNullOrEmpty(loginResponse.access_token))
                {
                    loginResponse.success = false;
                }

                if (!loginResponse.success)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, loginResponse.message));
                    response.Success = false;
                    return response;
                }

                response.Data = loginResponse.access_token;

            }
            catch (System.Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrío un error en el servicio de autenticación de la SUNAT"));
                response.Success = false;
            }

            return response;

        }
    }
}
