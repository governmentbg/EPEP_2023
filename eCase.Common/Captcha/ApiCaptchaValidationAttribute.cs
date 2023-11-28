using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace eCase.Common.Captcha
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ApiCaptchaValidationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCaptchaValidationAttribute"/> class.
        /// </summary>
        public ApiCaptchaValidationAttribute()
            : this("captcha") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCaptchaValidationAttribute"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        public ApiCaptchaValidationAttribute(string field)
        {
            Field = field;
        }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>The field.</value>
        public string Field { get; private set; }

        /// <summary>
        /// Called when [action executed].
        /// </summary>
        /// <param name="filterContext">The filter filterContext.</param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            #region captcha

            var captchaAttribute = Enumerable.FirstOrDefault<object>(filterContext.ActionDescriptor.GetCustomAttributes<ApiCaptchaValidationAttribute>()) as ApiCaptchaValidationAttribute;

            if (captchaAttribute != null)
            {
                string key = filterContext.ActionArguments.Keys.Single();

                ApiCaptchaModel model = filterContext.ActionArguments[key] as ApiCaptchaModel;

                // make sure no values are getting sent in from the outside
                model.captchaValid = false;
                
                // get the guid from the post back
                string guid = model.captchaGuid;
                
                // check for the guid because it is required from the rest of the opperation
                if (!String.IsNullOrEmpty(guid))
                {
                    // get values
                    CaptchaImage image = CaptchaImage.GetCachedCaptcha(guid);
                    string actualValue = model.Captcha;
                    string expectedValue = image == null ? String.Empty : image.Text;
                
                    // removes the captch from cache so it cannot be used again
                    System.Web.HttpRuntime.Cache.Remove(guid);
                
                    // validate the captcha
                    model.captchaValid =
                            !String.IsNullOrEmpty(actualValue)
                            && !String.IsNullOrEmpty(expectedValue)
                            && String.Equals(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);
                }
            }

            #endregion
        }

    }
}