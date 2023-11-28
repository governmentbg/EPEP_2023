using Epep.Core.Contracts;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.Services;
using Epep.Core.ViewModels;
using Epep.Core.ViewModels.Case;
using Epep.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epep.Web.Controllers
{
    [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly INomenclatureService nomService;
        public ReportController(
            IReportService reportService,
            INomenclatureService nomService)
        {
            this.reportService = reportService;
            this.nomService = nomService;
        }

        #region Разрешени достъпи на потребител

        public async Task<IActionResult> UserAssignments(Guid gid)
        {
            var model = await reportService.GetByGidAsync<UserRegistration>(gid);

            if (model == null)
            {
                return RedirectToAction(nameof(HomeController.NotFound), "Home");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserAssignmentsLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<GidLoaderVM>();

            if(request.exportFormat == GridViewConstants.ExportFormats.Excel)
            {
                var bytes = await reportService.ReportUserAssignmentsExcel(filter.Gid);
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }

            var data = await reportService.ReportUserAssignments(filter.Gid);
            return request.GetResponse(data);
        }

        #endregion


        #region Достъпи до дела по Закона за адвокатурата

        public async Task<IActionResult> LawyerView(Guid gid)
        {
            ViewBag.lawyer = await reportService.GetByGidAsync<Lawyer>(gid);
            if (ViewBag.lawyer == null)
            {
                return RedirectToAction(nameof(HomeController.NotFound), "Home");
            }
            var model = new FilterLawyerViewVM()
            {
                LawyerGid = gid
            };
            await SetViewBagLawyerView();
            return View(model);
        }

        private async Task SetViewBagLawyerView()
        {
            ViewBag.CourtId_ddl = (await nomService.GetDDL_Courts()).PrependAllItem();
            ViewBag.CaseKindId_ddl = (await nomService.GetDDL_CaseKind()).PrependAllItem();
            var years = nomService.GetDDL_CaseYears().PrependAllItem();
            ViewBag.RegYear_ddl = years;
        }

        [HttpPost]
        public async Task<IActionResult> LawyerViewLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterLawyerViewVM>();
            if (request.exportFormat == GridViewConstants.ExportFormats.Excel)
            {
                var bytes = await reportService.ReportLawyerViewExcel(filter);
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            filter.MyCasesOnly = false;
            var data = await reportService.ReportLawyerView(filter);
            var responce = request.BuildResponse(data);
            return Json(responce);
        }

        #endregion

        #region Справка дела по съд и година

        public async Task<IActionResult> CourtStat()
        {
            await SetViewBagLawyerView();
            var model = new FilterCaseVM();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CourtStatLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterCaseVM>();
            if (request.exportFormat == GridViewConstants.ExportFormats.Excel)
            {
                var bytes = await reportService.ReportCaseStatExcel(filter);
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            filter.MyCasesOnly = false;
            var data = await reportService.ReportCaseStat(filter);
            var responce = request.BuildResponse(data);
            return Json(responce);
        }

        #endregion

    }
}
