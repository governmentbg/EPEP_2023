using Epep.Core.Contracts;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epep.Web.Controllers
{
    public class LawyerController : BaseController
    {
        private readonly IUserService userService;
        private readonly IAdministrativeService adminService;
        private readonly INomenclatureService nomService;
        public LawyerController(
            IAdministrativeService adminService,
            INomenclatureService nomService,
            IUserService userService)
        {
            this.adminService = adminService;
            this.nomService = nomService;
            this.userService = userService;
        }

        [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
        public IActionResult Index()
        {
            var filter = new FilterLawyerVM();
            return View(filter);
        }

        [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
        [HttpPost]
        public IActionResult LoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterLawyerVM>();
            var data = adminService.SelectLawyers(filter);
            return request.GetResponse(data);
        }

        private async Task SetViewBag()
        {
            ViewBag.LawyerTypeId_ddl = await nomService.GetDDL_LawyerTypes();
        }

        [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
        public async Task<IActionResult> Add()
        {
            var model = new Lawyer();
            await SetViewBag();
            return PartialView("Edit", model);
        }

        [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
        public async Task<IActionResult> Edit(Guid gid)
        {
            var model = await adminService.GetByGidAsync<Lawyer>(gid);
            await SetViewBag();
            return PartialView("Edit", model);
        }

        [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
        [HttpPost]
        public async Task<IActionResult> Edit(Lawyer model)
        {
            var result = await adminService.Lawyer_SaveData(model);
            AuditFromSaveResult(result, model.LawyerId == 0, $"Адвокат {model.Name}");
            return Json(result);
        }

        [Authorize(Policy = LawyerPolicyRequirement.Name)]
        [HttpPost]
        public IActionResult VacationLoadData([FromBody] GridRequestModel request)
        {
            var data = userService.UserVacation_Select();
            return request.GetResponse(data);
        }

        [Authorize(Policy = LawyerPolicyRequirement.Name)]
        public async Task<IActionResult> VacationAdd()
        {
            var model = new UserVacation();
            await SetViewBagVacation();
            return PartialView(nameof(VacationEdit), model);
        }
        [Authorize(Policy = LawyerPolicyRequirement.Name)]
        public async Task<IActionResult> VacationEdit(long id)
        {
            var model = await userService.GetByIdAsync<UserVacation>(id);
            await SetViewBagVacation();
            return PartialView(nameof(VacationEdit), model);
        }

        [HttpPost]
        [Authorize(Policy = LawyerPolicyRequirement.Name)]
        public async Task<IActionResult> VacationEdit(UserVacation model)
        {
            var result = await userService.UserVacation_SaveData(model);
            //AuditFromSaveResult(result, model.Id == 0, $"Отсъствие");
            return Json(result);
        }

        async Task SetViewBagVacation()
        {
            ViewBag.VacationTypeId_ddl = await userService.GetDDL_UserVacationTypes();
        }
    }
}
