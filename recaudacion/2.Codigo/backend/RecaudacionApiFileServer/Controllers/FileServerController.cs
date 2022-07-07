using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using RecaudacionApiFileServer.Application.Command;
using RecaudacionApiFileServer.Application.Query;

namespace RecaudacionApiFileServer.Controllers
{
    [ApiController]
    [Route("api/file-server")]
    public class FileServerController : ControllerBase
    {
        private IMediator _mediator;

        public FileServerController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        //[HttpPost("upload/{subDirectory}")]
        //public async Task<IActionResult> UploadFile(string subDirectory, IFormFile file)
        //{
        //    try
        //    {
        //        var response = await _mediator.Send(new AddFileHandler.Command
        //        {
        //            SubDirectory = subDirectory,
        //            File = file,
        //        });
        //        return Ok(response);


        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest($"Ocurrío un error interno en el servicio");
        //    }
        //}

        [HttpGet("download/{subDirectory}/{fileName}")]
        public async Task<IActionResult> DownloadFile(string subDirectory, string fileName)
        {
            var response = await _mediator.Send(new FindFileByNameHandler.Query
            {
                SubDirectory = subDirectory,
                FileName = fileName
            });

            if (response.Success)
            {
                return File(response.Data.FileBytes, response.Data.ContentType, fileName);

            }
            else
            {
                return Ok(response);
            }
        }

        [HttpGet("verify-exists/{subDirectory}/{fileName}")]
        public async Task<IActionResult> VerifyExists(string subDirectory, string fileName)
        {
            var response = await _mediator.Send(new FindFileExistsNameHandler.Query
            {
                SubDirectory = subDirectory,
                FileName = fileName
            });

            return Ok(response);
        }



    }
}
