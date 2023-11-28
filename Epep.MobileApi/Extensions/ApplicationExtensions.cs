using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using Epep.Core.Services;
using Epep.Core.ViewModels.Common;
using Epep.MobileApi.Services;
using IO.PaymentProvider.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Epep.MobileApi.Extensions
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

            //TODO
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

            //services.AddScoped<IUserClaimsPrincipalFactory<UserRegistration>, ApplicationClaimsPrincipalFactory>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICaseService, CaseService>();
            services.AddScoped<ISummonService, SummonService>();
            //services.AddScoped<IDocumentService, DocumentService>();
            //services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<INomenclatureService, NomenclatureService>();
            //services.AddScoped<IPricelistService, PricelistService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            //services.AddScoped<IAdministrativeService, AdministrativeService>();
            services.AddScoped<IBlobService, BlobServiceSql>();
            services.Configure<EmailConfigModel>(configuration.GetSection("EmailConfig"));
            services.Configure<SMTPConfigModel>(configuration.GetSection("SMTPConfig"));
            services.AddScoped<IMailSenderService, MailSenderService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserContext, MobileUserContext>();
            //services.AddScoped<IRegixService, RegixService>();
            //services.AddScoped<IPaymentProviderClient, PaymentProviderClient>();
            //services.AddScoped<IPaymentService, PaymentService>();
            //services.AddScoped<IReportService, ReportService>();
            //services.AddScoped<IMigrationService, MigrationService>();

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

            //VerificationServiceOptions vsOptions = new VerificationServiceOptions()
            //{
            //    Token = configuration.GetValue<string>("Authentication:StampIT:VerificationService:Token"),
            //    VerificationServiceEndpoint = configuration.GetValue<string>("Authentication:StampIT:VerificationService:VerificationServiceEndpoint"),
            //    ClientId = configuration.GetValue<string>("Authentication:StampIT:VerificationService:ClientId")
            //};

            //if (!string.IsNullOrEmpty(configuration.GetValue<string>("Authentication:StampIT:Timestamp:Token")))
            //{
            //    services.AddIoTimestampClient(options =>
            //    {
            //        options.Token = configuration.GetValue<string>("Authentication:StampIT:Timestamp:Token");
            //        options.TimestampEndpoint = configuration.GetValue<string>("Authentication:StampIT:Timestamp:TimestampEndpoint");
            //        options.ValidateEndpoint = configuration.GetValue<string>("Authentication:StampIT:Timestamp:ValidateEndpoint");
            //    });
            //}

            //services.AddIOSignTools(options =>
            //{
            //    //options.TempDir = Configuration.GetValue<string>("TempPdfDir");
            //    options.HashAlgorithm = System.Security.Cryptography.HashAlgorithmName.SHA256.Name;
            //    //options.TimestampOptions = tsOptions;
            //    options.VerificationServiceOptions = vsOptions;
            //});
            //services.AddScoped<IIOSignToolsService, IOSignToolsService>();
            //services.AddScoped<ITimestampClient, IO.Timestamp.Client.TimestampClient>();
            //services.AddScoped<ITempFileHandler, SignToolsTempFileHelper>();

            //services.AddIoRegixClient(options =>
            //{
            //    options.CertificatePath = configuration.GetValue<string>("Regix:Certificate");
            //    options.Password = configuration.GetValue<string>("Regix:Password");
            //    options.ClientType = configuration.GetValue<bool>("Regix:IsInProduction") ? ClientType.Production : ClientType.Test;
            //});
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
