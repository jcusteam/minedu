
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RecaudacionApiOseSunat.Domain;
using RecaudacionApiOseSunat.Helpers;
using RecaudacionApiOseSunat.Services.Contracts;
using RecaudacionUtils;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace RecaudacionApiOseSunat.Applicacion.Command
{
    public class SendComprobanteHandler
    {
        public class StatusSendResponse : StatusResponse<object>
        {

        }
        public class Command : IRequest<StatusSendResponse>
        {
            public Documento FormDto { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {

            }

        }


        public class Handler : IRequestHandler<Command, StatusSendResponse>
        {
            private readonly IOseSunatService _oseSunatService;
            private readonly AppSettings _appSettings;
            private readonly ILogger<Handler> _logger;

            public Handler(
                IOseSunatService oseSunatService,
                AppSettings appSettings,
                ILogger<Handler> logger)
            {
                _oseSunatService = oseSunatService;
                _appSettings = appSettings;
                _logger = logger;

            }
            public async Task<StatusSendResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusSendResponse();

                try
                {
                    var documento = request.FormDto;

                    string strEndpoint = _appSettings.EndPoint;
                    string strUsuario = documento.UsuarioOseEmisor;
                    string strPassword = documento.ClaveOseEmisor;
                    string strCert = documento.NombreCertEmisor;
                    string strkey = documento.NombreKeyEmisior;
                    string strFirmante = documento.FirmanteEmisor;
                    Resultado eresult = new Resultado();
                    string strTipoXML = "";
                    string envOSE = _appSettings.EnvOSE;
                    string sendOSE = _appSettings.SendOSE;


                    Boolean exito = true;
                    StringBuilder _xml = new StringBuilder();

                    switch (documento.CodigoTipoComprobante)
                    {
                        case "01":
                            _xml = new EstructuraXMLDocumento().ArmarXmlFacturaUBL21(documento, _appSettings);
                            strTipoXML = "Invoice";
                            break;
                        case "03":
                            _xml = new EstructuraXMLDocumento().ArmarXmlBoletaUBL21(documento, _appSettings);
                            strTipoXML = "Invoice";
                            break;
                        case "07":
                            _xml = new EstructuraXMLDocumento().ArmarXmlNotaCreditoUBL21(documento, _appSettings);
                            strTipoXML = "CreditNote";
                            break;
                        case "08":
                            _xml = new EstructuraXMLDocumento().ArmarXmlNotaDebitoUBL21(documento, _appSettings);
                            strTipoXML = "DebitNote";
                            break;
                    };

                    //Envio a Sunat
                    byte[] xmlsunat = new Funciones(_appSettings).GenerarXml(_xml);
                    //Guardar xml
                    String filenameXml = documento.RUCEmisor + "-" + documento.CodigoTipoComprobante + "-" + documento.NumeroComprobante.Split('-')[0] + "-" + documento.NumeroComprobante.Split('-')[1] + ".xml";
                    String filenameRptaXml = "R-" + documento.RUCEmisor + "-" + documento.CodigoTipoComprobante + "-" + documento.NumeroComprobante.Split('-')[0] + "-" + documento.NumeroComprobante.Split('-')[1] + ".xml";

                    String filenameZip = documento.RUCEmisor + "-" + documento.CodigoTipoComprobante + "-" + documento.NumeroComprobante.Split('-')[0] + "-" + documento.NumeroComprobante.Split('-')[1] + ".zip";
                    String filenameRptaZip = "R-" + documento.RUCEmisor + "-" + documento.CodigoTipoComprobante + "-" + documento.NumeroComprobante.Split('-')[0] + "-" + documento.NumeroComprobante.Split('-')[1] + ".zip";

                    String filenamePDF = documento.RUCEmisor + "-" + documento.CodigoTipoComprobante + "-" + documento.NumeroComprobante.Split('-')[0] + "-" + documento.NumeroComprobante.Split('-')[1] + ".pdf";
                    String filenamePDF_ = documento.RUCEmisor + "-" + documento.CodigoTipoComprobante + "-" + documento.NumeroComprobante.Split('-')[0] + "-" + documento.NumeroComprobante.Split('-')[1] + "_.pdf";

                    string path = _appSettings.RutaComprobante.Replace("\\", @"\\");

                    String tmpXml = path + "/" + filenameXml;

                    filenameZip = path + "/" + filenameZip;
                    filenameRptaZip = path + "/" + filenameRptaZip;
                    filenameRptaXml = path + "/" + filenameRptaXml;

                    //Guardar Xml
                    if (File.Exists(tmpXml))
                        File.Delete(tmpXml);

                    new Funciones(_appSettings).SaveData(xmlsunat, tmpXml);

                    //Convirtiendo a ANSI
                    //Reads UTF8 file
                    StreamReader fileStream = new StreamReader(tmpXml);
                    string fileContent = fileStream.ReadToEnd();
                    fileStream.Close();
                    // Borra archivo
                    if (File.Exists(tmpXml))
                        File.Delete(tmpXml);
                    // Now writes the content in ANSI
                    StreamWriter ansiWriter = new StreamWriter(tmpXml, false, Encoding.UTF8);
                    ansiWriter.Write(fileContent);
                    ansiWriter.Close();

                    //Firmar Xml
                    String SignatureValue = "";
                    String DigestValue = "";
                    new FirmaDigital(_appSettings).FirmarXml(tmpXml, strTipoXML, ref SignatureValue, ref DigestValue,
                        strCert, strkey, strFirmante);

                    documento.SignatureValue = SignatureValue;
                    documento.DigestValue = DigestValue;

                    //Convirtiendo a ANSI
                    //Reads UTF8 file
                    fileStream = new StreamReader(tmpXml);
                    fileContent = fileStream.ReadToEnd();
                    fileStream.Close();
                    // Borra archivo
                    if (File.Exists(tmpXml))
                        File.Delete(tmpXml);
                    // Now writes the content in ANSI
                    ansiWriter = new StreamWriter(tmpXml, false, Encoding.UTF8);
                    ansiWriter.Write(fileContent);
                    ansiWriter.Close();

                    //Comprimir Xml
                    if (File.Exists(filenameZip))
                        File.Delete(filenameZip);

                    Boolean comprimido = new Funciones(_appSettings).ComprimirArchivo(tmpXml, filenameZip);

                    var hourNow = DateTime.Now.Hour;
                    //var dayNow = DateTime.Now.Day;

                    if (envOSE == Definition.ENV_DEV)
                    {
                        if (hourNow < 8 || hourNow > 20)
                        {
                            sendOSE = Definition.SEND_OSE_NO;
                        }
                    }

                    if (sendOSE == Definition.SEND_OSE_SI)
                    {
                        // return eresult;
                        try
                        {
                            byte[] ZipSunat = File.ReadAllBytes(filenameZip);
                            ServicePointManager.Expect100Continue = false;
                            /* Envio SUNAT */
                            TimeSpan timeout = new TimeSpan(5, 30, 1);
                            //billServiceClient proxy = new billServiceClient(EndpointConfiguration.BillServicePort);
                            /* Comentado para pruebas */
                            //proxy.ConfigureEndpoint();
                            var responseOse = await _oseSunatService.sendBill(Path.GetFileName(filenameZip), ZipSunat, strUsuario, strPassword);

                            if (!responseOse.Success)
                            {
                                response.Messages = responseOse.Messages;
                                response.Success = false;
                                return response;
                            }

                            byte[] oSalida = responseOse.Data;

                            if (File.Exists(filenameRptaZip))
                                File.Delete(filenameRptaZip);

                            if (File.Exists(filenameRptaXml))
                                File.Delete(filenameRptaXml);

                            new Funciones(_appSettings).SaveData(oSalida, filenameRptaZip);

                            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(filenameRptaZip))
                            {
                                zip.ExtractAll(path);
                            }

                            ////Leer Xml Respuesta Sunat
                            XmlDocument xmlDocRptaSunat = new XmlDocument();
                            xmlDocRptaSunat.Load(filenameRptaXml);
                            XmlNodeList ar = null;
                            XmlNodeList dr = null;
                            XmlNodeList lista = null;
                            if (envOSE == Definition.ENV_DEV)
                            {
                                ar = xmlDocRptaSunat.GetElementsByTagName("ApplicationResponse");
                                dr = ((XmlElement)ar[0]).GetElementsByTagName("cac:DocumentResponse");
                                lista = ((XmlElement)dr[0]).GetElementsByTagName("cac:Response");
                            }
                            else
                            {
                                ar = xmlDocRptaSunat.GetElementsByTagName("ns4:ApplicationResponse");
                                dr = ((XmlElement)ar[0]).GetElementsByTagName("ns3:DocumentResponse");
                                lista = ((XmlElement)dr[0]).GetElementsByTagName("ns3:Response");
                            }
                            string responsecode = String.Empty, description = String.Empty;

                            foreach (XmlNode nodo in lista)
                            {
                                if (envOSE == Definition.ENV_DEV)
                                {
                                    responsecode = nodo["cbc:ResponseCode"].InnerText;
                                    description = nodo["cbc:Description"].InnerText;
                                }
                                else
                                {
                                    responsecode = nodo["ResponseCode"].InnerText;
                                    description = nodo["Description"].InnerText;
                                }
                            }
                            /* Fin comentarios por pruebas */
                            String EstadoComprobante = responsecode == "0" ? "ACEPTADO" : "RECHAZADO";


                            eresult.CDR = responsecode;
                            eresult.EstadoComprobante = EstadoComprobante;
                            eresult.MensajeResultadoSunat = description;

                        }
                        catch (Exception ex)
                        {
                            eresult.CDR = ex.Message.ToString();
                            eresult.EstadoComprobante = "RECHAZADO";
                            eresult.MensajeResultadoSunat = ex.Message.ToString();
                            exito = false;
                        }

                    }

                    if (exito)
                    {
                        filenamePDF_ = path + "/" + filenamePDF_;
                        filenamePDF = path + "/" + filenamePDF;

                        if (File.Exists(filenamePDF))
                            File.Delete(filenamePDF);

                        if (File.Exists(filenamePDF_))
                            File.Delete(filenamePDF_);

                        //Generar PDF
                        switch (documento.CodigoTipoComprobante)
                        {
                            case "01":
                                FabricaSingleton<FacturaPDF>.Crear().GenerarPDF(filenamePDF, "", documento, _appSettings);
                                break;
                            case "03":
                                FabricaSingleton<BoletaPDF>.Crear().GenerarPDF(filenamePDF, "", documento, _appSettings);
                                break;
                            case "07":
                                FabricaSingleton<NotaCreditoPDF>.Crear().GenerarPDF(filenamePDF, "", documento, _appSettings);
                                break;
                            case "08":
                                FabricaSingleton<NotaDebitoPDF>.Crear().GenerarPDF(filenamePDF, "", documento, _appSettings);
                                break;
                        };

                        if (File.Exists(filenamePDF_))
                            File.Delete(filenamePDF_);

                        eresult.CDR = "02";
                        eresult.EstadoComprobante = "ACEPTADO";
                        eresult.NombrePDF = filenamePDF;
                        eresult.NombreXML = filenameRptaXml;
                        eresult.NombreZip = filenameRptaZip;
                        documento.sEstado = "V";

                        response.Data = eresult;
                    }

                }
                catch (System.Exception e)
                {
                    _logger.LogError($"Error {e.Message}");
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
