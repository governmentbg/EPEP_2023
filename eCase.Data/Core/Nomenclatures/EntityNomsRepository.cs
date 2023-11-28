using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using eCase.Data.Linq;

namespace eCase.Data.Core.Nomenclatures
{
    internal class EntityNomsRepository<TEntity, TQuery, TNomVO> : Repository, IEntityNomsRepository<TEntity, TNomVO>
        where TEntity : class
    {
        protected Expression<Func<TQuery, long>> keySelector;
        protected Expression<Func<TQuery, string>> nameSelector;
        protected Expression<Func<TQuery, TNomVO>> voSelector;
        protected Expression<Func<TQuery, decimal>> orderBySelector;

        public EntityNomsRepository(
            IUnitOfWork unitOfWork,
            Expression<Func<TQuery, long>> keySelector,
            Expression<Func<TQuery, string>> nameSelector,
            Expression<Func<TQuery, TNomVO>> voSelector,
            Expression<Func<TQuery, decimal>> orderBySelector = null)
            : base(unitOfWork)
        {
            this.keySelector = keySelector;
            this.nameSelector = nameSelector;
            this.voSelector = voSelector;
            this.orderBySelector = orderBySelector;
        }

        public virtual TNomVO GetNom(long nomValueId)
        {
            if (nomValueId == default(int))
            {
                throw new ArgumentException("Filtering by the default value for nomValueId is not allowed.");
            }

            var predicate =
                PredicateBuilder.True<TQuery>()
                .AndPropertyEquals(this.keySelector, nomValueId);

            return this.GetQuery()
                .Where(predicate)
                .Select(this.voSelector)
                .SingleOrDefault();
        }

        public virtual IEnumerable<TNomVO> GetNoms(string term, int offset = 0, int? limit = null)
        {
            var query = this.GetNameFilteredQuery(term);

            query = this.orderBySelector == null ?
                query.OrderBy(this.nameSelector) :
                query.OrderBy(this.orderBySelector);

            return query
                .WithOffsetAndLimit(offset, limit)
                .Select(this.voSelector)
                .ToList();
        }

        protected virtual IQueryable<TQuery> GetQuery()
        {
            return this.unitOfWork.DbContext.Set<TEntity>().Cast<TQuery>();
        }

        protected virtual IQueryable<TQuery> GetNameFilteredQuery(string term)
        {
             var predicate =
                PredicateBuilder.True<TQuery>()
                .AndStringContains(this.nameSelector, term);

             return this.GetQuery().Where(predicate);
        }
    }

    internal class EntityNomsRepository<TEntity, TNomVO> : EntityNomsRepository<TEntity, TEntity, TNomVO>
        where TEntity : class
    {
        public EntityNomsRepository(
            IUnitOfWork unitOfWork,
            Expression<Func<TEntity, long>> keySelector,
            Expression<Func<TEntity, string>> nameSelector,
            Expression<Func<TEntity, TNomVO>> voSelector)
            : base(
                unitOfWork,
                keySelector,
                nameSelector,
                voSelector)
        {
        }
    }
}
