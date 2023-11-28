using System;
using System.Data.Entity;
using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface ISideRepository : IAggregateRepository<Side>
    {
        Side GetSide(Guid sideGid);
    }

    internal class SideRepository : AggregateRepository<Side>, ISideRepository
    {
        public SideRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public Side GetSide(Guid sideGid)
        {
            return this.Set()
                .Where(i => i.Gid == sideGid)
                .Include(t => t.Subject)
                .Include(t => t.Subject.Person)
                .Include(t => t.Subject.Entity)
                .FirstOrDefault();
        }
    }
}