using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Models;
using Epep.Core.ViewModels;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.User;
using Epep.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epep.Web.Controllers
{
    [Authorize(Policy = OrganizationRepresentativePolicyRequirement.Name)]
    public class OrganizationController : BaseController
    {
        private readonly UserManager<UserRegistration> userManager;
        private readonly IUserService userService;
        private readonly IOrganizationService orgService;
        private readonly ILogger<UserController> logger;
        private readonly IUserContext userContext;
        public OrganizationController(
            UserManager<UserRegistration> _userManager,
            ILogger<UserController> _logger,
            IUserService _userService,
            IOrganizationService _orgService,
            IUserContext _userContext)
        {
            this.userManager = _userManager;
            this.logger = _logger;
            userService = _userService;
            orgService = _orgService;
            userContext = _userContext;
        }


        [HttpPost]
        public IActionResult LoadData([FromBody] GridRequestModel request)
        {
            var data = orgService.SelectOrganizationUsersByOrganization();
            return request.GetResponse(data);
        }

        public IActionResult OrganizationUserAdd()
        {
            if (userContext.UserType != NomenclatureConstants.UserTypes.OrganizationRepresentative)
            {
                return Content("Непозволена операция");
            }
            var model = new UserRegistrationVM()
            {
                IsActive = true
            };
            ViewBag.UserType_ddl = new List<SelectListItem>()
            {
                new SelectListItem("Представляващ",NomenclatureConstants.UserTypes.OrganizationRepresentative.ToString())
                ,new SelectListItem("Юрист",NomenclatureConstants.UserTypes.OrganizationUser.ToString())
            };
            return PartialView("_OrganizationUserEdit", model);
        }

        public async Task<IActionResult> OrganizationUserEdit(Guid gid)
        {
            var model = await orgService.OrganizationUserGetByGid(gid);
            ViewBag.UserType_ddl = new List<SelectListItem>()
            {
                new SelectListItem("Представляващ",NomenclatureConstants.UserTypes.OrganizationRepresentative.ToString()),
                new SelectListItem("Юрист",NomenclatureConstants.UserTypes.OrganizationUser.ToString())
            };
            //if (model.UserType != NomenclatureConstants.UserTypes.OrganizationUser)
            //{
            //    return Content("Непозволена операция");
            //}
            return PartialView("_OrganizationUserEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> OrganizationUserEdit(UserRegistrationVM model)
        {
            if (model == null)
            {
                return Json(new SaveResultVM(false));
            }
            var validation = await orgService.ValidateOrganizationUser(model);
            if (!validation.Result)
            {
                return Json(validation);
            }
            if (!model.Gid.HasValue)
            {
                return await organizationUserAdd(model);
            }

            var orgUser = await orgService.GetByGidAsync<UserRegistration>(model.Gid.Value);
            model.UserType = orgUser.UserTypeId;
            string auditInfo = await userService.GetAudiInfo_User(orgUser, model);
            orgUser.EGN = model.EGN;
            orgUser.FullName = model.FullName;
            orgUser.Email = model.Email;
            orgUser.IsActive = model.IsActive;
            var userUpdateResult = await userManager.UpdateAsync(orgUser);
            if (userUpdateResult.Succeeded)
            {
                var res = new SaveResultVM(true);
                res.AuditInfo = auditInfo;
                AuditFromSaveResult(res, false, $"Потребител: {model.FullName} ({userContext.OrganizationName})");
                return Json(res);
            }
            else
            {
                return Json(new SaveResultVM(false));
            }
        }



        private async Task<IActionResult> organizationUserAdd(UserRegistrationVM model)
        {

            var organizationEntity = await orgService.GetByIdAsync<UserRegistration>(userContext.OrganizationUserId);
            if (organizationEntity == null)
            {
                return Json(new SaveResultVM(false));
            }
            var orgUser = model.MapToEntity();
            orgUser.OrganizationUserId = organizationEntity.Id;
            orgUser.UIC = organizationEntity.UIC;
            //if (orgUser.UserTypeId == NomenclatureConstants.UserTypes.OrganizationUser)
            //{
                orgUser.ComfirmedDate = DateTime.Now;
                orgUser.ComfirmedUserId = userContext.UserId;
            //}

            var userCreateResult = await userManager.CreateAsync(orgUser);
            if (userCreateResult.Succeeded)
            {
                string auditInfo = await userService.GetAudiInfo_User(null, model);
                var res = new SaveResultVM(true);
                res.AuditInfo = auditInfo;
                AuditFromSaveResult(res, true, $"Потребител: {model.FullName} ({userContext.OrganizationName})");
                return Json(res);
            }
            else
            {
                return Json(new SaveResultVM(false));
            }
        }

        [HttpPost]
        public async Task<IActionResult> OrganizationCaseLoadData([FromBody] GridRequestModel request)
        {
            var loader = request.GetData<GidLoaderVM>();
            var data = await orgService.SelectOrganizationUsersByCase(loader);
            return request.GetResponse(data);
        }

        public async Task<IActionResult> OrganizationCaseAdd(Guid gid)
        {
            var model = new OrganizationUserVM()
            {
                CaseGid = gid
            };
            ViewBag.UserGid_ddl = await orgService.GetDDL_OrganizationUsers(NomenclatureConstants.UserTypes.OrganizationUser);
            return PartialView("_OrganizationCaseEdit", model);
        }

        public async Task<IActionResult> OrganizationCaseEdit(Guid gid)
        {
            var model = await orgService.SelectOrganizationUsersByGid(gid);
            ViewBag.UserGid_ddl = await orgService.GetDDL_OrganizationUsers(NomenclatureConstants.UserTypes.OrganizationUser);
            return PartialView("_OrganizationCaseEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> OrganizationCaseSaveData(OrganizationUserVM model)
        {
            var data = await orgService.OrganizationCase_SaveData(model);
            return Json(data);
        }
    }
}
