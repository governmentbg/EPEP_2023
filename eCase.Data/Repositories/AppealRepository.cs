using System;
using System.Linq.Expressions;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IAppealRepository : IAggregateRepository<Appeal>
    {
    }

    internal class AppealRepository : AggregateRepository<Appeal>, IAppealRepository
    {
        private ICaseRepository _caseRepository;
        private IActRepository _actRepository;

        public AppealRepository(IUnitOfWork unitOfWork, IActRepository actRepository, ICaseRepository caseRepository)
            : base(unitOfWork)
        {
            _caseRepository = caseRepository;
            _actRepository = actRepository;
        }

        public bool CheckPermission(Guid appealGid, long userId)
        {
            var appeal = this.FindByGid(appealGid);
            

            if (appeal != null)
            {
                var act = _actRepository.Find(appeal.ActId);
                if (act != null)
                {
                    return _caseRepository.CheckPermission(act.CaseId, userId);
                }
                
            }
            return false;
        }
            //protected override Expression<Func<Appeal, object>>[] Includes
            //{
            //    get
            //    {
            //        return new Expression<Func<Appeal, object>>[]
            //            {
            //                e => e.Act,
            //                e => e.Act.Case,
            //                e => e.Act.Case.CaseKind
            //            };
            //    }
            //}
        }
}