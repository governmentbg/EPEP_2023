using eCase.Data.Upgrade.Contracts;
using eCase.Domain.Entities.Upgrade;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace eCase.Web.Controllers
{

    public partial class HomeController : Controller
    {
        private readonly IUpgradeRepository upgRepository;
        public HomeController(IUpgradeRepository upgRepository)
        {
            this.upgRepository = upgRepository;
        }

        public virtual ActionResult Index()
        {
            //var model = upgRepository.AllReadonly<UserRegistration>()
            //                            .Include(x => x.Assignments)
            //                            .Where(x => x.EGN == "7706284005")
            //                            .ToList();


            return View();
        }

        public virtual ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public virtual ActionResult Help()
        {
            return View();
        }

        public virtual ActionResult ElectronicCasesAccessRules()
        {
            return View();
        }

        public virtual ActionResult AccessibilityPolicy()
        {
            return View();
        }

        public virtual ActionResult AboutTheSystem()
        {
            return View();
        }

        public virtual ActionResult CourtsList()
        {
            return View();
        }

    }
}