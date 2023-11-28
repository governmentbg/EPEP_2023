using Epep.Core.Contracts;
using Microsoft.Extensions.Logging;
using Quartz;
using static IO.QuartzExtensions.BaseQuartzExtensions;

namespace Epep.Service.Jobs
{

    /// <summary>
    /// Услуга за изтегляне на данни от Регистъра на адвокатите
    /// </summary>
    [DisallowConcurrentExecution]
    public class LawyerRegisterJob : BaseJob
    {
        private readonly ILawyerRegisterService service;

        public LawyerRegisterJob(
            ILawyerRegisterService _service,
            ILogger<LawyerRegisterJob> _logger)
        {
            service = _service;
            logger = _logger;
        }
        protected override async Task DoJob(IJobExecutionContext context)
        {
            await service.FetchLawyers();
        }
    }
}
