using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IPersonAssignmentRepository : IAggregateRepository<PersonAssignment>
    {
    }

    internal class PersonAssignmentRepository : AggregateRepository<PersonAssignment>, IPersonAssignmentRepository
    {
        public PersonAssignmentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
