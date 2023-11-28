using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IConnectedCaseRepository : IAggregateRepository<ConnectedCase>
    {
    }

    internal class ConnectedCaseRepository : AggregateRepository<ConnectedCase>, IConnectedCaseRepository
    {
        public ConnectedCaseRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}