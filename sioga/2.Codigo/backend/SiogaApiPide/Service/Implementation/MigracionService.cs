
using Newtonsoft.Json;
using SiogaApiPide.Clients;
using SiogaApiPide.Clients.Request;
using SiogaApiPide.Domain;
using SiogaApiPide.Service.Contracts;
using SiogaUtils;
using System;
using System.Threading.Tasks;

namespace SiogaApiPide.Service.Implementation
{
    public class MigracionService : IMigracionService
    {
        private readonly IMigracionAPI _migracionAPI;
        private readonly AppSettings _appSettings;

        public MigracionService(
            IMigracionAPI migracionAPI,
            AppSettings appSettings)
        {
            _migracionAPI = migracionAPI;
            _appSettings = appSettings;
        }
        public async Task<StatusApiResponse<Migracion>> FindByNroDoc(string numero)
        {
            var response = new StatusApiResponse<Migracion>();

            try
            {
                var loginResponse = await Login();

                if (!loginResponse.Success)
                {
                    response.Messages = loginResponse.Messages;
                    response.Success = false;
                    return response;
                }

                var aes = new AES256();
                var secretKey = _appSettings.Migracion.SecretKey;

                var migracionDto = new MigracionRequest();
                migracionDto.token = loginResponse.Data;
                migracionDto.id_sistema = _appSettings.Migracion.IdSistema;
                migracionDto.email_sistema = _appSettings.Migracion.EmailSistema;
                migracionDto.usuario_sistema = _appSettings.Migracion.UsuarioSistema;
                migracionDto.tipo_documento = _appSettings.Migracion.TipoDocumento;
                migracionDto.nro_documento = aes.Encrypt(numero, secretKey);
                migracionDto.id_servicio = _appSettings.Migracion.IdServicio;
                migracionDto.ip_usuario_sistema = _appSettings.Migracion.IpUsuarioSistema;
                var migracionResponse = await _migracionAPI.FindByNroDoc(migracionDto);

                if (migracionResponse.code.Equals("0000"))
                {
                    var migracionData = aes.Decrypt(migracionResponse.message, secretKey);
                    var migracion = JsonConvert.DeserializeObject<Migracion>(migracionData.Substring(10, (migracionData.Length - 11)).Replace("{}", "\"\""));

                    var nombreCompleto = $"{migracion.strNombres} {migracion.strPrimerApellido}";

                    if (!String.IsNullOrEmpty(migracion.strSegundoApellido))
                    {
                        nombreCompleto = $"{nombreCompleto} {migracion.strSegundoApellido}";
                    }
                    migracion.strNombreCompleto = nombreCompleto;
                    response.Data = migracion;
                }
                else
                {
                    response.Success = false;
                    if (Convert.ToInt32(migracionResponse.code) < 10)
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, CodigoErrores.MensajeMigraciones(migracionResponse.code)));
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número de Carné de extranjería '{numero}' no es válido"));

                    }
                }
            }
            catch (System.Exception)
            {
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_MIGRACION));
                response.Success = false;
            }

            return response;
        }

        public async Task<StatusApiResponse<string>> Login()
        {
            var response = new StatusApiResponse<string>();

            try
            {
                // Login
                var loginDto = new LoginMigracionRequest();
                loginDto.email = _appSettings.Sunat.Email;
                loginDto.password = _appSettings.Sunat.Password;
                loginDto.id_servicio = _appSettings.Migracion.IdServicio;
                var loginResponse = await _migracionAPI.Login(loginDto);

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
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrío un error en el servicio de autenticación de Migraciones"));
                response.Success = false;
            }

            return response;
        }
    }
}
