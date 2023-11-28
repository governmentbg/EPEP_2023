using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Document;
using Epep.Core.ViewModels.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epep.Web.Controllers
{
    [Authorize]
    public class MoneyController : Controller
    {
        private readonly IPaymentService paymentService;
        public MoneyController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }
        public IActionResult Index()
        {
            var filter = new FilterObligationVM();
            ViewBag.PaymentStatus_ddl = new List<SelectListItem>()
            {
                new SelectListItem("Неплатени","2"),
                new SelectListItem("Платени","1")
            }.PrependAllItem();
            return View(filter);
        }

        [HttpPost]
        public IActionResult LoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterObligationVM>();
            var data = paymentService.Select(filter);
            return request.GetResponse(data.AsQueryable());
        }

        public async Task<IActionResult> Details(Guid gid)
        {
            var obligation = await paymentService.GetByGidAsync<MoneyObligation>(gid);
            switch (obligation.AttachmentType)
            {
                case NomenclatureConstants.AttachedTypes.ElectronicDocument:
                    var docGid = await paymentService.GetPropById<ElectronicDocument, Guid>(x => x.Id == obligation.ParentId, x => x.Gid);
                    return RedirectToAction(nameof(DocumentController.Details), "Document", new { gid = docGid });

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
