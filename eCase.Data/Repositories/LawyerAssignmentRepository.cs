using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface ILawyerAssignmentRepository : IAggregateRepository<LawyerAssignment>
    {
    }

    internal class LawyerAssignmentRepository : AggregateRepository<LawyerAssignment>, ILawyerAssignmentRepository
    {
        public LawyerAssignmentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
