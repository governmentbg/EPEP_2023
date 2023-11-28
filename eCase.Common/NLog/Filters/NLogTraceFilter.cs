using System.Web.Mvc;
using NLog;

namespace eCase.Common.NLog.Filters
{
    public class NLogTraceFilter : System.Web.Mvc.ActionFilterAttribute
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Logger.Info(string.Empty);

            base.OnActionExecuted(filterContext);
        }
    }
}
