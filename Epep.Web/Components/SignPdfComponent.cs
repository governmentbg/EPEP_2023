using Epep.Core.Contracts;
using Epep.Core.ViewModels.Common;
using IO.SignTools.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Epep.Web.Components
{
    public class SignPdfComponent : ViewComponent
    {
        private readonly IBlobService blobService;
        private readonly ILogger logger;
        private readonly IIOSignToolsService signTools;
        public SignPdfComponent(
            IBlobService blobService,
            ILogger<SignPdfComponent> logger,
            IIOSignToolsService signTools)
        {
            this.blobService = blobService;
            this.logger = logger;
            this.signTools = signTools;
        }

        public async Task<IViewComponentResult> InvokeAsync(SignDocumentVM info, string viewName = "")
        {

            try
            {
                var blobInfo = await blobService.GetBlobInfo(info.BlobKey);
                if (blobInfo == null)
                {
                    info.ErrorMessage = "Невалиден файл за подписване";
                    return await Task.FromResult<IViewComponentResult>(View("Error", info));
                }
                var fileBytes = await blobService.GetFileContent(info.BlobKey);

                using (MemoryStream ms = new MemoryStream(fileBytes))
                {
                    var (hash, tempPdfId) = await signTools.GetPdfHash(ms, info.Reason, info.Location);

                    info.PdfUrl = Url.Action("GetFile", "Pdf", new { fileName = blobInfo.FileName, fileKey = info.BlobKey });
                    info.PdfId = tempPdfId;
                    info.PdfHash = hash;
                    info.FileName = blobInfo.FileName;
                    //info.FileTitle = pdf.FileTitle;
                    info.FileId = blobInfo.BlobContentId.ToString();
                    //model.SignituresCount = pdf.SignituresCount ?? 0;
                }
            }
            catch (ArgumentException aex)
            {
                logger.LogError(aex, "SignPdf Error");
                info.ErrorMessage = "Невалиден файл за подписване";

                return await Task.FromResult<IViewComponentResult>(View("Error", info));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SignPdf Error");
                return await Task.FromResult<IViewComponentResult>(View("Error", info));
            }

            return await Task.FromResult<IViewComponentResult>(View(viewName, info));
        }
    }
}
