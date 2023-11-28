using System;
using System.Collections.Generic;
using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Emails;

namespace eCase.Data.Repositories
{
    public interface IMailRepository : IAggregateRepository<Email>
    {
        IList<long> GetPendingEmailIds(int limit, int maxFailedAttempts, TimeSpan failedAttemptTimeout);
    }

    internal class MailRepository : AggregateRepository<Email>, IMailRepository
    {
        public MailRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IList<long> GetPendingEmailIds(int limit, int maxFailedAttempts, TimeSpan failedAttemptTimeout)
        {
            var maxInterval = DateTime.Now - failedAttemptTimeout;

            return this.unitOfWork.DbContext.Set<Email>()
                .Where(e => e.Status == EmailStatus.Pending && (e.FailedAttempts == 0 || (e.FailedAttempts < maxFailedAttempts && e.ModifyDate < maxInterval)))
                .OrderBy(e => e.CreateDate)
                .Select(e => e.EmailId)
                .Take(limit)
                .ToList();
        }
    }
}