using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IHearingRepository : IAggregateRepository<Hearing>
    {
        bool CheckPermission(Guid hearingGid, long userId);
    }

    internal class HearingRepository : AggregateRepository<Hearing>, IHearingRepository
    {
        private ICaseRepository _caseRepository;

        public HearingRepository(IUnitOfWork unitOfWork, ICaseRepository caseRepository)
            : base(unitOfWork)
        {
            _caseRepository = caseRepository;
        }

        //protected override Expression<Func<Hearing, object>>[] Includes
        //{
        //    get
        //    {
        //        return new Expression<Func<Hearing, object>>[]
        //            {
        //                e => e.Case,
        //                e => e.Case.Court,
        //                e => e.Case.CaseKind,
        //                e => e.HearingParticipants,
        //                e => e.Acts,
        //                e => e.Acts.Select(act =>act.ActKind),
        //                e => e.Acts.Select(act =>act.ActPreparators)
        //            };
        //    }
        //}

        public bool CheckPermission(Guid hearingGid, long userId)
        {
            var hearing = this.FindByGid(hearingGid);

            if (hearing != null)
            {
                return _caseRepository.CheckPermission(hearing.CaseId, userId);
            }

            return false;
        }
    }
}