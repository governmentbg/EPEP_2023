using System.Web.Mvc;

namespace VSS.Portal.Controllers
{
    public partial class HomeController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }

        //public virtual ActionResult cupdate()
        //{
        //    return View();
        //}


        public virtual ActionResult Help()
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

        //public ActionResult CourtList()
        //{
        //    return Json(CourtMetadata.CourtsList.Select(x=> new { x.Name,x.CourtCode,x.CityAddress,x.StreetAddress,x.Email}),JsonRequestBehavior.AllowGet);
        //}
    }
}