using eCase.Data.Core;
using eCase.Domain.Entities;
using System.Linq;

namespace eCase.Data.Repositories
{
    public interface ICaseRulingRepository : IAggregateRepository<CaseRuling>
    {
        IQueryable<CaseRuling> GetCaseRulings(long caseId);
    }

    internal class CaseRulingRepository : AggregateRepository<CaseRuling>, ICaseRulingRepository
    {
        public CaseRulingRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IQueryable<CaseRuling> GetCaseRulings(long caseId)
        {
            return this.Set().Where(t => t.CaseId == caseId);
        }
    }
}