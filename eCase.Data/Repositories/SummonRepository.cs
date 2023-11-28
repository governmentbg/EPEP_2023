using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using eCase.Common.Enums;
using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface ISummonRepository : IAggregateRepository<Summon>
    {
        List<Summon> GetReadSummonsForCertainDay(DateTime date);

        List<Summon> GetAllReadSummons();

        IEnumerable<Summon> GetSummonsByUserId(long userId);

        IEnumerable<Summon> GetSummonsByCourtCode(string courtCode);

        IEnumerable<Summon> GetSummonsByUserByType(long userId, SummonTypeNomenclature summonType, params long[] parentIds);

        bool CheckPermission(Guid summonGid, long userId);

        Summon GetSummonByGid(Guid summonGid);
    }

    internal class SummonRepository : AggregateRepository<Summon>, ISummonRepository
    {
        private IUserRepository _userRepository;
        private ICaseRepository _caseRepository;
        private IEntityCodeNomsRepository<Court, EntityCodeNomVO> _courtRepository;

        public SummonRepository(IUnitOfWork unitOfWork, IUserRepository userRepository, ICaseRepository caseRepository)
            : base(unitOfWork)
        {
            _userRepository = userRepository;
            _caseRepository = caseRepository;
        }

        protected override Expression<Func<Summon, object>>[] Includes
        {
            get
            {
                return new Expression<Func<Summon, object>>[]
                    {
                        e => e.Side,
                        e => e.Side.Case,
                        e => e.Side.Case.Court,
                        e => e.Side.Case.CaseKind,
                        e => e.SummonType
                    };
            }
        }

        public List<Summon> GetReadSummonsForCertainDay(DateTime date)
        {
            return this.SetWithoutIncludes()
                    .Where(t => t.ReadTime == date && t.IsRead)
                    .ToList();
        }

        public List<Summon> GetAllReadSummons()
        {
            return this.SetWithoutIncludes()
                    .Where(t => t.IsRead)
                    .ToList();
        }

        public bool CheckPermission(Guid summonGid, long userId)
        {
            var summon = this.FindByGid(summonGid);

            if (summon != null && summon.Side != null && summon.Side.Case != null)
            {
                return _caseRepository.CheckPermission(summon.Side.Case.CaseId, userId);
            }

            return false;
        }

        public IEnumerable<Summon> GetSummonsByUserId(long userId)
        {
            var user = _userRepository.FindFirstOrDefault(userId);

            var summons = new List<Summon>();

            if (user != null)
            {
                if (user.UserGroupId.Equals(UserGroup.Person))
                {
                    var assignments = this.unitOfWork.DbContext.Set<PersonAssignment>()
                            .Include(e => e.Side)
                            .Include(e => e.Side.Summons)

                            .Include(e => e.Side.Summons.Select(summon => summon.Side))
                            .Include(e => e.Side.Summons.Select(summon => summon.Side.Case))
                            .Include(e => e.Side.Summons.Select(summon => summon.Side.Case.Court))
                            .Include(e => e.Side.Summons.Select(summon => summon.SummonType))

                            .Include(e => e.PersonRegistration)
                            .Where(pa => pa.PersonRegistration.PersonRegistrationId == userId && (pa.IsActive ?? true)).ToList();

                    summons = assignments.Where(e => e.Side != null).SelectMany(e => e.Side.Summons).Distinct().ToList();
                }
                else if (user.UserGroupId.Equals(UserGroup.Lawyer))
                {
                    var assignments = this.unitOfWork.DbContext.Set<LawyerAssignment>()
                            .Include(e => e.Side)
                            .Include(e => e.Side.Summons)

                            .Include(e => e.Side.Summons.Select(summon => summon.Side))
                            .Include(e => e.Side.Summons.Select(summon => summon.Side.Case))
                            .Include(e => e.Side.Summons.Select(summon => summon.Side.Case.Court))
                            .Include(e => e.Side.Summons.Select(summon => summon.SummonType))

                            .Include(e => e.Lawyer)
                            .Include(e => e.Lawyer.LawyerRegistrations)
                            .Where(e => e.Lawyer.LawyerRegistrations.Any(lr => lr.LawyerRegistrationId == userId) && e.IsActive == true).ToList();

                    summons = assignments.Where(e => e.Side != null).SelectMany(e => e.Side.Summons).Distinct().ToList();
                }
            }

            return summons;
        }

        public IEnumerable<Summon> GetSummonsByCourtCode(string courtCode)
        {

            var summons = new List<Summon>();

            if (courtCode != null)
            {
                summons = this.unitOfWork.DbContext.Set<Summon>()
                           .Include(e => e.Case)
                           .Where(e => (e.Hearing.Case.Court.Code == courtCode || e.Case.Court.Code == courtCode || e.Act.Case.Court.Code == courtCode)).ToList();
            }

            return summons;
        }

        public IEnumerable<Summon> GetSummonsByUserByType(long userId, SummonTypeNomenclature summonType, params long[] parentIds)
        {
            var userSummons = this.GetSummonsByUserId(userId);
            var summons = userSummons
                        .Where(s => s.SummonType.Code == summonType.Code);

            var parentsList = parentIds.ToList();

            if (summonType == SummonTypeNomenclature.Act)
                summons = summons.Where(e => e.ActId != null && parentsList.Contains(e.ActId.Value));
            else if (summonType == SummonTypeNomenclature.Case)
                summons = summons.Where(e => e.CaseId != null && parentsList.Contains(e.CaseId.Value));
            else if (summonType == SummonTypeNomenclature.Hearing)
                summons = summons.Where(e => e.HearingId != null && parentsList.Contains(e.HearingId.Value));
            else if (summonType == SummonTypeNomenclature.Appeal)
                summons = summons.Where(e => e.AppealId != null && parentsList.Contains(e.AppealId.Value));

            return summons;
        }

        public Summon GetSummonByGid(Guid summonGid)
        {
            return this.Set()
                .Include(s => s.Side)
                .Include(s => s.Side.Case)
                .Include(s => s.Side.Case.Court)
                .Include(s => s.Side.Case.CaseKind)
                .Include(s => s.SummonType)
                .Where(t => t.Gid == summonGid)
                .SingleOrDefault();
        }
    }
}