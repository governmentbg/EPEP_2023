using eCase.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace eCase.Data.Upgrade.Contracts
{
    public interface IUpgradeRepository
    {
        void Add<T>(T entity) where T : class;
        void AddRange<T>(IEnumerable<T> entities) where T : class;
        IQueryable<T> All<T>() where T : class;
        IQueryable<T> All<T>(Func<T, bool> search) where T : class;
        IQueryable<T> AllReadonly<T>() where T : class;
        IQueryable<T> AllReadonly<T>(Func<T, bool> search) where T : class;
        void Delete<T>(object id) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteRange<T>(IEnumerable<T> entities) where T : class;
        void DeleteRange<T>(Func<T, bool> deleteWhereClause) where T : class;
        void Detach<T>(T entity) where T : class;
        void Dispose();
        T FindByGid<T>(Guid gid) where T : class, IGidRoot;
        T GetById<T>(object id) where T : class;
        Tprop GetPropById<T, Tprop>(Expression<Func<T, bool>> where, Expression<Func<T, Tprop>> select) where T : class;
        void SaveChanges();
        void Update<T>(T entity) where T : class;
    }
}
