using eCase.Common.Captcha;
using eCase.Common.Helpers;
using eCase.Common.NLog;
using eCase.Components.Communicators.HelpDeskCommunicator;
using eCase.Data.Core;
using eCase.Data.Repositories;
using eCase.Domain.Emails;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VSS.Portal.Models.Feedback;

namespace VSS.Portal.Controllers
{
    public partial class FeedbackController : Controller
    {
        private IMailRepository _mailRepository;
        private IUnitOfWork _unitOfWork;
        private IHelpDeskCommunicator _helpDeskCommunicator;
        private ILogger _logger;

        public FeedbackController(IUnitOfWork unitOfWork, IMailRepository mailRepository,
            IHelpDeskCommunicator helpDeskCommunicator, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _mailRepository = mailRepository;
            _helpDeskCommunicator = helpDeskCommunicator;
            _logger = logger;
        }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidation(Constants.CaptchaModelName)]
        public virtual ActionResult Index(FeedbackVM vm, bool? captchaValid)
        {
            if (captchaValid.HasValue && !captchaValid.Value)
            {
                ModelState.AddModelError(Constants.CaptchaModelName, "Невалиден код за сигурност.");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            string subjectText = String.Empty;
            switch (vm.Type)
            {
                case "1":
                    subjectText = "Въпрос";
                    break;
                case "2":
                    subjectText = "Предложение";
                    break;
                case "3":
                    subjectText = "Технически проблем";
                    break;
                default:
                    break;
            }

            subjectText += " ВСС (обратна връзка)";

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                foreach (var recipient in ConfigurationManager.AppSettings["VSS.Portal:FeedbackEmail"].Split(','))
                {
                    Email email = new Email(
                    recipient,
                    Constants.FeedbackMessage,
                    JObject.FromObject(
                        new
                        {
                            type = subjectText,
                            name = vm.Name,
                            email = vm.Email,
                            message = vm.Message
                        }));

                    _mailRepository.Add(email);
                }

                _unitOfWork.Save();
                transaction.Commit();
            }

            Task.Run(() =>
            {
                try
                {
                    SendFeedbackToHelpDesk(subjectText, vm.Name, vm.Email, vm.Message);
                } catch (Exception ex)
                {
                    _logger.Error(Helper.GetDetailedExceptionInfo(ex));
                }
                
            });

            return View(Views.FeedbackSuccess);
        }

        [NonAction]
        private void SendFeedbackToHelpDesk(string subject, string name, string email, string description)
        {
            //var access_token = _helpDeskCommunicator.Login(
            //    ConfigurationManager.AppSettings["eCase.Web:HelpDeskDomain"],
            //    ConfigurationManager.AppSettings["eCase.Web:HelpDeskClientId"],
            //    ConfigurationManager.AppSettings["eCase.Web:HelpDeskSecret"],
            //    ConfigurationManager.AppSettings["eCase.Web:HelpDeskUsername"],
            //    ConfigurationManager.AppSettings["eCase.Web:HelpDeskPassword"]
            //    );

            //_helpDeskCommunicator.Send(
            //    ConfigurationManager.AppSettings["eCase.Web:HelpDeskDomain"],
            //    access_token,
            //    subject,
            //    name,
            //    email,
            //    description
            //    );
        }
    }
}