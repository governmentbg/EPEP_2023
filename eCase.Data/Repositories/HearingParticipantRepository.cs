using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IHearingParticipantRepository : IAggregateRepository<HearingParticipant>
    {
    }

    internal class HearingParticipantRepository : AggregateRepository<HearingParticipant>, IHearingParticipantRepository
    {
        public HearingParticipantRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}