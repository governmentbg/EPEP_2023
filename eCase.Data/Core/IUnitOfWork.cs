using System;
using System.Data;
using System.Data.Entity;

namespace eCase.Data.Core
{
    public delegate IUnitOfWork UnitOfWorkFactory(DbKey dbKey);

    public interface IUnitOfWork : IDisposable
    {
        DbContextTransaction BeginTransaction();

        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

        void Save();
    }
}
