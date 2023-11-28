using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using eCase.Data.Linq;
using eCase.Domain.Core;

namespace eCase.Data.Core
{
    internal class AggregateRepository<TEntity, TBaseEntity> : Repository, IAggregateRepository<TEntity>
        where TEntity : class, IAggregateRoot
        where TBaseEntity : class, IAggregateRoot
    {
        public AggregateRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            this.unitOfWork = (UnitOfWork)unitOfWork;
        }

        public virtual TEntity Find(long id)
        {
            return this.FindEntity(id, this.Includes);
        }

        public virtual TEntity FindByGid(Guid gid)
        {
            return this.FindEntityByGid(gid, this.Includes);
        }

        public virtual TEntity FindFirstOrDefault(long id)
        {
            try
            {
                return this.FindEntity(id, this.Includes);
            }
            catch
            {
                return default(TEntity);
            }
        }

        public virtual TEntity FindForUpdate(long id, byte[] version)
        {
            var entity = this.Find(id);
            this.CheckVersion(entity.Version, version);

            return entity;
        }

        public byte[] GetVersion(long id)
        {
            return this.Get(id).Version;
        }

        public IQueryable<TEntity> SetWithoutIncludes()
        {
            return this.unitOfWork.DbContext.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            if (entity.CreateDate == DateTime.MinValue)
            {
                entity.CreateDate = DateTime.Now;
                entity.ModifyDate = entity.CreateDate;
            }

            this.unitOfWork.DbContext.Set<TEntity>().Add(entity);
        }

        public void Remove(TEntity entity)
        {
            this.unitOfWork.DbContext.Set<TEntity>().Remove(entity);
        }

        #region Protected methods

        protected virtual Expression<Func<TEntity, object>>[] Includes
        {
            get
            {
                return new Expression<Func<TEntity, object>>[] { };
            }
        }

        protected TEntity Get(long id)
        {
            return this.FindEntity(id, new Expression<Func<TEntity, object>>[0]);
        }

        protected IQueryable<TEntity> Set()
        {
            return this.unitOfWork.DbContext.Set<TBaseEntity>().OfType<TEntity>().IncludeMultiple(this.Includes);
        }

        protected void CheckVersion(byte[] version1, byte[] version2)
        {
            if (!Enumerable.SequenceEqual(version1, version2))
            {
                throw new DataUpdateConcurrencyException();
            }
        }

        #endregion

        #region Private methods

        private TEntity FindEntity(long id, params Expression<Func<TEntity, object>>[] includes)
        {
            object[] keyValues = new object[] { id };

            return this.FindInStore(keyValues, includes);
        }

        private TEntity FindEntityByGid(Guid gid, params Expression<Func<TEntity, object>>[] includes)
        {
            object[] keyValues = new object[] { gid };

            return this.FindInStoreByGid(keyValues, includes);
        }

        private TEntity FindInStore(object[] keyValues, params Expression<Func<TEntity, object>>[] includes)
        {
            ObjectContext objectContext = ((IObjectContextAdapter)this.unitOfWork.DbContext).ObjectContext;
            ObjectSet<TBaseEntity> objectSet = objectContext.CreateObjectSet<TBaseEntity>();

            string quotedEntitySetName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}",
                this.QuoteIdentifier(objectSet.EntitySet.EntityContainer.Name),
                this.QuoteIdentifier(objectSet.EntitySet.Name));

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("SELECT VALUE X FROM {0} AS X WHERE ", quotedEntitySetName);

            var entityKeyValues = this.CreateEntityKey(objectSet.EntitySet, keyValues).EntityKeyValues;
            var parameters = new ObjectParameter[entityKeyValues.Length];

            for (var i = 0; i < entityKeyValues.Length; i++)
            {
                if (i > 0)
                {
                    queryBuilder.Append(" AND ");
                }

                var name = string.Format(CultureInfo.InvariantCulture, "gc{0}", i.ToString(CultureInfo.InvariantCulture));
                queryBuilder.AppendFormat("X.{0} = @{1}", this.QuoteIdentifier(entityKeyValues[i].Key), name);
                parameters[i] = new ObjectParameter(name, entityKeyValues[i].Value);
            }

            IQueryable<TEntity> query = objectContext.CreateQuery<TBaseEntity>(queryBuilder.ToString(), parameters).OfType<TEntity>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.SingleOrDefault();
        }

        private TEntity FindInStoreByGid(object[] keyValues, params Expression<Func<TEntity, object>>[] includes)
        {
            ObjectContext objectContext = ((IObjectContextAdapter)this.unitOfWork.DbContext).ObjectContext;
            ObjectSet<TBaseEntity> objectSet = objectContext.CreateObjectSet<TBaseEntity>();

            string quotedEntitySetName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}",
                this.QuoteIdentifier(objectSet.EntitySet.EntityContainer.Name),
                this.QuoteIdentifier(objectSet.EntitySet.Name));

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("SELECT VALUE X FROM {0} AS X WHERE ", quotedEntitySetName);

            var entityKeyValues = this.CreateEntityGid(objectSet.EntitySet, keyValues).EntityKeyValues;
            var parameters = new ObjectParameter[entityKeyValues.Length];

            for (var i = 0; i < entityKeyValues.Length; i++)
            {
                if (i > 0)
                {
                    queryBuilder.Append(" AND ");
                }

                var name = string.Format(CultureInfo.InvariantCulture, "gc{0}", i.ToString(CultureInfo.InvariantCulture));
                queryBuilder.AppendFormat("X.{0} = @{1}", this.QuoteIdentifier(entityKeyValues[i].Key), name);
                parameters[i] = new ObjectParameter(name, entityKeyValues[i].Value);
            }

            IQueryable<TEntity> query = objectContext.CreateQuery<TBaseEntity>(queryBuilder.ToString(), parameters).OfType<TEntity>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.FirstOrDefault();
        }

        private string QuoteIdentifier(string identifier)
        {
            return "[" + identifier.Replace("]", "]]") + "]";
        }

        private EntityKey CreateEntityKey(System.Data.Entity.Core.Metadata.Edm.EntitySet entitySet, object[] keyValues)
        {
            if (keyValues == null || !keyValues.Any() || keyValues.Any(v => v == null))
            {
                throw new ArgumentException("Parameter keyValues cannot be empty or contain nulls.");
            }

            var keyNames = entitySet.ElementType.KeyMembers.Select(m => m.Name).ToList();
            if (keyNames.Count != keyValues.Length)
            {
                throw new ArgumentException("Invalid number of key values.");
            }

            return new System.Data.Entity.Core.EntityKey(entitySet.EntityContainer.Name + "." + entitySet.Name, keyNames.Zip(keyValues, (name, value) => new KeyValuePair<string, object>(name, value)));
        }

        private EntityKey CreateEntityGid(System.Data.Entity.Core.Metadata.Edm.EntitySet entitySet, object[] keyValues)
        {
            if (keyValues == null || !keyValues.Any() || keyValues.Any(v => v == null))
            {
                throw new ArgumentException("Parameter keyValues cannot be empty or contain nulls.");
            }

            var keyNames = entitySet.ElementType.KeyMembers.Select(m => "Gid").ToList();
            if (keyNames.Count != keyValues.Length)
            {
                throw new ArgumentException("Invalid number of key values.");
            }

            return new System.Data.Entity.Core.EntityKey(entitySet.EntityContainer.Name + "." + entitySet.Name, keyNames.Zip(keyValues, (name, value) => new KeyValuePair<string, object>(name, value)));
        }

        #endregion
    }

    internal class AggregateRepository<TEntity> : AggregateRepository<TEntity, TEntity>
        where TEntity : class, IAggregateRoot
    {
        public AggregateRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
