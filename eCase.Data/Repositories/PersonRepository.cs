using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IPersonRepository : IAggregateRepository<Person>
    {
    }

    internal class PersonRepository : AggregateRepository<Person>, IPersonRepository
    {
        public PersonRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}