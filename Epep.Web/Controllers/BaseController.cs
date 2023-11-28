using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;

namespace Epep.Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Текущ контекст на промените за даден обект
        /// </summary>
        ActionExecutedContext lastContext;
        string lastClientIP;
        protected int Audit_Operation;
        protected string Audit_Object;
        protected string Audit_Action;
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            lastContext = context;
            lastClientIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (Request.Headers.TryGetValue("X-Forwarded-For", out var currentIp))
            {
                lastClientIP = currentIp;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("bg");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("bg");
        }

        protected override void Dispose(bool disposing)
        {
            if (Audit_Operation > 0)
            {
                var requestUrl = $"{lastContext.HttpContext.Request.Path}{lastContext.HttpContext.Request.QueryString}";
                var auditService = (IAuditLogService)HttpContext.RequestServices.GetService(typeof(IAuditLogService));
                var auditSave = auditService.SaveAuditLog(Audit_Operation, Audit_Object, lastClientIP, requestUrl, Audit_Action).Result;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Обработка на антиспам защита - Google ReCaptcha
        /// </summary>
        /// <param name="gRecaptchaResponse"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        protected async Task<bool> ReCaptchaPassed(string gRecaptchaResponse, string secretKey)
        {
            HttpClient httpClient = new HttpClient();
            var res = await httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={gRecaptchaResponse}");

            if (res.StatusCode != HttpStatusCode.OK)
                return false;

            string JSONres = await res.Content.ReadAsStringAsync();
            dynamic JSONdata = JObject.Parse(JSONres);

            if (JSONdata.success != "true" || JSONdata.score <= 0.5m)
                return false;

            return true;
        }

        protected void SetSaveResultMessage(SaveResultVM result)
        {
            if (result == null)
            {
                SetSaveResultMessage(new SaveResultVM(false));
            }
            if (result.Result)
            {
                SetSuccessMessage(result.Message);
            }
            else
            {
                SetErrorMessage(result.Message);
            }
        }

        protected void AuditFromSaveResult(SaveResultVM result, bool isInsert, string objectInfo)
        {
            if (result == null || !result.Result)
            {
                return;
            }
            Audit_Operation = isInsert ? NomenclatureConstants.AuditOperations.Append : NomenclatureConstants.AuditOperations.Update;
            Audit_Object = objectInfo;
            Audit_Action = result.AuditInfo;

            result.AuditInfo = null;
        }

        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }
        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }
    }
}
