using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using Epep.Core.Services;
using Epep.Core.ViewModels.Common;
using Epep.Web.Models;
using IO.PaymentProvider.Models;
using IO.RegixClient;
using IO.SignTools.Contracts;
using IO.SignTools.Extensions;
using IO.SignTools.Models;
using IO.SignTools.Services;
using IO.Timestamp.Client.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;

namespace Epep.Web.Extensions
{
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Регистриране на контекста за достъп до базата данни
        /// </summary>
        /// <param name="builder"></param>
        public static void AddDbContext(this WebApplicationBuilder builder)
        {
            IConfiguration Configuration = builder.Configuration;
            var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            //builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddScoped<IRepository, Repository>();
        }

        private static Task HandleRemoteFailure(RemoteFailureContext context, string certErrorPath = "/home/logincerterror?error=")
        {
            context.Response.Redirect($"{certErrorPath}{context.Failure}");
            context.HandleResponse();

            return Task.FromResult(0);
        }

        /// <summary>
        /// Регистриране на модела за автентикация
        /// </summary>
        /// <param name="builder"></param>
        public static void AddApplicationAuthentication(this WebApplicationBuilder builder)
        {
            IConfiguration Configuration = builder.Configuration;

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }
            )
            .AddCookie()
            .AddStampIT(options =>
            {
                options.AppId = Configuration.GetValue<string>("Authentication:StampIT:AppId");
                options.AppSecret = Configuration.GetValue<string>("Authentication:StampIT:AppSecret");
                options.Scope.Add("pid");
                options.Scope.Add("organization");
                options.ClaimActions.DeleteClaim(ClaimTypes.NameIdentifier);
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "pid");
                options.ClaimActions.MapJsonKey(CustomClaimTypes.OrganizationUic, "org");
                options.ClaimActions.MapJsonKey(CustomClaimTypes.IdStampit.CertificateNumber, "certno");
                var callbackPath = Configuration.GetValue<string>("Authentication:StampIT:CallbackPath");
                if (!string.IsNullOrEmpty(callbackPath))
                    options.CallbackPath = callbackPath;
                options.AuthorizationEndpoint = Configuration.GetValue<string>("Authentication:StampIT:AuthorizationEndpoint");
                options.TokenEndpoint = Configuration.GetValue<string>("Authentication:StampIT:TokenEndpoint");
                options.UserInformationEndpoint = Configuration.GetValue<string>("Authentication:StampIT:UserInformationEndpoint");
                options.Events = new OAuthEvents()
                {
                    OnRemoteFailure = context => HandleRemoteFailure(context, Configuration.GetValue<string>("Authentication:StampIT:CertErrorPath"))
                };
            });

            builder.Services.AddDefaultIdentity<UserRegistration>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            int cookieMaxAgeMinutes = Configuration.GetValue<int>("Authentication:CookieMaxAgeMinutes", 30);
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieMaxAgeMinutes);
                options.LoginPath = "/user/login";
                options.AccessDeniedPath = "/home/accessdenied";
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(GlobalAdminPolicyRequirement.Name, policy =>
                   policy.Requirements.Add(new GlobalAdminPolicyRequirement()));

                options.AddPolicy(OrganizationRepresentativePolicyRequirement.Name, policy =>
                   policy.Requirements.Add(new OrganizationRepresentativePolicyRequirement()));

                options.AddPolicy(LawyerPolicyRequirement.Name, policy =>
                  policy.Requirements.Add(new LawyerPolicyRequirement()));
            });
        }

        /// <summary>
        /// Регистриране на Dependancy Injection контейнера
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddScoped<IUserClaimsPrincipalFactory<UserRegistration>, ApplicationClaimsPrincipalFactory>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICaseService, CaseService>();
            services.AddScoped<ISummonService, SummonService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<INomenclatureService, NomenclatureService>();
            services.AddScoped<IPricelistService, PricelistService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IAdministrativeService, AdministrativeService>();
            services.AddScoped<IBlobService, BlobServiceSql>();
            services.Configure<EmailConfigModel>(configuration.GetSection("EmailConfig"));
            services.Configure<SMTPConfigModel>(configuration.GetSection("SMTPConfig"));
            services.Configure<RecaptchaOptions>(configuration.GetSection("RecaptchaOptions"));
            services.AddScoped<IMailSenderService, MailSenderService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IRegixService, RegixService>();
            services.AddScoped<IPaymentProviderClient, PaymentProviderClient>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IMigrationService, MigrationService>();

            services.AddRazorTemplating();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;

                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        public static void ConfigureIOServices(this IServiceCollection services, IConfiguration configuration)
        {
            //TimestampClientOptions tsOptions = null;

            //if (!string.IsNullOrEmpty(Configuration.GetValue<string>("Authentication:StampIT:Timestamp:Token")))
            //{
            //    tsOptions = new TimestampClientOptions()
            //    {
            //        Token = Configuration.GetValue<string>("Authentication:StampIT:Timestamp:Token"),
            //        TimestampEndpoint = Configuration.GetValue<string>("Authentication:StampIT:Timestamp:TimestampEndpoint"),
            //        ValidateEndpoint = Configuration.GetValue<string>("Authentication:StampIT:Timestamp:ValidateEndpoint")
            //    };
            //}

            VerificationServiceOptions vsOptions = new VerificationServiceOptions()
            {
                Token = configuration.GetValue<string>("Authentication:StampIT:VerificationService:Token"),
                VerificationServiceEndpoint = configuration.GetValue<string>("Authentication:StampIT:VerificationService:VerificationServiceEndpoint"),
                ClientId = configuration.GetValue<string>("Authentication:StampIT:VerificationService:ClientId")
            };

            if (!string.IsNullOrEmpty(configuration.GetValue<string>("Authentication:StampIT:Timestamp:Token")))
            {
                services.AddIoTimestampClient(options =>
                {
                    options.Token = configuration.GetValue<string>("Authentication:StampIT:Timestamp:Token");
                    options.TimestampEndpoint = configuration.GetValue<string>("Authentication:StampIT:Timestamp:TimestampEndpoint");
                    options.ValidateEndpoint = configuration.GetValue<string>("Authentication:StampIT:Timestamp:ValidateEndpoint");
                });
            }

            services.AddIOSignTools(options =>
            {
                //options.TempDir = Configuration.GetValue<string>("TempPdfDir");
                options.HashAlgorithm = System.Security.Cryptography.HashAlgorithmName.SHA256.Name;
                //options.TimestampOptions = tsOptions;
                options.VerificationServiceOptions = vsOptions;
            });
            services.AddScoped<IIOSignToolsService, IOSignToolsService>();
            services.AddScoped<ITimestampClient, IO.Timestamp.Client.TimestampClient>();
            services.AddScoped<ITempFileHandler, SignToolsTempFileHelper>();

            services.AddIoRegixClient(options =>
            {
                options.CertificatePath = configuration.GetValue<string>("Regix:Certificate");
                options.Password = configuration.GetValue<string>("Regix:Password");
                options.ClientType = configuration.GetValue<bool>("Regix:IsInProduction") ? ClientType.Production : ClientType.Test;
            });
        }

        /// <summary>
        /// Регистрира HttpClient-ите и сертификатите към тях
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        /// <param name="Configuration">Настройки на приложението</param>
        public static void ConfigureHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services.ConfigurePaymentProviderHttpClients();
        }

        public static async Task<string> RenderPartialViewAsync<TModel>(this Controller controller, string execPath, string viewName, TModel model, bool partial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.GetView(execPath, viewName, !partial);

                if (viewResult.Success == false)
                {
                    return $"A view with the name {viewName} could not be found";
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                try
                {
                    return writer.GetStringBuilder().ToString();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

    }
}
