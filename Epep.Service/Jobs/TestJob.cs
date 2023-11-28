using Microsoft.Extensions.Logging;
using Quartz;
using static IO.QuartzExtensions.BaseQuartzExtensions;

namespace Epep.Service.Jobs
{

    /// <summary>
    /// Тестова услуга
    /// </summary>
    [DisallowConcurrentExecution]
    public class TestJob : BaseJob
    {

        public TestJob(
            ILogger<TestJob> _logger)
        {
            logger = _logger;
        }
        protected override async Task DoJob(IJobExecutionContext context)
        {
            logger.LogError($"TestJob {DateTime.Now:HH:mm:ss}");
        }
    }
}
