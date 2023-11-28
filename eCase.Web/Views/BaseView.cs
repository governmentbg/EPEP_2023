using eCase.Data.Repositories;
using System.Security.Claims;
using System.Web.Mvc;
using System.Linq;

namespace eCase.Web.Views
{
    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public BaseViewPage() { }

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
                    var _summonRepository = DependencyResolver.Current.GetService<ISummonRepository>();

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