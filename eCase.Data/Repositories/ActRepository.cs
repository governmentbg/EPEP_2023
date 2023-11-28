using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IActRepository : IAggregateRepository<Act>
    {
        bool CheckPermission(Guid actGid, long userId);
    }

    internal class ActRepository : AggregateRepository<Act>, IActRepository
    {
        private ICaseRepository _caseRepository;

        public ActRepository(IUnitOfWork unitOfWork, ICaseRepository caseRepository)
            : base(unitOfWork)
        {
            _caseRepository = caseRepository;
        }

        //protected override Expression<Func<Act, object>>[] Includes
        //{
        //    get
        //    {
        //        return new Expression<Func<Act, object>>[]
        //            {
        //                e => e.Case,
        //                e => e.Case.CaseKind,
        //                e => e.Case.CaseType,
        //                e => e.Case.Court,
        //                e => e.ActKind,
        //                e => e.ActPreparators
        //            };
        //    }
        //}

        public bool CheckPermission(Guid actGid, long userId)
        {
            var act = this.FindByGid(actGid);

            if (act != null)
            {
                return _caseRepository.CheckPermission(act.CaseId, userId);
            }

            return false;
        }
    }
}