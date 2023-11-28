using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IEntityRepository : IAggregateRepository<Entity>
    {
    }

    internal class EntityRepository : AggregateRepository<Entity>, IEntityRepository
    {
        public EntityRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}