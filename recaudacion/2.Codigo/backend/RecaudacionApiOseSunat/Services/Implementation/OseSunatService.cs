using Microsoft.Extensions.Logging;
using RecaudacionApiOseSunat.Services.Contracts;
using RecaudacionUtils;
using ServiceReference1;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using static RecaudacionApiOseSunat.Helpers.WsSecurity;

namespace RecaudacionApiOseSunat.Services.Implementation
{
    public class OseSunatService : IOseSunatService
    {
        public readonly EndpointAddress endpointAddress;
        public readonly BasicHttpBinding basicHttpBinding;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        //public readonly string serviceUrl = "https://oselab.todasmisfacturas.com.pe/ol-ti-itcpfegem/billservice?wsdl";

        public OseSunatService(ILogger<OseSunatService> logger, AppSettings appSettings)
        {
            _appSettings = appSettings;

            endpointAddress = new EndpointAddress(_appSettings.EndPoint);

            basicHttpBinding = new BasicHttpBinding(endpointAddress.Uri.Scheme.ToLower() == "http" ? BasicHttpSecurityMode.None : BasicHttpSecurityMode.Transport);

            //Please set the time accordingly, this is only for demo
            basicHttpBinding.OpenTimeout = TimeSpan.MaxValue;
            basicHttpBinding.CloseTimeout = TimeSpan.MaxValue;
            basicHttpBinding.ReceiveTimeout = TimeSpan.MaxValue;
            basicHttpBinding.SendTimeout = TimeSpan.MaxValue;
            this._logger = logger;
        }

        public async Task<billServiceClient> GetInstanceAsync()
        {
            return await Task.Run(() => new billServiceClient(basicHttpBinding, endpointAddress));
        }

        public async Task<StatusResponse<byte[]>> sendBill(string filename, byte[] content, string strUser, string strPass)
        {
            var response = new StatusResponse<byte[]>();
            try
            {
                _logger.LogInformation("Send ---- ");
                var client = await GetInstanceAsync();
                client.Endpoint.EndpointBehaviors.Add(new WsSecurityEndpointBehavior(strUser, strPass));
                var result = await client.sendBillAsync(filename, content);
                _logger.LogInformation("End ---- ");
                response.Data = result.applicationResponse;
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Error {e.Message}");
                response.Success = false;
                if (Validators.IsNumeric(e.Message))
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, $"Servicio OSE SUNAT '{MessageError.Sunat(e.Message)}'"));
                }
                else
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_OSE_SUNAT));
                }

            }


            return response;
        }


    }
}
