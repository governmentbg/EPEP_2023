using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Models;
using Epep.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epep.Web.Controllers
{

    [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
    public class PricelistController : BaseController
    {
        private readonly IAdministrativeService adminService;
        private readonly INomenclatureService nomService;
        private readonly IPricelistService pricelistService;
        public PricelistController(
            IPricelistService pricelistService,
            IAdministrativeService adminService,
            INomenclatureService nomService)
        {
            this.pricelistService = pricelistService;
            this.adminService = adminService;
            this.nomService = nomService;
        }


        public IActionResult Index()
        {
            var filter = new FilterPricelistVM();
            return View(filter);
        }

        [HttpPost]
        public IActionResult LoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterPricelistVM>();
            var data = pricelistService.PricelistSelect(filter);
            return request.GetResponse(data);
        }

        public async Task<IActionResult> Add()
        {
            var model = new MoneyPricelist()
            {
                DateFrom = DateTime.Now
            };
            await SetViewBag();
            return PartialView(nameof(Edit), model);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var model = await pricelistService.PricelistGetById(id);
            await SetViewBag();
            return PartialView(nameof(Edit), model);
        }

        async Task SetViewBag()
        {
            ViewBag.DocumentsList_ddl = await nomService.GetDDL_ElectronicDocumentTypes();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MoneyPricelist model)
        {
            var result = await pricelistService.PricelistSaveData(model);
            AuditFromSaveResult(result, model.Id == 0, $"Тарифа {model.Name}");
            return Json(result);
        }

        [HttpPost]
        public IActionResult ValuesLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<MoneyPricelist>();
            var data = pricelistService.SelectPricelistValueByPricelist(filter.Id);
            return request.GetResponse(data);
        }

        public async Task<IActionResult> AddValue(long pricelistId)
        {
            var model = new MoneyPricelistValue()
            {
                MoneyPricelistId = pricelistId
            };
            await SetViewBagValue();
            return PartialView(nameof(EditValue), model);
        }

        public async Task<IActionResult> EditValue(long id)
        {
            var model = await pricelistService.GetByIdAsync<MoneyPricelistValue>(id);
            await SetViewBagValue();
            return PartialView(nameof(EditValue), model);
        }

        [HttpPost]
        public async Task<IActionResult> EditValue(MoneyPricelistValue model)
        {
            var result = await pricelistService.PricelistValueSaveData(model);
            AuditFromSaveResult(result, model.Id == 0, $"Стойност {model.Type}");
            return Json(result);
        }

        async Task SetViewBagValue()
        {
            ViewBag.Type_ddl = new List<SelectListItem>() {
            new SelectListItem("Стойност", NomenclatureConstants.MoneyValueTypes.Value.ToString()),
            new SelectListItem("Процент", NomenclatureConstants.MoneyValueTypes.Procent.ToString())
            };
            ViewBag.MoneyCurrencyId_ddl = await pricelistService.GetDDL_Currencies();
        }
    }
}
