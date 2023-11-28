using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.User;
using Epep.Web.Extensions;
using Epep.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using QRCoder;
using System.Security.Claims;

namespace Epep.Web.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly UserManager<UserRegistration> userManager;
        private readonly SignInManager<UserRegistration> signInManager;
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;
        private readonly RecaptchaOptions recaptcha;
        private readonly IUserContext userContext;
        private readonly IConfiguration configuration;
        private readonly IBlobService blobService;
        private readonly IEmailService emailService;

        private readonly bool DISABLE_REGISTRATION;

        public UserController(
            UserManager<UserRegistration> _userManager,
            SignInManager<UserRegistration> _signInManager,
            ILogger<UserController> _logger,
            IUserService _userService,
            IOptions<RecaptchaOptions> _recaptcha,
            IUserContext userContext,
            IConfiguration configuration,
            IBlobService blobService,
            IEmailService emailService)
        {
            this.userManager = _userManager;
            this.signInManager = _signInManager;
            this.logger = _logger;
            userService = _userService;
            this.recaptcha = _recaptcha.Value;
            this.userContext = userContext;
            this.configuration = configuration;
            this.blobService = blobService;

            this.DISABLE_REGISTRATION = configuration.GetValue<bool>("DisableRegistration", false);
            this.emailService = emailService;
        }

        [AllowAnonymous]
        public IActionResult RegistrationCheck()
        {
            var redirectUrl = Url.Action("ExternalLoginCallback");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(NomenclatureConstants.LoginProperties.StampItProvider, redirectUrl);

            properties.SetString(NomenclatureConstants.LoginProperties.EntityType, NomenclatureConstants.UserTypes.Registration.ToString());

            return new ChallengeResult(NomenclatureConstants.LoginProperties.StampItProvider, properties);
        }

        [AllowAnonymous]
        public IActionResult Registration()
        {
            if (string.IsNullOrEmpty(userContext.Identifier))
            {
                return RedirectToAction(nameof(Login), new { error = "Моля, изберете валиден сертификат." });
            }
            var model = new UserRegistrationVM()
            {
                UserType = NomenclatureConstants.UserTypes.Person,
                EGN = userContext.Identifier,
                UIC = userContext.OrganizationUIC,
                Email = userContext.Email,
                RepresentativeEGN = userContext.Identifier,
                RepresentativeEmail = userContext.Email
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registration(UserRegistrationVM model, ICollection<IFormFile> orgFile)
        {
            if (!(await ReCaptchaPassed(model.reCaptchaToken, recaptcha.SecretKey)))
            {
                //ModelState.AddModelError(string.Empty, "Невалидна антиспам защита");
                ModelState.AddModelError(nameof(UserRegistrationVM.Gid), "Невалидна антиспам защита");
            }

            if (DISABLE_REGISTRATION)
            {
                ModelState.AddModelError(nameof(UserRegistrationVM.Gid), "Достъпът до регистриране е временно ограничен.");
            }

            model.EGN = userContext.Identifier;
            model.UIC = userContext.OrganizationUIC;
            model.RepresentativeEGN = userContext.Identifier;
            model.RegCertificateInfo = userContext.RegCertInfo;

            var validationResult = await userService.ValidateUserRegistration(model);
            if (!validationResult.Result)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.Control ?? nameof(UserRegistrationVM.Gid), error.Error);
                }
            }


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registrationResult = await createPublicRegistrations(model);

            if (registrationResult.Result)
            {
                await signInManager.SignOutAsync();
                var newUser = await userService.GetByIdAsync<UserRegistration>(registrationResult.ObjectId);
                if (newUser != null && model.UserType == NomenclatureConstants.UserTypes.Person)
                {
                    await signInManager.SignInAsync(newUser, true);
                }
                if (model.UserType == NomenclatureConstants.UserTypes.Organization && orgFile != null && orgFile?.Count > 0)
                {
                    await uploadUserRegistrationFile(orgFile.First(), registrationResult);
                }

                if (model.UserType == NomenclatureConstants.UserTypes.Person)
                {
                    await emailService.NewMailMessage(newUser.Email, NomenclatureConstants.EmailTemplates.NewUserRegistrationMessage, JObject.FromObject(
                        new
                        {
                            isActivated = newUser.IsActive,
                            email = newUser.Email,
                            courtName = "",
                            name = newUser.FullName
                        }));
                }
                return View("RegistrationComplete", model.UserType);
            }
            else
            {
                ModelState.AddModelError("", registrationResult.Message);
                return View(model);
            }
        }

        private async Task uploadUserRegistrationFile(IFormFile file, SaveResultVM userResult)
        {
            BlobInfo fileResult;
            var fileName = Path.GetFileName(file.FileName);
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileResult = await blobService.AppendUpdateAttachedDocumentFile(NomenclatureConstants.AttachedTypes.UserRegistration, (long)userResult.ParentId, ms.ToArray(), fileName, false, "Документ за упълномощяване");
                fileResult = await blobService.AppendUpdateAttachedDocumentFile(NomenclatureConstants.AttachedTypes.UserRegistration, (long)userResult.ObjectId, ms.ToArray(), fileName, false, "Документ за упълномощяване");
            }
        }

        [AllowAnonymous]
        private async Task<SaveResultVM> createPublicRegistrations(UserRegistrationVM model)
        {
            switch (model.UserType)
            {
                case NomenclatureConstants.UserTypes.Person:
                    var personUser = model.MapToEntity();
                    IdentityResult resPerson = await userManager.CreateAsync(personUser);
                    return new SaveResultVM()
                    {
                        Result = resPerson.Succeeded,
                        Message = "Моля, влезте в профила си като използвате КЕП с посочените от Вас данни.",
                        ObjectId = personUser.Id
                    };
                case NomenclatureConstants.UserTypes.Organization:
                    if (model.ExistingOrganizationId > 0)
                    {
                        var representativeUser = model.MapRepresentativeToEntity(model.ExistingOrganizationId.Value);
                        IdentityResult resRepresentative = await userManager.CreateAsync(representativeUser);

                        if (resRepresentative.Succeeded)
                        {
                            return new SaveResultVM()
                            {
                                Result = true,
                                Message = "Подадените от Вас данни са приети и ще бъдат потвърдени от администратор.",
                                ParentId = model.ExistingOrganizationId,
                                ObjectId = representativeUser.Id
                            };
                        }
                    }
                    else
                    {
                        var newOrganization = model.MapToEntity();
                        IdentityResult resOrganization = await userManager.CreateAsync(newOrganization);
                        if (resOrganization.Succeeded)
                        {
                            var representativeUser = model.MapRepresentativeToEntity(newOrganization.Id);
                            IdentityResult resRepresentative = await userManager.CreateAsync(representativeUser);

                            if (resRepresentative.Succeeded)
                            {
                                return new SaveResultVM()
                                {
                                    Result = true,
                                    Message = "Подадените от Вас данни са приети и ще бъдат потвърдени от администратор.",
                                    ParentId = newOrganization.Id,
                                    ObjectId = representativeUser.Id
                                };
                            }
                        }
                    }
                    return new SaveResultVM(false);
                default:
                    break;
            }
            return new SaveResultVM(false);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null, string error = null, string success = null)
        {
            var model = new LoginVM
            {
                ReturnUrl = returnUrl
            };
            var externalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (externalLogins.Any())
            {
                model.ExternalProvider = externalLogins.First().Name;
            }

            ViewBag.errorMessage = error;
            ViewBag.successMessage = success;
            ViewBag.adminMode = configuration.GetValue<bool>("AdminMode", false);

            return View("LoginKEP", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await userService.FindByUIC(model.LoginUserType, model.UIC, model.OrganizationUIC);
            if (user == null)
            {
                ViewBag.errorMessage = "Невалиден потребител";
                return View(model);
            }

            if (!configuration.GetValue<bool>("AdminMode", false) && model.LoginUserType == NomenclatureConstants.UserTypes.Administrator)
            {
                return RedirectToAction(nameof(Login), new { returnUrl = model.ReturnUrl, error = "Невалиден тип потребител!" });
            }


            user.LoginUserType = model.LoginUserType;
            await signInManager.SignInAsync(user, isPersistent: true);
            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Изход от системата
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LogOff()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Вход чрез външен доставчик на ауторизация - вход с КЕП
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(LoginVM model)
        {

            var invalidUserType = configuration.GetValue<bool>("AdminMode", false) && !NomenclatureConstants.UserTypes.AdminTypes.Contains(model.LoginUserType)
                || !configuration.GetValue<bool>("AdminMode", false) && !NomenclatureConstants.UserTypes.PublicTypes.Contains(model.LoginUserType);

            if (invalidUserType)
            {
                return RedirectToAction(nameof(Login), new { returnUrl = model.ReturnUrl, error = "Невалиден тип потребител!" });
            }


            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", new { model.ReturnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(model.ExternalProvider, redirectUrl);

            properties.SetString(NomenclatureConstants.LoginProperties.EntityType, model.LoginUserType.ToString());

            return new ChallengeResult(model.ExternalProvider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                logger.LogError($"Error from external provider: {remoteError}");

                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                logger.LogError("Error loading external login information.");

                return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl });
            }

            UserRegistration user = null;
            if (info.LoginProvider == NomenclatureConstants.LoginProperties.StampItProvider)
            {
                var entityType = int.Parse(info.AuthenticationProperties.GetString(NomenclatureConstants.LoginProperties.EntityType));
                if (entityType == NomenclatureConstants.UserTypes.Registration)
                {
                    return await loginForRegistration(info);
                }
                var organizationUic = "";
                if (entityType == NomenclatureConstants.UserTypes.Organization)
                {
                    organizationUic = info.Principal.Claims.Where(x => x.Type == CustomClaimTypes.OrganizationUic)
                                                    .Select(c => c.Value).FirstOrDefault();
                    if (string.IsNullOrEmpty(organizationUic))
                    {
                        return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl, error = "КЕП на потребител не съдържа данни за избраната организация" });
                    }
                }
                user = await userService.FindByUIC(entityType, info.ProviderKey, organizationUic);
                if (user == null || !user.IsActive)
                {
                    return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl, error = "Невалиден потребител" });
                }
                user.LoginUserType = entityType;
                user.CertNo = info.Principal.Claims.Where(x => x.Type == CustomClaimTypes.IdStampit.CertificateNumber)
                                                    .Select(c => c.Value).FirstOrDefault();

            }

            if (user != null)
            {

                await signInManager.SignInAsync(user, isPersistent: true);
                switch (user.UserTypeId)
                {
                    case NomenclatureConstants.UserTypes.GlobalAdmin:
                        return RedirectToAction("Desktop", "Admin");
                    case NomenclatureConstants.UserTypes.CourtAdmin:
                        //Няма специализиран десктоп за администратори на съд
                        return RedirectToAction("Index", "Home");
                    default:
                        return RedirectToAction("Desktop", "Case");
                }
            }

            return RedirectToAction("AccessDenied");
        }

        private async Task<IActionResult> loginForRegistration(ExternalLoginInfo info)
        {
            var registerUser = new UserRegistration()
            {
                EGN = info.ProviderKey,
                FullName = info.Principal.GetClaim(ClaimTypes.Name),
                Email = info.Principal.GetClaim(ClaimTypes.Email),
                LoginUserType = NomenclatureConstants.UserTypes.Registration,
                UIC = info.Principal.GetClaim(CustomClaimTypes.OrganizationUic)
            };

            registerUser.RegCertificateInfo = $"{registerUser.FullName}, {registerUser.Email}, {info.Principal.GetClaim(CustomClaimTypes.IdStampit.CertificateNumber)}";
            await signInManager.SignInAsync(registerUser, isPersistent: false);
            return RedirectToAction(nameof(Registration));
        }


        public async Task<IActionResult> Profile()
        {
            var model = await userService.SelectDataForProfile();

            return View(model);
        }

        public async Task<IActionResult> ProfileData()
        {
            var model = await userService.SelectDataForProfile();
            return PartialView("_ProfileData", model);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileData(UserRegistrationVM model)
        {
            if (model == null)
            {
                return Json(new SaveResultVM(false));
            }

            var result = new SaveResultVM();
            if (!ModelState.IsValid)
            {
                ModelState.AppendErrors(result);
                return Json(result);
            }

            if (string.IsNullOrEmpty(model.FullName))
            {
                result.AddError("Невалидно име", nameof(model.FullName));
                return Json(result);
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                result.AddError("Невалидна електронна поща", nameof(model.Email));
                return Json(result);
            }

            var profileUser = await userService.GetByIdAsync<UserRegistration>(userContext.UserId);
            profileUser.FullName = model.FullName;
            profileUser.Email = model.Email;
            profileUser.NotificationEmail = model.NotificationEmail;
            profileUser.ModifyDate = DateTime.Now;
            int currentUserType = userContext.UserType;
            var userUpdateResult = await userManager.UpdateAsync(profileUser);
            if (userUpdateResult.Succeeded)
            {
                await signInManager.SignOutAsync();
                var loginUser = await userService.FindByUIC(currentUserType, profileUser.EGN, profileUser.UIC);
                loginUser.LoginUserType = currentUserType;
                await signInManager.SignInAsync(loginUser, true);
                return Json(new SaveResultVM(true));
            }
            else
            {
                return Json(new SaveResultVM(false));
            }
        }

        /// <summary>
        /// Обработка на грешка при избор на КЕП
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginCertError(string error)
        {
            //logger.LogError(error);
            return RedirectToAction(nameof(Login), new { error = "Моля изберете валиден сертификат." });
        }

        [AllowAnonymous]
        public IActionResult GetCode()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCodePng = new PngByteQRCode(qrCodeData);

            byte[] qrCodeImagePng = qrCodePng.GetGraphic(5, new byte[] { 0, 0, 0}, new byte[] { 255, 255, 255 });
            return File(qrCodeImagePng, "image/png");
        }
    }
}
