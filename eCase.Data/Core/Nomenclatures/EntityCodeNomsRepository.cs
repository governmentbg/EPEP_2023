using System;
using System.Linq;
using System.Linq.Expressions;

using eCase.Data.Linq;

namespace eCase.Data.Core.Nomenclatures
{
    internal class EntityCodeNomsRepository<TEntity, TQuery, TCodeNomVO> : EntityNomsRepository<TEntity, TQuery, TCodeNomVO>, IEntityCodeNomsRepository<TEntity, TCodeNomVO>
        where TEntity : class
    {
        protected Expression<Func<TQuery, string>> codeSelector;

        public EntityCodeNomsRepository(
            IUnitOfWork unitOfWork,
            Expression<Func<TQuery, long>> keySelector,
            Expression<Func<TQuery, string>> nameSelector,
            Expression<Func<TQuery, string>> codeSelector,
            Expression<Func<TQuery, TCodeNomVO>> voSelector,
            Expression<Func<TQuery, decimal>> orderBySelector = null)
            : base(unitOfWork, keySelector, nameSelector, voSelector, orderBySelector)
        {
            this.codeSelector = codeSelector;
        }

        public virtual long GetNomIdByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("code");
            }

            var predicate =
                PredicateBuilder.True<TQuery>()
                .AndPropertyEquals(this.codeSelector, code);

            return this.GetQuery()
                .Where(predicate)
                .Select(this.keySelector)
                .Single();
        }

        public virtual bool HasCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("code");
            }

            var predicate =
                PredicateBuilder.True<TQuery>()
                .AndPropertyEquals(this.codeSelector, code);

            return this.GetQuery().Any(predicate);
        }
    }

    internal class EntityCodeNomsRepository<TEntity, TCodeNomVO> : EntityCodeNomsRepository<TEntity, TEntity, TCodeNomVO>
        where TEntity : class
    {
        public EntityCodeNomsRepository(
            IUnitOfWork unitOfWork,
            Expression<Func<TEntity, long>> keySelector,
            Expression<Func<TEntity, string>> nameSelector,
            Expression<Func<TEntity, string>> codeSelector,
            Expression<Func<TEntity, TCodeNomVO>> voSelector,
            Expression<Func<TEntity, decimal>> orderBySelector = null)
            : base(unitOfWork, keySelector, nameSelector, codeSelector, voSelector, orderBySelector)
        {
        }
    }
}
