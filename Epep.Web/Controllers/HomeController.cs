using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.ViewModels.Common;
using Epep.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Epep.Web.Controllers
{

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> logger;
        private readonly RecaptchaOptions recaptcha;
        private readonly IMailSenderService mailsenderService;
        private readonly INomenclatureService nomService;
        private readonly IEmailService emailService;

        public HomeController(
            ILogger<HomeController> _logger,
            IOptions<RecaptchaOptions> _recaptcha,
            IMailSenderService mailsenderService,
            INomenclatureService nomService,
            IEmailService emailService
            )
        {
            logger = _logger;
            recaptcha = _recaptcha.Value;
            this.mailsenderService = mailsenderService;
            this.nomService = nomService;
            this.emailService = emailService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

       
        public async Task<IActionResult> Courts()
        {
            var model = await nomService.Get_CourtList();
            return View(model);
        }
        public IActionResult AccessibilityPolicy()
        {
            return View();
        }
       
        public IActionResult Feedback()
        {
            SetViewbagFeedback();
            return View(new FeedbackVM());
        }

        void SetViewbagFeedback()
        {
            ViewBag.Type_ddl = new List<SelectListItem>()
            {
               new SelectListItem { Value = "0", Text = "Изберете", },
               new SelectListItem { Value = "1", Text = "Въпрос", },
               new SelectListItem { Value = "2", Text = "Предложение", },
               new SelectListItem { Value = "3", Text = "Технически проблем", }
            };
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(FeedbackVM model)
        {
            if (!(await ReCaptchaPassed(model.reCaptchaToken, recaptcha.SecretKey)))
            {
                ModelState.AddModelError(string.Empty, "Невалидна антиспам защита");
            }

            if (!ModelState.IsValid)
            {
                SetViewbagFeedback();
                return View(model);
            }

            await emailService.NewMailMessage(mailsenderService.GetFeedbackMail(), NomenclatureConstants.EmailTemplates.FeedbackMessage,
                JObject.FromObject(
                    new
                    {
                        type = model.TypeName,
                        name = model.Name,
                        email = model.Email,
                        message = model.Message
                    }));

            SetSuccessMessage("Вашето съобщение беше прието успешно.");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //public IActionResult Courts()
        //{
        //    return View();
        //}
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult NotFound()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginCertError(string error)
        {
            logger.LogError(error);
            var model = new ErrorViewModel()
            {
                Message = "Моля изберете валиден сертификат."
            };
            return View(nameof(Error), model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature.Error.Message;
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = error,
                //InnerException = feature.Error.InnerException?.Message
                InnerException = feature.Error.ToString()
            };
            //if (feature.Error is NotFoundException)
            //{
            //    errorModel.Title = "Ненамерен ресурс";
            //}
            return View(errorModel);
        }
    }
}