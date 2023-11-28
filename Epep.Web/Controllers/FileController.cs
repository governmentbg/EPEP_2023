using Epep.Core.Contracts;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using ICSharpCode.SharpZipLib.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Web;

namespace Epep.Web.Controllers
{
    public class FileController : ControllerBase
    {
        private readonly INomenclatureService nomService;
        private readonly IBlobService blobService;
        public FileController(IBlobService _blobService, INomenclatureService _nomService)
        {
            blobService = _blobService;
            nomService = _nomService;
        }

        [HttpGet]
        [Route("act/getactpublicfile")]
        public async Task<IActionResult> DownloadPublicAct(Guid guid, CancellationToken cancellationToken)
        {
            var act = await nomService.GetByGidAsync<Act>(guid);
            if(act!= null && act.PublicActBlobKey != null) {
                return await Download(act.PublicActBlobKey.Value, cancellationToken);
            }

            return Content("Избраният документ не е достъпен");
        }

        [HttpGet]
        [Route("act/getmotivepublicfile")]
        public async Task<IActionResult> DownloadPublicMotive(Guid guid, CancellationToken cancellationToken)
        {
            var act = await nomService.GetByGidAsync<Act>(guid);
            if (act != null && act.PublicMotiveBlobKey != null)
            {
                return await Download(act.PublicMotiveBlobKey.Value, cancellationToken);
            }

            return Content("Избраният документ не е достъпен");
        }

        [HttpGet]
        [Route("api/file/download/{fileKey:guid}")]
        public async Task<IActionResult> Download(Guid fileKey, CancellationToken cancellationToken)
        {
            try
            {
                BlobInfo blobInfo = await blobService.GetBlobInfo(fileKey, cancellationToken);
                if (blobInfo == null)
                {
                    return NotFound();
                }
                var responseStream = new MemoryStream();
                await blobService.CopyBlobContentToResponseStream(responseStream, null, blobInfo, cancellationToken);
                responseStream.Position = 0;
                return File(responseStream, blobService.GetMimeType(blobInfo.FileName), blobInfo.FileName);
            }
            catch
            {
                //throw an OperationCanceledException if we have been canceled
                cancellationToken.ThrowIfCancellationRequested();

                return Content("Избраният документ не е достъпен");
                //otherwise rethrow the original exception
                throw;
            }
        }

        [Route("api/file/previewpdf/{fileKey:guid}")]
        public async Task<IActionResult> PreviewPdf(Guid fileKey, CancellationToken cancellationToken)
        {
            try
            {

                BlobInfo blobInfo = await blobService.GetBlobInfo(fileKey, cancellationToken);
                if (blobInfo == null)
                {
                    return NotFound();
                }
                var responseStream = new MemoryStream();


                await blobService.CopyBlobContentToResponseStream(responseStream, null, blobInfo, cancellationToken);
                responseStream.Position = 0;
                var contentDispositionHeader = new ContentDisposition
                {
                    Inline = true,
                    FileName = HttpUtility.UrlPathEncode(blobInfo.FileName).Replace(",", "%2C")
                };
                Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());
                return File(responseStream, blobService.GetMimeType(blobInfo.FileName));
            }
            catch
            {
                //throw an OperationCanceledException if we have been canceled
                cancellationToken.ThrowIfCancellationRequested();

                return Content("Избраният документ не е достъпен");
                //otherwise rethrow the original exception
                throw;
            }
        }
    }
}
