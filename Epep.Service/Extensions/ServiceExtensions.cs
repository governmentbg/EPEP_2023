using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using Epep.Core.Services;
using Epep.Core.ViewModels.Common;
using IO.QuartzExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Epep.Service.Extensions
{
    internal static class ServiceExtensions
    {
        /// <summary>
        /// Регистриране на контекста за достъп до базата данни
        /// </summary>
        /// <param name="builder"></param>
        internal static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ApplicationDbContext");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IRepository, Repository>();
        }


        /// <summary>
        /// Регистриране на Dependancy Injection контейнера
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<INomenclatureService, NomenclatureService>();
            services.AddScoped<ILawyerRegisterService, LawyerRegisterService>();
            services.AddScoped<IDocumentCleanerService, DocumentCleanerService>();
            services.Configure<EmailConfigModel>(configuration.GetSection("EmailConfig"));
            services.Configure<SMTPConfigModel>(configuration.GetSection("SMTPConfig"));
            services.AddScoped<IMailSenderService, MailSenderService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddMvcCore().AddRazorRuntimeCompilation();
            services.AddRazorTemplating();

            services.AddQuartConfiguration(configuration);
        }

        /// <summary>
        /// Регистрира HttpClient-ите и сертификатите към тях
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        /// <param name="Configuration">Настройки на приложението</param>
        internal static void ConfigureHttpClients(this IServiceCollection services, IConfiguration config)
        {

            //Адвокатски регистър
            services.AddHttpClient(LawyerRegisterService.HttpClientName, client =>
            {
                client.Timeout = TimeSpan.FromMinutes(2);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var certificatePath = config.GetValue<string>("LawyerRegister:CertificatePath");
                var certificatePassword = config.GetValue<string>("LawyerRegister:CertificatePassword");
                HttpClientHandler result = new HttpClientHandler();
                if (!string.IsNullOrEmpty(certificatePath))
                {
                    //if (OperatingSystem.IsWindows())
                    //{
                    //    string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
                    //    filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
                    //    certificatePath = Path.Combine(filePath, certificatePath);
                    //}

                    var _cert = new X509Certificate2(certificatePath, certificatePassword);
                    var addCertificateInStore = config.GetValue<bool>("AddCertificateInStore", false);
                    if (addCertificateInStore)
                    {
                        appendCertificateToStore(_cert);
                    }
                    result.ClientCertificates.Add(_cert);
                    result.ClientCertificateOptions = ClientCertificateOption.Manual;
                    result.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                }
                return result;
            });


        }

        static void appendCertificateToStore(X509Certificate2 certificate)
        {
            try
            {
                using (X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    if (!store.Certificates.Contains(certificate))
                    {
                        store.Open(OpenFlags.ReadWrite);
                        store.Add(certificate);
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
