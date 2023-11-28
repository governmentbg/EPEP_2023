using Epep.Core.Contracts;
using Microsoft.Extensions.Logging;
using Quartz;
using static IO.QuartzExtensions.BaseQuartzExtensions;

namespace Epep.Service.Jobs
{

    /// <summary>
    /// Услуга за изчистване на стари неподадени документи
    /// </summary>
    [DisallowConcurrentExecution]
    public class DocumentCleanerJob : BaseJob
    {
        private readonly IDocumentCleanerService service;

        public DocumentCleanerJob(
            IDocumentCleanerService _service,
            ILogger<DocumentCleanerJob> _logger)
        {
            service = _service;
            logger = _logger;
        }
        protected override async Task DoJob(IJobExecutionContext context)
        {
            try
            {
                await service.Notify();
            }
            catch (Exception ex) { }
            try
            {
                await service.Clean();
            }
            catch (Exception ex) { }
        }
    }
}
