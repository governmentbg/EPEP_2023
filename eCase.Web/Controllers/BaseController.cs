using eCase.Data.Repositories;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Linq;

namespace eCase.Web.Controllers
{
    public abstract partial class BaseController : Controller
    {
        protected ISummonRepository _summonRepository;

        public BaseController(ISummonRepository summonRepository) 
        {
            _summonRepository = summonRepository;
        }

        protected override void ExecuteCore()
        {
            base.ExecuteCore();
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // prevent browser caching
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnActionExecuting(filterContext);
        }

        private eCaseUser _currentUser;
        protected eCaseUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    ClaimsIdentity ci = this.User.Identity as ClaimsIdentity;
                    _currentUser = eCaseUserManager.LoadUser(ci);
                }
                return _currentUser;
            }
        }

        private bool? _showSummons;
        public bool ShowSummons
        {
            get
            {
                if (!_showSummons.HasValue)
                { 
                    _showSummons = Request.IsAuthenticated && (CurrentUser.IsPerson || CurrentUser.IsLawyer);
                }
                return _showSummons.Value;
            }
        }

        private int? _summonsCount;
        public int SummonsCount
        {
            get
            {
                if (!_summonsCount.HasValue)
                {
                    if (ShowSummons)
                        _summonsCount = _summonRepository.GetSummonsByUserId(CurrentUser.UserID).Where(e => !e.IsRead).Count();
                    else
                        _summonsCount = 0;
                }

                return _summonsCount.Value;
            }
        }
    }
}