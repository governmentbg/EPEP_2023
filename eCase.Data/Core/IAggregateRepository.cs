using System;

using eCase.Domain.Core;
using System.Linq;

namespace eCase.Data.Core
{
    public interface IAggregateRepository<TEntity> : IRepository
        where TEntity : class, IAggregateRoot
    {
        TEntity Find(long id);

        TEntity FindByGid(Guid gid);

  
        TEntity FindFirstOrDefault(long id);

        TEntity FindForUpdate(long id, byte[] version);

        void Add(TEntity entity);

        void Remove(TEntity entity);

        byte[] GetVersion(long id);

        IQueryable<TEntity> SetWithoutIncludes();
    }
}
