using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocXToPdfConverter;
using MediatR;
using RecaudacionApiReporte.Application.Command.Dtos;
using System;
using RecaudacionUtils;
using Microsoft.Extensions.Logging;
using RecaudacionApiReporte.Domain;

namespace RecaudacionApiReporte.Application.Command
{
    public class ReciboIngresoVentanillaHandler
    {
        public class StatusReciboVentanillaResponse : StatusResponse<FileContent>
        {
        }
        public class Command : IRequest<StatusReciboVentanillaResponse>
        {
            public ReciboIngresoVentanillaDto ReciboIngresoVentanillaDto { get; set; }
        }

        public class Handler : IRequestHandler<Command, StatusReciboVentanillaResponse>
        {
            private readonly AppSettings _appSettings;
            private readonly ILogger<Handler> _logger;
            public Handler(AppSettings appSettings, ILogger<Handler> logger)
            {
                _appSettings = appSettings;
                _logger = logger;
            }

            public async Task<StatusReciboVentanillaResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusReciboVentanillaResponse();
                var docxLocation = "";
                var fileDestinationPdf = "";

                try
                {

                    var pathFileServer = _appSettings.RutaPlantilla.Replace("\\", @"\\");
                    var pathTemporal = _appSettings.RutaTemporal;
                    var fileNameDocx = _appSettings.Plantilla.ReciboIngresoVentanilla;
                    var fileNamePdf = "Recibo_Ingreso_Ventanilla.pdf";

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

                    var reciboIngreso = request.ReciboIngresoVentanillaDto;

                    placeholders.TextPlaceholderStartTag = "##";
                    placeholders.TextPlaceholderEndTag = "##";
                    placeholders.TablePlaceholderStartTag = "==";
                    placeholders.TablePlaceholderEndTag = "==";
                    placeholders.NewLineTag = "<br/>";

                    var dia = reciboIngreso.Fecha.ToString("dd");
                    var mes = reciboIngreso.Fecha.ToString("MM");
                    var anio = reciboIngreso.Fecha.ToString("yyyy");

                    var liquidaciones = reciboIngreso.Liquidaciones.OrderBy(x => x.ClasificadorIngresoId).ToList();

                    var lista = liquidaciones.GroupBy(p => p.ClasificadorIngresoId).Select(grp => grp.First()).ToList();

                    foreach (var item in lista)
                    {
                        var total = reciboIngreso.Liquidaciones.Where(x => x.ClasificadorIngresoId == item.ClasificadorIngresoId).Sum(x => x.Total);

                        string[] tipoCaptaciones = reciboIngreso.Liquidaciones
                        .Where(x => x.ClasificadorIngresoId == item.ClasificadorIngresoId).Select(x => " -     " + Tools.ToUpper(x.NombreTipoCaptacion)).ToArray();

                        if (tipoCaptaciones.Length > 0)
                        {
                            item.Codigo = item.Codigo + "<br/>" + "  ";
                            item.Parcial = String.Format("{0:C}", total) + "<br/>" + "   ";
                            var captacion = string.Join("<br/>", tipoCaptaciones);
                            item.Clasicador = Tools.ToUpper(item.Nombre) + "<br/>" + captacion;

                        }
                        else
                        {
                            item.Clasicador = item.Nombre;
                            item.Total = total;
                        }

                    }

                    string[] codigo = new string[] { };
                    string[] clasificador = new string[] { };
                    string[] parcial = new string[] { };

                    codigo = lista.Select(x => x.Codigo).ToArray();
                    clasificador = lista.Select(x => x.Clasicador).ToArray();
                    parcial = lista.Select(x => x.Parcial).ToArray();

                    placeholders.TextPlaceholders = new Dictionary<string, string>{
                        {"Ejecutora", Tools.reclaceIsNullOrEmpty(reciboIngreso.Ejecutora) },
                        {"SecEje", Tools.reclaceIsNullOrEmpty(reciboIngreso.SecEje)},
                        {"Ruc", Tools.reclaceIsNullOrEmpty(reciboIngreso.NumeroRuc) },
                        {"Numero",Tools.reclaceIsNullOrEmpty(reciboIngreso.Numero) },
                        {"Dia", dia },
                        {"Mes", mes},
                        {"Anio", anio},
                        {"Procedencia", Tools.ToUpper(reciboIngreso.Procedencia)},
                        {"Glosa", Tools.ToUpper(reciboIngreso.Glosa)},
                        {"Concepto", Tools.ToUpper(reciboIngreso.Concepto)},
                        {"CuentaCorriente", Tools.ToUpper(reciboIngreso.CuentaCorriente)},
                        {"Boleta", Tools.reclaceIsNullOrEmpty(reciboIngreso.BoletaVenta)},
                        {"Factura", Tools.reclaceIsNullOrEmpty(reciboIngreso.Factura)},
                        {"NumeroLiquidacion", Tools.reclaceIsNullOrEmpty(reciboIngreso.numeroLiquidacion)},
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
                                {"Codigo", codigo },
                                {"Clasificador", clasificador  },
                                {"Parcial",  parcial },
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
