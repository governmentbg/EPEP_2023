using eCase.Data.Core;
using eCase.Data.Upgrade.Contracts;
using eCase.Domain.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace eCase.Data.Repositories
{
    /// <summary>
    /// Implementation of repository access methods
    /// for Relational Database Engine
    /// </summary>
    /// <typeparam name="T">Type of the data table to which 
    /// current reposity is attached</typeparam>
    public class UpgradeRepository : IUpgradeRepository
    {
        /// <summary>
        /// Entity framework DB context holding connection information and properties
        /// and tracking entity states 
        /// </summary>
        private readonly UnitOfWork unitOfWork;

        /// <summary>
        /// Representation of table in database
        /// </summary>
        DbSet<T> DbSet<T>() where T : class
        {

            return this.unitOfWork.DbContext.Set<T>();

        }

        /// <summary>
        /// Public constructor to inject dependancies into the repository
        /// </summary>
        /// <param name="context">EF context to inject</param>
        public UpgradeRepository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = (UnitOfWork)unitOfWork;
        }

        /// <summary>
        /// Adds entity to the database
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public void Add<T>(T entity) where T : class
        {
            this.DbSet<T>().Add(entity);
        }

        /// <summary>
        /// Ads collection of entities to the database
        /// </summary>
        /// <param name="entities">Enumerable list of entities</param>
        public void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            this.DbSet<T>().AddRange(entities);
        }

        /// <summary>
        /// All records in a table
        /// </summary>
        /// <returns>Queryable expression tree</returns>
        public IQueryable<T> All<T>() where T : class
        {
            return this.DbSet<T>().AsQueryable();
        }

        public IQueryable<T> All<T>(Func<T, bool> search) where T : class
        {
            return this.DbSet<T>().Where(search).AsQueryable();
        }

        /// <summary>
        /// The result collection won't be tracked by the context
        /// </summary>
        /// <returns>Expression tree</returns>
        public IQueryable<T> AllReadonly<T>() where T : class
        {
            return this.DbSet<T>()
                .AsQueryable()
                .AsNoTracking();
        }
        public IQueryable<T> AllReadonly<T>(Func<T, bool> search) where T : class
        {
            return this.DbSet<T>()
                .Where(search)
                .AsQueryable()
                .AsNoTracking();
        }

        /// <summary>
        /// Deletes a record from database
        /// </summary>
        /// <param name="id">Identificator of record to be deleted</param>
        public void Delete<T>(object id) where T : class
        {
            T entity = GetById<T>(id);
            if (entity != null)
            {
                Delete<T>(entity);
            }
        }

        /// <summary>
        /// Deletes a record from database
        /// </summary>
        /// <param name="entity">Entity representing record to be deleted</param>
        public void Delete<T>(T entity) where T : class
        {
            var entry = this.unitOfWork.DbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.DbSet<T>().Attach(entity);
            }

            entry.State = EntityState.Deleted;
        }

        /// <summary>
        /// Detaches given entity from the context
        /// </summary>
        /// <param name="entity">Entity to be detached</param>
        public void Detach<T>(T entity) where T : class
        {
            var entry = this.unitOfWork.DbContext.Entry(entity);

            entry.State = EntityState.Detached;
        }

        /// <summary>
        /// Disposing the context when it is not neede
        /// Don't have to call this method explicitely
        /// Leave it to the IoC container
        /// </summary>
        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

        /// <summary>
        /// Gets specific record from database by primary key
        /// </summary>
        /// <param name="id">record identificator</param>
        /// <returns>Single record</returns>
        public T GetById<T>(object id) where T : class
        {
            return this.DbSet<T>().Find(id);
        }

        public T FindByGid<T>(Guid gid) where T : class, IGidRoot
        {
            return All<T>().Where(x => x.Gid == gid).FirstOrDefault();
        }

        /// <summary>
        /// Saves all made changes in trasaction
        /// </summary>
        /// <returns>Error code</returns>
        public void SaveChanges()
        {
            this.unitOfWork.Save();
        }

        /// <summary>
        /// Updates a record in database
        /// </summary>
        /// <param name="entity">Entity for record to be updated</param>
        public void Update<T>(T entity) where T : class
        {
            this.DbSet<T>().AddOrUpdate(entity);
        }

        /// <summary>
        /// Deletes multiple records 
        /// </summary>
        /// <param name="entities">Records to delete</param>
        public void DeleteRange<T>(IEnumerable<T> entities) where T : class
        {
            this.DbSet<T>().RemoveRange(entities);
        }

        /// <summary>
        /// Deletes multiple records
        /// </summary>
        /// <param name="deleteWhereClause">Where clause</param>
        public void DeleteRange<T>(Func<T, bool> deleteWhereClause) where T : class
        {
            var entities = All<T>(deleteWhereClause);
            DeleteRange(entities);
        }

        public Tprop GetPropById<T, Tprop>(Expression<Func<T, bool>> where, Expression<Func<T, Tprop>> select)
            where T : class
        {
            return this.DbSet<T>().Where(where).Select(select).FirstOrDefault();
        }
    }
}
