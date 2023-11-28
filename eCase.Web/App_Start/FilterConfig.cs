using eCase.Common.NLog.Filters;
using eCase.Web.Helpers;
using System.Web;
using System.Web.Mvc;

namespace eCase.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new NLogExceptionFilter());
            filters.Add(new NLogTraceFilter());

            filters.Add(new AjaxExceptionFilter());
        }
    }
}
