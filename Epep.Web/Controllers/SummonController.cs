using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Extensions;
using Epep.Core.ViewModels.Case;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.Extensions;

namespace Epep.Web.Controllers
{
    [Authorize]
    public class SummonController : BaseController
    {
        private readonly ISummonService summonService;
        private readonly INomenclatureService nomService;
        private readonly ICaseService caseService;
        public SummonController(
            ISummonService _summonService,
            INomenclatureService _nomService,
            ICaseService _caseService)
        {
            summonService = _summonService;
            nomService = _nomService;
            caseService = _caseService;
        }
        public async Task<IActionResult> Index()
        {
            var model = new FilterSummonVM()
            {
                NotReadOnly = true
            };
            await SetViewBagIndex();
            return View();
        }

        private async Task SetViewBagIndex(bool forHearings = false)
        {
            ViewBag.CourtId_ddl = (await nomService.GetDDL_Courts()).PrependAllItem();
            ViewBag.CaseKindId_ddl = (await nomService.GetDDL_CaseKind()).PrependAllItem();
            var years = nomService.GetDDL_CaseYears().PrependAllItem();
            ViewBag.RegYear_ddl = years;
            ViewBag.PrevYear_ddl = years;
            ViewBag.ActYear_ddl = years;
        }

        [Authorize]
        [HttpPost]
        public IActionResult LoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterSummonVM>();
            var data = summonService.SelectSummonsByUser(filter);
            return request.GetResponse(data.AsQueryable());
        }

    }
}
