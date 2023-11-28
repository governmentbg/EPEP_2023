using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eCase.Common.NLog;
using eCase.Web.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using eCase.Data.Core;
using eCase.Data.Repositories;
using eCase.Common.Captcha;
using eCase.Domain.Emails;

namespace eCase.Web.Controllers
{
    [Authorize]
    public partial class AccountController : BaseController
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private SignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<SignInManager>();
            }
        }

        private eCaseUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<eCaseUserManager>();
            }
        }

        private IUnitOfWork _unitOfWork;
        private ILogger _logger;
        private IUserRepository _userRepository;
        private IPersonRegistrationRepository _personRegistrationRepository;
        private ILawyerRegistrationRepository _lawyerRegistrationRepository;
        private IMailRepository _mailRepository;

        public AccountController(
            IUnitOfWork unitOfWork,
            ILogger logger,
            IUserRepository userRepository,
            IPersonRegistrationRepository personRegistration,
            ILawyerRegistrationRepository lawyerRegistration,
            IMailRepository mailRepository,
                ISummonRepository summonRepository)
            : base(summonRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userRepository = userRepository;
            _personRegistrationRepository = personRegistration;
            _lawyerRegistrationRepository = lawyerRegistration;
            _mailRepository = mailRepository;
        }

        #region Login/Logoff

        [AllowAnonymous]
        public virtual async Task<ActionResult> Login()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
            }

#if false
            //var user = _userRepository.Find("courtadmin@courtadmin.com");
            var user = _userRepository.Find("cborisoff@gmail.com");
            
            await SignInManager.SignInAsync(new eCaseUser()
            {
                //Id = user.UserId.ToString(),
                UserId = user.UserId.ToString(),
                Email = user.Username,
                Name = user.Name,
                CourtId = user.CourtId.HasValue ? user.CourtId.Value.ToString() : string.Empty,
                UserGroupId = user.UserGroupId.ToString() ?? string.Empty,
            
            }, false, false);
            
            return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);

            //return View(new LoginVM());
#else
            return View(new LoginVM());
#endif
        }

        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation(Constants.CaptchaModelName)]
        public virtual async Task<ActionResult> Login(LoginVM vm, bool? captchaValid)
        {
            if (captchaValid.HasValue && !captchaValid.Value)
            {
                ModelState.AddModelError(Constants.CaptchaModelName, "Невалиден код за сигурност.");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            try
            {
                var user = _userRepository.Find(vm.Email);

                if (user != null && user.IsActive && user.VerifyPassword(vm.Password))
                {
                    await SignInManager.SignInAsync(new eCaseUser()
                    {
                        UserId = user.UserId.ToString(),
                        Email = user.Username,
                        Name = user.Name,
                        CourtId = user.CourtId.HasValue ? user.CourtId.Value.ToString() : string.Empty,
                        UserGroupId = user.UserGroupId.ToString() ?? string.Empty,

                    }, false, false);
                }
                else
                {
                    ModelState.AddModelError("_form", "Грешно потребителско име или парола.");
                    return View(vm);
                }
            }
            catch
            {
                return View(MVC.Shared.Views.Failure, (object)"Възникна грешка. Моля, опитайте отново по-късно.");
            }

            TempData[Constants.IsFromLoginKey] = true;

            return RedirectToAction(MVC.Case.ActionNames.Search, MVC.Case.Name);
        }

        public virtual ActionResult Logout()
        {
            AuthenticationManager.SignOut();

            return RedirectToAction(ActionNames.Login);
        }

        #endregion

        #region ForgotPassword

        [AllowAnonymous]
        public virtual ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordVM());
        }

        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation(Constants.CaptchaModelName)]
        public virtual ActionResult ForgotPassword(ForgotPasswordVM vm, bool? captchaValid)
        {
            if (captchaValid.HasValue && !captchaValid.Value)
            {
                ModelState.AddModelError(Constants.CaptchaModelName, "Невалиден код за сигурност.");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = _userRepository.Find(vm.Email);

            if (user != null && user.IsActive)
            {
                using (var transaction = _unitOfWork.BeginTransaction())
                {
                    user.ForgottenPassword();

                    _unitOfWork.Save();
                    transaction.Commit();
                }
            }
            else
            {
                ModelState.AddModelError("_form", "В системата няма регистриран профил с посочения имейл адрес.");
                return View(vm);
            }

            return View(MVC.Account.Views.ForgotPasswordSuccess, (object)vm.Email);
        }

        [AllowAnonymous]
        public virtual ActionResult PasswordRecovery(string ac)
        {
            if (string.IsNullOrEmpty(ac))
            {
                throw new ArgumentNullException("Missing Activation Code.");
            }

            var user = _userRepository.FindByActivationCode(ac);
            if (user == null)
            {
                return View(MVC.Shared.Views.Failure, (object)"Невалиден линк за възстановяване на парола.");
            }
            else if (user.IsActivationCodeValid == false)
            {
                return View(MVC.Shared.Views.Failure, (object)"Вече сте използвали този линк за възстановяване на парола.");
            }
            else
            {
                return View(new ActivateVM() { UserGid = user.Gid });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult PasswordRecovery(ActivateVM vm)
        {
            this.CheckBirthDate(vm);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = _userRepository.FindByGid(vm.UserGid);

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                user.SetPassword(vm.Password);
                user.IsActivationCodeValid = false;
                user.IsActive = true;

                _unitOfWork.Save();
                transaction.Commit();
            }

            return View(MVC.Shared.Views.Success, (object)"Вашата парола е възстановена успешно.");
        }

        #endregion

        #region Activate

        [AllowAnonymous]
        public virtual ActionResult Activate(string ac)
        {
            if (string.IsNullOrEmpty(ac))
            {
                throw new ArgumentNullException("Missing Activation Code.");
            }

            var user = _userRepository.FindByActivationCode(ac);
            if (user == null)
            {
                return View(MVC.Shared.Views.Failure, (object)"Невалиден линк за активация.");
            }
            else if (user.IsActivationCodeValid == false)
            {
                return View(MVC.Shared.Views.Failure, (object)"Линкът за активиране на потребителски профил е деактивиран.");
            }
            else
            {
                return View(new ActivateVM() { UserGid = user.Gid });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Activate(ActivateVM vm)
        {
            this.CheckBirthDate(vm);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = _userRepository.FindByGid(vm.UserGid);

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                user.SetPassword(vm.Password);
                user.IsActivationCodeValid = false;
                user.IsActive = true;

                _unitOfWork.Save();
                transaction.Commit();
            }

            if (Request.IsAuthenticated)
            {
                AuthenticationManager.SignOut();
            }

            return View(MVC.Shared.Views.Success, (object)"Вашата регистрация е активирана успешно.");
        }

        #endregion

        #region ChangePassword

        [Authorize]
        public virtual ActionResult ChangePassword()
        {
            return View(new ChangePasswordVM());
        }

        [HttpPost]
        [Authorize]
        public virtual ActionResult ChangePassword(ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var user = _userRepository.Find(CurrentUser.Email);


                if (!user.VerifyPassword(vm.OldPassword))
                {
                    ModelState.AddModelError("_form", "Грешна стара парола.");
                    return View();
                }

                user.ChangePassword(vm.Password);

                _unitOfWork.Save();

                transaction.Commit();
            }

            return View(MVC.Shared.Views.Success, (object)"Паролата е сменена успешно.");
        }

        #endregion

        private void CheckBirthDate(ActivateVM vm)
        {
            var user = _userRepository.FindByGid(vm.UserGid);

            var lawyer = _lawyerRegistrationRepository.FindFirstOrDefault(user.UserId);

            if (lawyer != null)
            {
                DateTime pBirthDate = DateTime.MinValue;

                if (DateTime.TryParse(vm.BirthDate, out pBirthDate))
                {
                    if (!lawyer.BirthDate.Equals(pBirthDate))
                    {
                        ModelState.AddModelError("BirthDate", "Датата, която сте въвели трябва да съвпада с Вашата рождена дата.");
                    }
                }
            }
            else
            {
                var person = _personRegistrationRepository.FindFirstOrDefault(user.UserId);

                if (person != null)
                {
                    DateTime pBirthDate = DateTime.MinValue;

                    if (DateTime.TryParse(vm.BirthDate, out pBirthDate))
                    {
                        if (!person.BirthDate.Equals(pBirthDate))
                        {
                            ModelState.AddModelError("BirthDate", "Датата, която сте въвели трябва да съвпада с Вашата рождена дата.");
                        }
                    }
                }
            }
        }
    }
}