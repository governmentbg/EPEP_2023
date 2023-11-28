using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface ILawyerRegistrationRepository : IAggregateRepository<LawyerRegistration>
    {
        LawyerRegistration GetLawyerRegistrationByLawyerId(long lawyerId);
    }

    internal class LawyerRegistrationRepository : AggregateRepository<LawyerRegistration>, ILawyerRegistrationRepository
    {
        public LawyerRegistrationRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public LawyerRegistration GetLawyerRegistrationByLawyerId(long lawyerId)
        {
            return this.Set()
                    .Where(e => e.LawyerId == lawyerId)
                    .SingleOrDefault();
        }
    }
}
