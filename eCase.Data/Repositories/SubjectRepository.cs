using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface ISubjectRepository : IAggregateRepository<Subject>
    {
    }

    internal class SubjectRepository : AggregateRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }

    public enum SubjectType
    {
        Person = 1,
        Entity = 2
    }
}