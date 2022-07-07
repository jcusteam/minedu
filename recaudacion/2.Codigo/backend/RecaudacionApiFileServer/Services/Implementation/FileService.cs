using System.IO;
using System.Threading.Tasks;
using RecaudacionApiFileServer.Services.Contracts;
using Microsoft.AspNetCore.Http;
using RecaudacionUtils;
using RecaudacionApiFileServer.Domain;
using System;

namespace RecaudacionApiFileServer.Services.Implementation
{
    public class FileService : IFileService
    {
        private readonly AppSettings _appSettings;
        public FileService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<StatusResponse<FileContent>> FindByNameFile(string subDirectory, string fileName)
        {
            var response = new StatusResponse<FileContent>();

            try
            {

                var fileContent = new FileContent();
                var fileServer = _appSettings.RutaFileServer.Replace("\\", @"\\");

                var filePath = fileServer + "/" + subDirectory + "/" + fileName;

                if (!File.Exists(filePath))
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "El directorio del archivo no existe"));
                    response.Success = false;
                    return response;
                }

                byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

                fileContent.FileBytes = fileBytes;
                fileContent.FileName = fileName;
                fileContent.ContentType = Tools.ContentType(filePath);
                response.Data = fileContent;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error el momento de obtener el archivo"));
            }

            return response;

        }

        public async Task<StatusResponse<string>> SaveFile(IFormFile file, string subDirectory)
        {
            var response = new StatusResponse<string>();
            try
            {
                subDirectory = subDirectory ?? string.Empty;
                var fileServer = _appSettings.RutaFileServer.Replace("\\", @"\\");
                var target = Path.Combine(fileServer, subDirectory);

                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                var filePath = Path.Combine(target, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                response.Data = filePath;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, "Ocurrio un error el momento de subir el archivo"));
            }

            return response;
        }
    }
}
