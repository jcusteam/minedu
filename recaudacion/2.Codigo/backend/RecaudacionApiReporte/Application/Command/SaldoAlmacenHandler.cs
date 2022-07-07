using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Report;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecaudacionApiReporte.Application.Command.Dtos;
using RecaudacionApiReporte.Domain;
using RecaudacionUtils;

namespace RecaudacionApiReporte.Application.Command
{
    public class SaldoAlmacenHandler
    {
        public class StatusSaldoResponse : StatusResponse<FileContent>
        {

        }
        public class Command : IRequest<StatusSaldoResponse>
        {
            public SaldoAlmacenDto SaldoAlmacenDto { get; set; }
        }

        public class Handler : IRequestHandler<Command, StatusSaldoResponse>
        {
            private readonly AppSettings _appSettings;
            private readonly ILogger<Handler> _logger;
            public Handler(AppSettings appSettings, ILogger<Handler> logger)
            {
                _appSettings = appSettings;
                _logger = logger;
            }

            public async Task<StatusSaldoResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusSaldoResponse();
                var fileDestinationXLSX = "";

                try
                {
                    var pathFileServer = _appSettings.RutaPlantilla.Replace("\\", @"\\");
                    var pathTemporal = _appSettings.RutaTemporal;
                    var saldoAlmacen = request.SaldoAlmacenDto;

                    var memory = new MemoryStream();
                    var fileName = _appSettings.Plantilla.SaldoAlmacen;

                    if (!Directory.Exists(pathTemporal))
                    {
                        Directory.CreateDirectory(pathTemporal);
                    }

                    var pathFileTemplate = Path.Combine(pathTemporal, fileName);
                    fileDestinationXLSX = Path.Combine(pathTemporal, fileName);

                    File.Copy(pathFileServer + "/" + fileName, pathFileTemplate);
                    var template = new XLTemplate(pathFileTemplate);
                    template.AddVariable(saldoAlmacen);
                    template.Generate();
                    template.SaveAs(fileDestinationXLSX);

                    using (var stream = new FileStream(fileDestinationXLSX, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    var fileContent = new FileContent();

                    fileContent.FileBytes = memory.ToArray();
                    fileContent.ContentType = Tools.ContentType(fileDestinationXLSX);
                    fileContent.FileName = fileName;

                    response.Data = fileContent;

                }
                catch (System.Exception)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }
                finally
                {
                    if (!String.IsNullOrEmpty(fileDestinationXLSX))
                        if (File.Exists(fileDestinationXLSX))
                            File.Delete(fileDestinationXLSX);
                }
                return response;
            }

        }
    }
}
