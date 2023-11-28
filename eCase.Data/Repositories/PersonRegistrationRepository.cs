using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IPersonRegistrationRepository : IAggregateRepository<PersonRegistration>
    {
    }

    internal class PersonRegistrationRepository : AggregateRepository<PersonRegistration>, IPersonRegistrationRepository
    {
        public PersonRegistrationRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
