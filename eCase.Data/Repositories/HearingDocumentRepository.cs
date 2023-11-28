using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IHearingDocumentRepository : IAggregateRepository<HearingDocument>
    {
    }

    internal class HearingDocumentRepository : AggregateRepository<HearingDocument>, IHearingDocumentRepository
    {
        public HearingDocumentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}