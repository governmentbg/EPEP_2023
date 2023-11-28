using Epep.Core.Contracts;
using Epep.Core.ViewModels.Common;
using IO.SignTools.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Web;

namespace Epep.Web.Controllers
{
    public class PdfController : BaseController
    {
        private readonly IBlobService blobService;
        private readonly ILogger logger;
        private readonly IIOSignToolsService signTools;
        public PdfController(IBlobService blobService, ILogger<PdfController> logger, IIOSignToolsService signTools)
        {
            this.blobService = blobService;
            this.logger = logger;
            this.signTools = signTools;
        }


        [HttpPost]
        public async Task<IActionResult> SignPdf(SignDocumentVM model)
        {
            try
            {
                //bool overrideSignEnabled = config.GetValue<bool>("Environment:GlobalAdmin:OverrideSign", false);
                (byte[] resultFile, string EGN) = await signTools.EmbedPdfSignature(model.PdfId, model.Signature);

                //if (!string.IsNullOrEmpty(model.SignerUic))
                //{
                //    if (!(userContext.IsUserInRole(AccountConstants.Roles.GlobalAdministrator) && overrideSignEnabled))
                //    {
                //        if (string.Compare(model.SignerUic, EGN, StringComparison.InvariantCultureIgnoreCase) != 0)
                //        {
                //            SetErrorMessage($"Документът трябва да бъде подписан от {model.SignerName}.");
                //            return LocalRedirect(model.ErrorUrl);
                //        }
                //    }
                //}

                var blobInfo = await blobService.GetBlobInfo(model.BlobKey);
                if (blobInfo == null)
                {
                    model.ErrorMessage = "Невалиден файл за подписване";
                    return LocalRedirect(model.ErrorUrl);
                    //return await Task.FromResult<IViewComponentResult>(View("Error", model));
                }
                var fileBytes = await blobService.GetFileContent(model.BlobKey);

                //var fileItem = cdn.Select(model.SourceType, model.SourceId).FirstOrDefault();
                if (blobInfo.BlobContentId.ToString() != model.FileId)
                {
                    SetErrorMessage("Заредения от вас документ е актуализиран/подписан от друг потребител. Моля, повторете стъпката по подписване, за да заредите актуалното му съдържание.");
                    return LocalRedirect(model.ErrorUrl);
                }


                var ts = Math.Abs(model.ClientCode - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                if (ts > (double)600000)
                {
                    SetErrorMessage("Моля, сверете часовника на вашия компютър.");
                    return LocalRedirect(model.ErrorUrl);
                }


                var uplGuid = await blobService.UploadFileToBlobStorage(model.BlobKey, resultFile, blobService.GetMimeType(model.FileName), Core.Services.BlobServiceBase.FileType.AttachedDocument, DateTime.Now, true);
                if (uplGuid != Guid.Empty)
                {
                    return LocalRedirect(model.SuccessUrl);
                }
                else
                {
                    return LocalRedirect(model.ErrorMessage);
                }
                //if (await cdn.MongoCdn_AppendUpdate(new CdnUploadRequest()
                //{
                //    SourceId = model.SourceId,
                //    SourceType = model.SourceType,
                //    ContentType = MediaTypeNames.Application.Pdf,
                //    FileContentBase64 = Convert.ToBase64String(resultFile),
                //    FileName = model.FileName,
                //    Title = model.FileTitle,
                //    SignituresCount = model.SignituresCount + 1
                //}))
                //{
                //    AddAuditInfo(AuditConstants.Operations.Update, "Подписване на документ", model.FileTitle, "Подписване");
                //}

                //SetSuccessMessage("Документът е успешно подписан");

                //return LocalRedirect(model.SuccessUrl);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "Process Sign PDF result error");
                //var signatureException = ex as SignatureValidationException;

                //if (signatureException != null &&
                //    signatureException.Message == "Signature time is invalid")
                //{
                //    SetErrorMessage("Моля, сверете часовника на вашия компютър.");
                //}
                //else
                //{
                //    SetErrorMessage("Възникна грешка при подписване на документа");
                //    TempData["signError"] = ex.Message;
                //}

                return LocalRedirect(model.ErrorUrl);
            }
        }

        [Route("/pdf/getfile/{fileName}")]
        public async Task<IActionResult> GetFile(Guid fileKey, string fileName, CancellationToken cancellationToken)
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

                //otherwise rethrow the original exception
                throw;
            }
        }

        [HttpGet]
        public IActionResult CheckLSMErrorCode(int errorCode)
        {
            var errorMessage = signTools.GetLSMErrorMessage(errorCode)?.Bg;

            return new JsonResult(new { errorCode, errorMessage });
        }
    }
}
