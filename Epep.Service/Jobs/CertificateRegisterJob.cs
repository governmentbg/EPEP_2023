using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Security.Cryptography.X509Certificates;
using static IO.QuartzExtensions.BaseQuartzExtensions;

namespace Epep.Service.Jobs
{

    /// <summary>
    /// Услуга за изтегляне на данни от Регистъра на адвокатите
    /// </summary>
    [DisallowConcurrentExecution]
    public class CertificateRegisterJob : BaseJob
    {
        private readonly string certPath;
        private readonly string certPassword;
        public CertificateRegisterJob(
            IConfiguration config,
            ILogger<CertificateRegisterJob> _logger)
        {
            logger = _logger;
            certPath = config.GetValue<string>("CertificateRegister:Path");
            certPassword = config.GetValue<string>("CertificateRegister:Password");
        }
        protected override async Task DoJob(IJobExecutionContext context)
        {
            try
            {
                var cert = new X509Certificate2(certPath, certPassword);
                using (X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    if (!store.Certificates.Contains(cert))
                    {
                        store.Open(OpenFlags.ReadWrite);
                        store.Add(cert);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Certificate={certPath}@{certPassword} Error: {ex.Message} {ex.InnerException?.Message}", ex);
            }
        }
    }
}
