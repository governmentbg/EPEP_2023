using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace eCase.Web.Helpers
{
    public class SecretCheckerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // TODO find solution
            base.OnActionExecuting(actionContext);
        }
    }
}