using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Data.Linq;
using eCase.Data.Repositories;
using eCase.Domain.Entities;
using eCase.Web.Helpers;
using eCase.Web.Models.Statistics;
using System;
using System.Linq;
using System.Web.Mvc;

namespace eCase.Web.Controllers
{
    [AuthorizeUser(AllowedRoleId = UserGroup.SuperAdmin)]
    public partial class StatisticsController : Controller
    {
        private UnitOfWork unitOfWork;
        public StatisticsController(IUnitOfWork unitOfWork, IEntityCodeNomsRepository<Court, EntityCodeNomVO> courtRepository)
        {
            this.unitOfWork = (UnitOfWork)unitOfWork;
        }

        [HttpGet]
        public virtual ActionResult Cases(int? year)
        {
            IQueryable<Case> cases = unitOfWork.DbContext.Set<Case>();

            var predicate = PredicateBuilder.True<Case>();

            if (year.HasValue)
            {
                predicate = predicate.And(c => c.CaseYear == year.Value);
            }

            if (!predicate.IsTrueLambdaExpr())
            {
                cases = cases.Where(predicate);
            }

            var result =
                (from court in unitOfWork.DbContext.Set<Court>()

                 join c in cases on court.CourtId equals c.CourtId into j1
                 from c in j1.DefaultIfEmpty()

                 group new
                 {
                     CaseId = (long?)c.CaseId
                 }
                 by new
                 {
                     court.CourtId,
                     court.Name
                 } into g

                 select new CourtInfo
                 {
                     CourtName = g.Key.Name,
                     CasesCount = g.Count(e => e.CaseId != null)
                 })
                .OrderBy(e => e.CourtName)
                .ToList();

            CasesCountVM vm = new CasesCountVM()
            {
                Courts = result,
                TotalCount = result.Sum(c => c.CasesCount)
            };

            vm.Years = Enumerable.Range(DateTime.Now.Year - 30, 31).OrderByDescending(e => e).Select(e => new SelectListItem() { Value = e.ToString(), Text = e.ToString() });

            return View(vm);
        }

        [HttpPost]
        public virtual ActionResult Cases(string year)
        {
            int value;
            if (Int32.TryParse(year, out value))
                return RedirectToAction(ActionNames.Cases, new { year = value });
            else
                return RedirectToAction(ActionNames.Cases);
        }
    }
}