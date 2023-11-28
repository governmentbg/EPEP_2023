using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Document;
using IO.SignTools.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMART.Extensions;

namespace Epep.Web.Controllers
{
    [Authorize]
    public class DocumentController : BaseController
    {
        private readonly IDocumentService docService;
        private readonly ICaseService caseService;
        private readonly INomenclatureService nomService;
        private readonly IPricelistService pricelistService;
        private readonly IBlobService blobService;
        private readonly IIOSignToolsService signtoolsService;
        private readonly IPaymentService paymentService;
        public DocumentController(
            IDocumentService docService,
            ICaseService caseService,
            INomenclatureService nomService,
            IPricelistService pricelistService,
            IIOSignToolsService signtoolsService,
            IPaymentService paymentService,
            IBlobService blobService)
        {
            this.docService = docService;
            this.caseService = caseService;
            this.nomService = nomService;
            this.blobService = blobService;
            this.pricelistService = pricelistService;
            this.signtoolsService = signtoolsService;
            this.paymentService = paymentService;
        }


        public async Task<IActionResult> Index()
        {
            await SetViewBag(null, "Всички");
            return View();
        }

        [HttpPost]
        public IActionResult LoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterDocumentListVM>();
            var data = docService.Select(filter);
            return request.GetResponse(data.AsQueryable());
        }

        public async Task<IActionResult> Add(Guid? caseGid = null, Guid? sideGid = null)
        {
            var model = await docService.InitDocument(caseGid, sideGid);
            if (model.Result)
            {
                return RedirectToAction(nameof(Edit), new { gid = model.ObjectId });
            }
            SetErrorMessage(model.Message);
            return View(model);
        }
        public async Task<IActionResult> Edit(Guid gid)
        {
            var model = await docService.GetById(gid);
            await SetViewBag(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ElectronicDocumentVM model)
        {
            switch (model.SaveMode)
            {
                case "preview":
                    {
                        model.Sanitize();
                        if (model.CourtId == null)
                        {
                            ModelState.AddModelError(nameof(ElectronicDocumentVM.CourtId), "Изберете съд");
                        }
                        if (model.ElectronicDocumentTypeId == null)
                        {
                            ModelState.AddModelError(nameof(ElectronicDocumentVM.ElectronicDocumentTypeId), "Изберете документ");
                        }

                        if (model.DocumentKind != NomenclatureConstants.DocumentKinds.SideDoc)
                        {
                            var fileList = await docService.GetFileList(model.Gid);
                            if (fileList.Count == 0)
                            {
                                ModelState.AddModelError(nameof(ElectronicDocumentVM.FileError), "Моля, прикачете поне един документ.");
                            }
                        }

                        if (!ModelState.IsValid)
                        {
                            await SetViewBag(model);
                            return View(nameof(Edit), model);
                        }
                        var result = await docService.SaveDocumentData(model);
                        if (result.Result)
                        {
                            await prepareDocumentFile(model.Gid);

                            return RedirectToAction(nameof(Details), new { gid = model.Gid });
                        }
                        break;
                    }
                case "close":
                    {
                        var result = await docService.SaveDocumentData(model);
                        SetSaveResultMessage(result);
                        return RedirectToAction(nameof(Index));
                    }
                default:
                    break;
            }
            await SetViewBag(model);
            return View(nameof(Edit), model);
        }


        public async Task<IActionResult> Correct(Guid gid)
        {
            var result = await docService.CorrectDocument(gid);
            if (result.Result)
            {
                SetSuccessMessage("Документът е върнат за корекция");
                return RedirectToAction(nameof(Edit), new { gid = gid });
            }
            return RedirectToAction(nameof(Details), new { gid = gid });
        }

        public async Task<IActionResult> Details(Guid gid)
        {
            var model = await docService.ReadForPrint(gid);
            if (model == null || !model.DocumentApplyBlobKey.HasValue)
            {
                return RedirectToAction(nameof(Edit), new { gid });
            }

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gid">MoneyObligation.Gid</param>
        /// <returns></returns>
        public async Task<IActionResult> Payment(Guid gid)
        {
            var moneyObligation = await docService.GetByGidAsync<MoneyObligation>(gid);
            var docGid = await docService.GetPropById<ElectronicDocument, Guid>(x => x.Id == moneyObligation.ParentId, x => x.Gid);
            var model = await docService.ReadForPrint(docGid);
            if (model == null || !model.DocumentApplyBlobKey.HasValue || model.DateApply == null)
            {
                return RedirectToAction(nameof(Edit), new { gid });
            }
            if (model.DatePaid != null)
            {
                return RedirectToAction(nameof(Details), new { gid });
            }
            var manageResult = await paymentService.ManageObligationPayments(gid, Request.Scheme);
            if (manageResult.Result)
            {
                if (!string.IsNullOrEmpty(manageResult.CardFormUrl))
                {
                    return Redirect(manageResult.CardFormUrl);
                }
                return RedirectToAction(nameof(Details), new { gid = docGid });
            }
            else
            {
                SetErrorMessage(manageResult.Message);
                return RedirectToAction(nameof(Details), new { gid = docGid });
            }
        }

        public async Task<IActionResult> PaymentSuccess(Guid payment)
        {
            var result = await paymentService.UpdateStatus(payment, true);
            SetSaveResultMessage(result);
            if (result.ParentId != null)
            {
                return RedirectToAction(nameof(Details), new { gid = result.ParentId });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> PaymentFailed(Guid payment)
        {
            var result = await paymentService.UpdateStatus(payment, false);
            SetSaveResultMessage(result);
            if (result.ParentId != null)
            {
                return RedirectToAction(nameof(Details), new { gid = result.ParentId });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> RefreshFile(Guid gid)
        {
            await prepareDocumentFile(gid);

            SetSuccessMessage("Документът е прегенериран успешно");

            return RedirectToAction(nameof(Details), new { gid = gid });
        }

        public async Task<IActionResult> GetPricelistsByDocument(long documentTypeId)
        {
            var model = await pricelistService.SelectPricelistsByDocument(documentTypeId);
            return Json(model);
        }

        public async Task<IActionResult> GetDocumentListByCourt(long courtId, int? documentKind)
        {
            var model = (await nomService.GetDDL_ElectronicDocumentTypes(documentKind, courtId)).PrependAllItem("Изберете").SingleOrSelect();
            return Json(model);
        }

        public async Task<IActionResult> GetPrice(long pricelistId, decimal baseAmount = 0M)
        {
            var model = await pricelistService.GetPrice(pricelistId, baseAmount);
            return Json(model);
        }

        async Task prepareDocumentFile(Guid gid)
        {
            var model = await docService.ReadForPrint(gid);
            if (model == null)
            {
                return;
            }

            foreach (var file in model.Files)
            {
                var fileContent = await blobService.GetFileContent(file.BlobKey);
                file.SignersInfo = getSignerInfo(fileContent, file.FileName);
            }

            var pdfBytes = await (new ViewAsPdfByteWriter("_DocumentBlank", model, true)).GetByte(this.ControllerContext);
            await docService.SaveDocumentApply(gid, pdfBytes);
        }

        //public async Task<IActionResult> file(Guid gid)
        //{
        //    var model = await docService.ReadForPrint(gid);
        //    if (model == null)
        //    {
        //        return null;
        //    }

        //    foreach (var file in model.Files)
        //    {
        //        var fileContent = await blobService.GetFileContent(file.BlobKey);
        //        file.SignersInfo = getSignerInfo(fileContent, file.FileName);
        //    }

        //    var pdfBytes = await (new ViewAsPdfByteWriter("_DocumentBlank", model, true)).GetByte(this.ControllerContext);
        //    return File(pdfBytes, "application/pdf");
        //}


        public async Task<IActionResult> SignDocumentComplete(Guid gid)
        {
            var saveRes = await docService.SaveDocumentApplyTime(gid);
            if (saveRes.Result)
            {
                SetSuccessMessage("Документът е подписан успешно.");
                return RedirectToAction(nameof(Details), new { gid });
            }
            return RedirectToAction(nameof(Details), new { gid });
        }
        public async Task<IActionResult> SignDocument(Guid gid)
        {
            var model = await docService.ReadForPrint(gid);
            if (model == null)
            {
                // return false;
            }

            var signModel = new SignDocumentVM()
            {
                BlobKey = model.DocumentApplyBlobKey.Value,
                FileName = model.DocumentTypeName,
                Location = "Портал ЕПЕП",
                Reason = "Подписване",
                SuccessUrl = Url.Action("SignDocumentComplete", "Document", new { gid }),
                CancelUrl = Url.Action("Details", "Document", new { gid }),
                ErrorUrl = Url.Action("Details", "Document", new { gid })
            };
            return View("_SignPdf", signModel);
        }


        async Task SetViewBag(ElectronicDocumentVM document = null, string allItemText = "Изберете")
        {
            int? documentKind = null;
            if (document != null)
            {
                documentKind = document.DocumentKind;
            }

            long? courtId = null;
            if (document != null && document.DocumentKind == NomenclatureConstants.DocumentKinds.Compliant)
            {
                courtId = document.CourtId;
            }

            ViewBag.CourtId_ddl = (await nomService.GetDDL_CourtsForDocument(documentKind)).PrependAllItem(allItemText);
            ViewBag.ElectronicDocumentTypeId_ddl = (await nomService.GetDDL_ElectronicDocumentTypes(documentKind, courtId)).PrependAllItem(allItemText).SingleOrSelect();
            var years = nomService.GetDDL_CaseYears().PrependAllItem();
            ViewBag.CaseYear_ddl = years;
        }

        #region Files

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> FileUpload(Guid gid, ICollection<IFormFile> uploadFile)
        {
            if (uploadFile == null || uploadFile.Count == 0)
            {
                return Json(new SaveResultVM(false, "Изберете файл"));
            }
            var result = new List<SaveResultVM>();
            foreach (var file in uploadFile)
            {
                if (!checkFile(file))
                {
                    return Json(new SaveResultVM(false, $"Файл {Path.GetFileName(file.FileName)} не е подписан и/или не се поддържа от системата!"));
                }
                result.Add(await docService.UploadFile(gid, file));
            }

            return Json(new SaveResultVM(!result.Any(f => !f.Result)));
        }

        bool checkFile(IFormFile file)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    ms.Position = 0;

                    var signersInfo = getSignerInfo(ms.ToArray(), file.FileName);
                    return !string.IsNullOrWhiteSpace(signersInfo);
                }
            }
            catch (Exception ex) { }
            return false;
        }

        string getSignerInfo(byte[] fileContent, string fileName)
        {
            var result = "";
            try
            {
                var _fileName = System.IO.Path.GetFileName(fileName).ToLower();
                var fileExt = System.IO.Path.GetExtension(_fileName).TrimStart('.');
                if (!NomenclatureConstants.FilesAccepted.AcceptedExt.Contains(fileExt))
                {
                    return string.Empty;
                }
                if (!NomenclatureConstants.FilesAccepted.AcceptedFileEnds.Any(f => _fileName.EndsWith(f)))
                {
                    return string.Empty;
                }
                var signerInfo = signtoolsService.GetSignerInfo(fileContent, fileExt);
                if (signerInfo != null && !signerInfo.Any(s => s.ValidTo < DateTime.Now))
                {
                    foreach (var signer in signerInfo)
                    {
                        result += $"{signer.Name},сертификат: {signer.CertificateNumber}/{signer.Issuer}; Валиден до: {signer.ValidTo:dd.MM.yyyy HH:mm:ss}";
                    }
                }
                return result;
            }
            catch (Exception ex) { }
            return string.Empty;
        }

        public async Task<IActionResult> GetFileList(Guid gid)
        {
            var model = await docService.GetFileList(gid);
            return PartialView("_FileList", model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFile(Guid gid)
        {
            var model = await docService.RemoveFile(gid);

            return Json(model);
        }


        #endregion

        #region Sides

        public async Task<IActionResult> GetSideList(Guid gid)
        {
            var model = await docService.SelectSides(gid, null);
            return PartialView("_SideList", await model.ToListAsync());
        }

        public async Task<IActionResult> SideAdd(Guid gid)
        {
            var model = new ElectronicDocumentSideVM()
            {
                SubjectTypeId = NomenclatureConstants.SubjectTypes.Person,
                DocumentGid = gid,
                SideGid = Guid.Empty
            };
            await SetViewBagSide();
            return PartialView("_SideEdit", model);
        }

        public async Task<IActionResult> SideEdit(Guid gid)
        {
            var model = await (await docService.SelectSides(Guid.Empty, gid)).FirstOrDefaultAsync();
            await SetViewBagSide();
            return PartialView("_SideEdit", model);
        }
        async Task SetViewBagSide()
        {
            ViewBag.SubjectTypeId_ddl = nomService.GetDDL_SubjectTypes();
            ViewBag.SideInvolvementKindId_ddl = await nomService.GetDDL_SideInvolvementKind();
        }

        [HttpPost]
        public async Task<IActionResult> SaveSideData(ElectronicDocumentSideVM model)
        {
            var result = await docService.SaveSideData(model);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSide(Guid gid)
        {
            var model = await docService.RemoveSide(gid);
            return Json(model);
        }



        #endregion
    }
}
