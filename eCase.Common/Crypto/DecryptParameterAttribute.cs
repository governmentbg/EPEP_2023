using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace eCase.Common.Crypto
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DecryptParameterAttribute : ActionFilterAttribute
    {
        public string IdParamName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionParameters.ContainsKey(IdParamName))
            {
                var idValue = filterContext.ActionParameters[IdParamName].ToString();

                if (!string.IsNullOrEmpty(idValue))
                {
                    filterContext.ActionParameters[IdParamName] = ConfigurationBasedStringEncrypter.Decrypt(idValue);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DecryptParametersAttribute : ActionFilterAttribute
    {
        public string[] IdsParamName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            foreach (var IdParamName in IdsParamName)
            {
                if (filterContext.ActionParameters.ContainsKey(IdParamName))
                {
                    var idValue = filterContext.ActionParameters[IdParamName].ToString();

                    if (!string.IsNullOrEmpty(idValue))
                    {
                        filterContext.ActionParameters[IdParamName] = ConfigurationBasedStringEncrypter.Decrypt(idValue);
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}