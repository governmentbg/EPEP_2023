using Epep.Core.Contracts;
using Microsoft.Extensions.Logging;
using Quartz;
using static IO.QuartzExtensions.BaseQuartzExtensions;

namespace Epep.Service.Jobs
{

    /// <summary>
    /// Услуга за проверка и изпращане на чакащите мейли
    /// </summary>
    [DisallowConcurrentExecution]
    public class EmailSenderJob : BaseJob
    {
        private readonly IEmailService service;


        public EmailSenderJob(
            IEmailService _service,
            ILogger<EmailSenderJob> _logger)
        {
            service = _service;
            logger = _logger;
        }
        protected override async Task DoJob(IJobExecutionContext context)
        {
            try
            {
                await service.SendMails();
            }catch(Exception ex) { }
        }
    }
}
