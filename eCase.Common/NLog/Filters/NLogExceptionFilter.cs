using System.Web.Mvc;
using NLog;
using eCase.Common.Helpers;

namespace eCase.Common.NLog.Filters
{
    public class NLogExceptionFilter : HandleErrorAttribute
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void OnException(ExceptionContext filterContext)
        {
            Logger.Error(Helper.GetDetailedExceptionInfo(filterContext.Exception));

            base.OnException(filterContext);
        }
    }
}
