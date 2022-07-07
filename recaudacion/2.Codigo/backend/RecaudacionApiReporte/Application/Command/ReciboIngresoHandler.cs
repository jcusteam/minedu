using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocXToPdfConverter;
using MediatR;
using Microsoft.Extensions.Logging;
using RecaudacionApiReporte.Application.Command.Dtos;
using RecaudacionApiReporte.Domain;
using RecaudacionUtils;

namespace RecaudacionApiReporte.Application.Command
{
    public class ReciboIngresoHandler
    {
        public class StatusReciboResponse : StatusResponse<FileContent>
        {
        }
        public class Command : IRequest<StatusReciboResponse>
        {
            public ReciboIngresoDto ReciboIngresoDto { get; set; }
        }

        public class Handler : IRequestHandler<Command, StatusReciboResponse>
        {
            private readonly AppSettings _appSettings;
            private readonly ILogger<Handler> _logger;
            public Handler(AppSettings appSettings, ILogger<Handler> logger)
            {
                _appSettings = appSettings;
                _logger = logger;
            }

            public async Task<StatusReciboResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusReciboResponse();
                var docxLocation = "";
                var fileDestinationPdf = "";

                try
                {
                    var pathFileServer = _appSettings.RutaPlantilla.Replace("\\", @"\\");
                    var pathTemporal = _appSettings.RutaTemporal;
                    var fileNameDocx = _appSettings.Plantilla.ReciboIngreso;
                    var fileNamePdf = "Recibo_Ingreso.pdf";

                    if (!Directory.Exists(pathTemporal))
                    {
                        Directory.CreateDirectory(pathTemporal);
                    }

                    docxLocation = Path.Combine(pathTemporal, fileNameDocx);
                    fileDestinationPdf = Path.Combine(pathTemporal, fileNamePdf);

                    File.Copy(pathFileServer + "/" + fileNameDocx, docxLocation);

                    var pathLibreOffice = _appSettings.RutaLibreOfficeSoffice;

                    if (!File.Exists(pathLibreOffice))
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ruta de Libre Office no existe"));
                        response.Success = false;
                        return response;
                    }

                    var placeholders = new Placeholders();

                    var reciboIngreso = request.ReciboIngresoDto;

                    placeholders.TextPlaceholderStartTag = "##";
                    placeholders.TextPlaceholderEndTag = "##";
                    placeholders.TablePlaceholderStartTag = "==";
                    placeholders.TablePlaceholderEndTag = "==";

                    var dia = reciboIngreso.Fecha.ToString("dd");
                    var mes = reciboIngreso.Fecha.ToString("MM");
                    var anio = reciboIngreso.Fecha.ToString("yyyy");

                    if (String.IsNullOrEmpty(reciboIngreso.Concepto))
                    {
                        reciboIngreso.NumeroDeposito = reciboIngreso.Concepto;
                    }

                    string[] clasifador = new string[] { };
                    string[] parcial = new string[] { };

                    clasifador = reciboIngreso.detalles.Select(x => Tools.ToUpper(x.Clasificador)).ToArray();
                    parcial = reciboIngreso.detalles.Select(x => String.Format("{0:C}", x.Parcial)).ToArray();


                    placeholders.TextPlaceholders = new Dictionary<string, string>{
                        {"Ejecutora", Tools.reclaceIsNullOrEmpty(reciboIngreso.Ejecutora) },
                        {"SecEje", Tools.reclaceIsNullOrEmpty(reciboIngreso.SecEje)},
                        {"Ruc", Tools.reclaceIsNullOrEmpty(reciboIngreso.NumeroRuc) },
                        {"Numero",Tools.reclaceIsNullOrEmpty(reciboIngreso.Numero) },
                        {"Dia", Tools.reclaceIsNullOrEmpty(dia) },
                        {"Mes", Tools.reclaceIsNullOrEmpty(mes)},
                        {"Anio", Tools.reclaceIsNullOrEmpty(anio)},
                        {"Procedencia", Tools.ToUpper(reciboIngreso.Procedencia)},
                        {"Glosa", Tools.ToUpper(reciboIngreso.Glosa)},
                        {"CuentaCorriente", Tools.ToUpper(reciboIngreso.CuentaCorriente)},
                        {"Concepto", Tools.ToUpper(reciboIngreso.Concepto)},
                        {"nroCompPago", Tools.reclaceIsNullOrEmpty(reciboIngreso.NumeroComprobantePago)},
                        {"nroSIAF", Tools.reclaceIsNullOrEmpty(reciboIngreso.ExpedienteSiaf)},
                        {"nroResolucion", Tools.reclaceIsNullOrEmpty(reciboIngreso.NumeroResolucion)},
                        {"nroBanco", Tools.reclaceIsNullOrEmpty(reciboIngreso.NumeroDeposito)},
                        {"Total", String.Format("{0:C}",reciboIngreso.Total)},
                        {"Plgo", Tools.reclaceIsNullOrEmpty(reciboIngreso.Pliego)},
                        {"Fin", Tools.reclaceIsNullOrEmpty(reciboIngreso.FuenteFinanciamiento)},
                        {"Uni", Tools.reclaceIsNullOrEmpty(reciboIngreso.Unidad)},
                        {"Codigo", Tools.reclaceIsNullOrEmpty(reciboIngreso.Codigo)},
                        {"Codigodos", Tools.reclaceIsNullOrEmpty(reciboIngreso.CodigoDos)},
                        {"Debe", String.Format("{0:C}",reciboIngreso.CuentaDebe)},
                        {"Haber", String.Format("{0:C}",reciboIngreso.CuentaHaber)}
                    };

                    placeholders.TablePlaceholders = new List<Dictionary<string, string[]>>
                    {

                            new Dictionary<string, string[]>()
                            {
                                {"Clasificador",  clasifador },
                                {"Parcial",  parcial }
                            }
                    };

                    var memory = new MemoryStream();
                    var test = new ReportGenerator(pathLibreOffice);
                    test.Convert(docxLocation, fileDestinationPdf, placeholders);
                    using (var stream = new FileStream(fileDestinationPdf, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;

                    var fileContent = new FileContent();
                    fileContent.FileBytes = memory.ToArray();
                    fileContent.ContentType = Tools.ContentType(fileDestinationPdf);
                    fileContent.FileName = fileNamePdf;

                    response.Data = fileContent;



                }
                catch (System.Exception)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }
                finally
                {
                    if (!String.IsNullOrEmpty(docxLocation))
                        if (File.Exists(docxLocation))
                            File.Delete(docxLocation);

                    if (!String.IsNullOrEmpty(fileDestinationPdf))
                        if (File.Exists(fileDestinationPdf))
                            File.Delete(fileDestinationPdf);
                }

                return response;
            }

        }

    }
}
