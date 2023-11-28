using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IReporterRepository : IAggregateRepository<Reporter>
    {
    }

    internal class ReporterRepository : AggregateRepository<Reporter>, IReporterRepository
    {
        public ReporterRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}