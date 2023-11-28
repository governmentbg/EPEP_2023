using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IActPreparatorRepository : IAggregateRepository<ActPreparator>
    {
    }

    internal class ActPreparatorRepository : AggregateRepository<ActPreparator>, IActPreparatorRepository
    {
        public ActPreparatorRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}