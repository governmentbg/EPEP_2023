using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Payment;
using Epep.Core.ViewModels.User;
using Epep.Web.Extensions;
using IO.Timestamp.Client.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMART.Extensions;

namespace Epep.Web.Controllers
{
    [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
    public class AdminController : BaseController
    {
        private readonly IAdministrativeService adminService;
        private readonly IAuditLogService auditlogService;
        private readonly IUserService userService;
        private readonly INomenclatureService nomService;
        private readonly IRegixService regixService;
        private readonly IBlobService blobService;
        private readonly IEmailService emailService;
        private readonly IMigrationService migrationService;
        private readonly UserManager<UserRegistration> userManager;
        public AdminController(
            IAdministrativeService adminService,
            IAuditLogService auditlogService,
            IUserService userService,
            INomenclatureService nomService,
            IRegixService regixService,
            IBlobService blobService,
            IEmailService emailService,
            IMigrationService migrationService,
            UserManager<UserRegistration> userManager)
        {
            this.adminService = adminService;
            this.auditlogService = auditlogService;
            this.userService = userService;
            this.userManager = userManager;
            this.nomService = nomService;
            this.regixService = regixService;
            this.blobService = blobService;
            this.emailService = emailService;
            this.migrationService = migrationService;
        }

        public async Task<IActionResult> migration()
        {
            var result = await migrationService.MigrateData();
            return Json(result);
        }

        public async Task<IActionResult> regix(string gid)
        {
            var regixModel = await regixService.GetEntityInfo(gid);
            return Content(JsonConvert.SerializeObject(regixModel));
        }

        //public async Task<IActionResult> regixreport(string gid)
        //{
        //    var regixModel = await regixService.GetEntityInfo(gid);
        //    var reportBytes = await (new ViewAsPdfByteWriter("_RegixReport", regixModel, true)).GetByte(this.ControllerContext);

        //    return File(reportBytes, "application/pdf", $"regix{gid}.pdf");

        //}

        public async Task<IActionResult> externaltest(string gid = "831641791")
        {
            string regixResult = "";
            string tsResult = "";
            try
            {
                var regixModel = await regixService.GetEntityInfo(gid);
                regixResult = JsonConvert.SerializeObject(regixModel);
            }
            catch (Exception ex)
            {
                regixResult = ex.Message;
            }

            var timestampClient = HttpContext.RequestServices.GetService<ITimestampClient>();

            DateTime ReadTime = DateTime.Now;
            byte[] tsr = null;
            try
            {
                byte[] tsContent = System.Text.Encoding.UTF8.GetBytes(gid);
                (tsr, ReadTime) = await timestampClient.GetTimestampAsync(tsContent);
                tsResult = $"ts bytes: {tsr.Length};ts date:{ReadTime.ToLocalTime()}";
            }
            catch (Exception ex)
            {
                tsResult = ex.Message;
            }

            return Content(JsonConvert.SerializeObject(new JsonResult(new { regixResult, tsResult })));
        }

        public IActionResult Desktop()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewUsersLoadData([FromBody] GridRequestModel request)
        {
            var data = adminService.NewOrganizationUsers();
            return request.GetResponse(data);
        }

        public async Task<IActionResult> ComfimUser(Guid gid, CancellationToken cancellationToken = default)
        {
            var user = await userService.SelectDataForProfile(gid);
            var model = new UserComfirmVM()
            {
                Gid = user.Gid ?? Guid.Empty,
                UserType = user.UserType,
                UserTypeName = user.UserTypeName,
                FullName = user.FullName,
                Comfirm = true,
                Email = user.Email,
                Uic = user.EGN ?? user.UIC,
                OrganizationName = user.OrgFullName,
                OrganizationUic = user.UIC,
                RegCertificateInfo = user.RegCertificateInfo
            };
            if (!string.IsNullOrEmpty(model.OrganizationUic))
            {
                await prepareUserFiles(model, cancellationToken);
            }
            ViewBag.Comfirm_ddl = new List<SelectListItem>()
            {
                new SelectListItem("Потвърди","true"),
                new SelectListItem("Откажи","false")
            };
            return PartialView("_ComfirmUser", model);
        }


        async Task prepareUserFiles(UserComfirmVM model, CancellationToken cancellationToken = default)
        {
            var user = await adminService.GetByGidAsync<UserRegistration>(model.Gid);
            if (user == null)
            {
                return;
            }

            model.Files = await blobService.SelectAttachedDocument(NomenclatureConstants.AttachedTypes.UserRegistration, user.Id).ToListAsync();


            var regixReportFiles = blobService.SelectAttachedDocument(NomenclatureConstants.AttachedTypes.UserRegistrationRegixReport, user.Id);
            if (await regixReportFiles.AnyAsync())
            {
                model.Files.Insert(0, await regixReportFiles.FirstOrDefaultAsync());
                return;
            }

            var regixModel = await regixService.GetEntityInfo(model.OrganizationUic);
            var reportBytes = await (new ViewAsPdfByteWriter("_RegixReport", regixModel, true)).GetByte(this.ControllerContext);
            var regixTitle = "Отчет Regix";
            var blobInfo = await blobService.AppendUpdateAttachedDocumentFile(NomenclatureConstants.AttachedTypes.UserRegistrationRegixReport, user.Id, reportBytes, $"regixReport{model.OrganizationUic}.pdf", true, regixTitle);
            if (blobInfo != null)
            {
                model.Files.Insert(0, new Core.ViewModels.Case.FileItemVM()
                {
                    FileGid = blobInfo.BlobKey,
                    FileName = blobInfo.FileName,
                    Title = regixTitle
                });
            }
            return;
        }

        [HttpPost]
        public async Task<IActionResult> ComfimUser(UserComfirmVM model)
        {
            var result = await adminService.SaveUserComfirm(model);

            if (result.Result)
            {
                var objInfo = "";
                var userModel = await adminService.GetByGidAsync<UserRegistration>(model.Gid);
                string userName = null;
                var orgName = userModel.FullName;
                var recEmail = userModel.Email;
                if (userModel != null && userModel.UserTypeId == NomenclatureConstants.UserTypes.OrganizationRepresentative)
                {
                    userName = userModel.FullName;
                    var orgModel = await adminService.GetByIdAsync<UserRegistration>(userModel.OrganizationUserId);
                    if (orgModel != null)
                    {
                        orgName = orgModel.FullName;
                        objInfo = $"ЮЛ: {orgModel.FullName}; Представител: {userModel.FullName}";
                    }
                }
                else
                {
                    objInfo = $"ЮЛ: {userModel.FullName}";
                }

                await emailService.NewMailMessage(recEmail, NomenclatureConstants.EmailTemplates.ApprovalRegistrationMessage,
                            JObject.FromObject(
                            new
                            {

                                userName,
                                orgName,
                                isComfirmed = userModel.ComfirmedDate != null,
                                deniedReason = userModel.DeniedDescription
                            }));

                Audit_Operation = (model.Comfirm) ? NomenclatureConstants.AuditOperations.ComfirmUL : NomenclatureConstants.AuditOperations.DenyUL;
                Audit_Object = objInfo;
                Audit_Action = result.AuditInfo;

                result.AuditInfo = null;
            }

            return Json(result);
        }

        public async Task<IActionResult> Users()
        {
            ViewBag.UserType_ddl = (await userService.GetDDL_UserRegistrationTypes()).PrependAllItem();
            ViewBag.ComfirmedMode_ddl = new List<SelectListItem>() {
            new SelectListItem("Всички","-1"),
            new SelectListItem("Непотвърдени","1"),
            new SelectListItem("Потвърдени","2"),
            new SelectListItem("Отказани","3"),
            };
            var model = new UserFilterVM();
            return View(model);
        }

        [HttpPost]
        public IActionResult UsersLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<UserFilterVM>();
            var data = adminService.SelectUsers(filter);
            return request.GetResponse(data);
        }

        public async Task<IActionResult> UserEdit(Guid gid)
        {
            var model = await userService.SelectDataForProfile(gid);
            ViewBag.CourtId_ddl = await nomService.GetDDL_Courts(false);
            if (NomenclatureConstants.UserTypes.OrganizationTypes.Contains((model.UserType)))
            {
                ViewBag.comfirmedInfo = await userService.SelectConfirmInfo(gid);
            }
            return PartialView(model);
        }

        public async Task<IActionResult> UserAdd(bool global = false)
        {
            ViewBag.CourtId_ddl = await nomService.GetDDL_Courts(false);
            var model = new UserRegistrationVM()
            {
                UserType = (global) ? NomenclatureConstants.UserTypes.GlobalAdmin : NomenclatureConstants.UserTypes.CourtAdmin,
                IsActive = true
            };
            return PartialView(nameof(UserEdit), model);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserRegistrationVM model)
        {
            if (model == null)
            {
                return Json(new SaveResultVM(false));
            }

            if (string.IsNullOrEmpty(model.FullName))
            {
                var res = new SaveResultVM(false);
                res.AddError(NomenclatureConstants.Messages.InvalidValue, nameof(model.FullName));
                return Json(res);
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                var res = new SaveResultVM(false);
                res.AddError(NomenclatureConstants.Messages.InvalidValue, nameof(model.Email));
                return Json(res);
            }
            switch (model.UserType)
            {
                case NomenclatureConstants.UserTypes.Organization:
                    if (string.IsNullOrEmpty(model.UIC))
                    {
                        var res = new SaveResultVM(false);
                        res.AddError(NomenclatureConstants.Messages.InvalidValue, nameof(model.UIC));
                        return Json(res);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(model.EGN))
                    {
                        var res = new SaveResultVM(false);
                        res.AddError(NomenclatureConstants.Messages.InvalidValue, nameof(model.EGN));
                        return Json(res);
                    }
                    break;
            }

            if (model.Gid == null)
            {
                return Json(await createAdminUser(model));
            }
            

            var editUser = await userService.GetByGidAsync<UserRegistration>(model.Gid.Value);
            model.UserType = editUser.UserTypeId;
            string auditInfo = await userService.GetAudiInfo_User(editUser, model);
            editUser.EGN = model.EGN;
            editUser.FullName = model.FullName;
            editUser.Email = model.Email;
            editUser.IsActive = model.IsActive;
            editUser.ModifyDate = DateTime.Now;
            if(editUser.UserTypeId == NomenclatureConstants.UserTypes.CourtAdmin && model.CourtId > 0)
            {
                editUser.CourtId = model.CourtId;
            }

            var userUpdateResult = await userManager.UpdateAsync(editUser);
            if (userUpdateResult.Succeeded)
            {
                var res = new SaveResultVM(true);
                res.AuditInfo = auditInfo;
                AuditFromSaveResult(res, false, $"Потребител: {editUser.FullName}");
                await emailService.NewMailMessage(editUser.Email, NomenclatureConstants.EmailTemplates.ChangeUserProfileMessage, JObject.FromObject(
                    new
                    {
                        isActivated = editUser.IsActive,
                        email = editUser.Email,
                        courtName = "",
                        name = editUser.FullName
                    }));
                return Json(res);
            }
            else
            {
                return Json(new SaveResultVM(false));
            }
        }

        private async Task<SaveResultVM> createAdminUser(UserRegistrationVM model)
        {
            var val = await userService.ValidateUserRegistration(model);
            if (!val.Result)
            {
                return val;
            }

            var adminUser = model.MapToEntity();
            IdentityResult resPerson = await userManager.CreateAsync(adminUser);
            if (resPerson.Succeeded)
            {
                string auditInfo = await userService.GetAudiInfo_User(null, model);
                var res = new SaveResultVM(true);
                res.AuditInfo = auditInfo;
                AuditFromSaveResult(res, true, $"Потребител: {model.FullName}");
                var courtName = "";
                if (adminUser.CourtId > 0)
                {
                    courtName = await adminService.GetPropById<Court, string>(x => x.CourtId == adminUser.CourtId.Value, x => x.Name);
                }
                await emailService.NewMailMessage(adminUser.Email, NomenclatureConstants.EmailTemplates.NewUserRegistrationMessage, JObject.FromObject(
                    new
                    {
                        isActivated = adminUser.IsActive,
                        email = adminUser.Email,
                        courtName = courtName,
                        name = adminUser.FullName
                    }));
                return res;
            }
            else
            {
                return new SaveResultVM(false);
            }
        }

        public IActionResult Auditlog()
        {
            var filter = new AuditLogFilterVM()
            {
                DateFrom = DateTime.Now.AddDays(-1),
                DateTo = DateTime.Now,
            };
            return View(filter);
        }

        [HttpPost]
        public IActionResult AuditlogLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<AuditLogFilterVM>();
            var data = auditlogService.Select(filter);
            return request.GetResponse(data);
        }

        public async Task<IActionResult> Courts()
        {
            var model = new FilterCourtVM();
            ViewBag.CourtTypeId_ddl = (await nomService.GetDDL_CourtTypes()).PrependAllItem();
            return View(model);
        }

        [HttpPost]
        public IActionResult CourtsLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterCourtVM>();
            var data = nomService.CourtsSelect(filter);
            return request.GetResponse(data);
        }

        public async Task<IActionResult> CourtEdit(long id)
        {
            var model = await adminService.GetByIdAsync<Court>(id);
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> CourtEdit(Court model)
        {
            var result = await nomService.CourtSaveData(model);
            return Json(result);
        }


        public async Task<IActionResult> TestPayment(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                return Content(message);
            }

            var model = new TestPaymentVM();
            ViewBag.CourtId_ddl = await nomService.GetDDL_CourtsForDocument(NomenclatureConstants.DocumentKinds.Initial);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TestPayment(TestPaymentVM model)
        {
            var result = await adminService.InitTestPayment(model, Request.Scheme);
            if (!string.IsNullOrEmpty(result.CardFormUrl))
            {
                return Redirect(result.CardFormUrl);
            }
            return Content(result.Message);
        }
    }
}
