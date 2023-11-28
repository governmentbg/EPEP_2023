using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using eCase.Data.Core;
using eCase.Domain.Entities;
using System.Collections.Generic;

namespace eCase.Data.Repositories
{
    public interface ICaseRepository : IAggregateRepository<Case>
    {
        bool CheckPermission(long caseId, long userId);

        bool CheckPermission(Guid caseGid, long userId);

        IEnumerable<Case> GetConnectedCases(long caseId);

        IQueryable<Case> GetAllPredecessorCases();

        void UpdateCasesWithPredecessors(IQueryable<Case> predecessorCases, ref IQueryable<Case> cases);

        Case GetCaseByInitIncomingDocument(long incomingDocumentId);

        Guid GetCaseIdByNumberYearKindCourt(int caseNumber, int caseYear, long caseKindId, long courtId);
    }

    internal class CaseRepository : AggregateRepository<Case>, ICaseRepository
    {
        public CaseRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        //protected override Expression<Func<Case, object>>[] Includes
        //{
        //    get
        //    {
        //        return new Expression<Func<Case, object>>[]
        //            {
        //                e => e.Reporters,
        //                e => e.Sides,
        //                e => e.Sides.Select(side => side.PersonAssignments),
        //                e => e.Sides.Select(side => side.Subject),
        //                e => e.Sides.Select(side => side.SideInvolvementKind),
        //                e => e.Sides.Select(side => side.LawyerAssignments),
        //                e => e.Sides.Select(side => side.LawyerAssignments.Select(assignment => assignment.Lawyer)),
        //                e => e.Court,
        //                e => e.CaseKind,
        //                e => e.Acts,
        //                e => e.Acts.Select(act => act.ActKind),
        //                e => e.Acts.Select(act => act.ActPreparators),
        //                e => e.Acts.Select(act => act.Appeals),
        //                e => e.Hearings,
        //                e => e.Assignments,
        //                e => e.IncomingDocument,
        //                e => e.IncomingDocuments,
        //                e => e.IncomingDocuments.Select(doc => doc.IncomingDocumentType),
        //                e => e.OutgoingDocuments,              
        //                e => e.OutgoingDocuments.Select(doc => doc.OutgoingDocumentType),
        //                e => e.CaseRulings,
        //                e => e.CaseRulings.Select(ruling => ruling.CaseRulingKind),
        //                e => e.StatisticCode
        //            };
        //    }
        //}

        public bool CheckPermission(long caseId, long userId)
        {
            var user = this.unitOfWork.DbContext.Set<User>().FirstOrDefault(e => e.UserId == userId);
            var c = this.FindFirstOrDefault(caseId);

            return CheckPermission(c, user);
        }

        public bool CheckPermission(Guid caseGid, long userId)
        {
            var user = this.unitOfWork.DbContext.Set<User>().FirstOrDefault(e => e.UserId == userId);
            var c = this.FindByGid(caseGid);

            return CheckPermission(c, user);
        }

        private bool CheckPermission(Case c, User user)
        {
            if (user != null && c != null)
            {
                var userGroupId = user.UserGroupId;

                if (userGroupId.Equals(UserGroup.SuperAdmin) || userGroupId.Equals(UserGroup.SystemAdmin))
                {
                    return true;
                }
                else if (userGroupId.Equals(UserGroup.CourtAdmin) && user.CourtId.HasValue)
                {
                    return c.CourtId == user.CourtId.Value;
                }
                else if (userGroupId.Equals(UserGroup.Person))
                {
                    return this.unitOfWork.DbContext.Set<PersonAssignment>()
                        .Include(e => e.PersonRegistration)
                        .Include(e => e.PersonRegistration.User)
                        .Where(pa => pa.PersonRegistration.User.UserId == user.UserId)
                        .Any(e => e.Side.CaseId == c.CaseId && (e.IsActive ?? true) == true);
                }
                else
                {
                    return this.unitOfWork.DbContext.Set<User>()
                         .Include(e => e.LawyerRegistration)
                         .Include(e => e.LawyerRegistration.Lawyer)
                         .Include(e => e.LawyerRegistration.Lawyer.LawyerAssignments)
                         .Any(e => e.LawyerRegistration.Lawyer.LawyerAssignments.Any(la => la.Side.CaseId == c.CaseId && e.IsActive == true));
                }
            }

            return false;
        }

        public IEnumerable<Case> GetConnectedCases(long caseId)
        {
            var predecessors =
                (from cc in this.unitOfWork.DbContext.Set<ConnectedCase>()
                 join c in this.unitOfWork.DbContext.Set<Case>() on cc.PredecessorCaseId equals c.CaseId

                 where cc.CaseId == caseId

                 select c).Include(e => e.CaseKind).Include(e => e.Court).ToList();

            var successors =
                (from cc in this.unitOfWork.DbContext.Set<ConnectedCase>()
                 join c in this.unitOfWork.DbContext.Set<Case>() on cc.CaseId equals c.CaseId

                 where cc.PredecessorCaseId == caseId

                 select c).Include(e => e.CaseKind).Include(e => e.Court).ToList();

            return predecessors.Concat(successors);
        }

        public IQueryable<Case> GetAllPredecessorCases()
        {
            return from cc in this.unitOfWork.DbContext.Set<ConnectedCase>()
                   join c in this.unitOfWork.DbContext.Set<Case>() on cc.PredecessorCaseId equals c.CaseId

                   select c;
        }

        public void UpdateCasesWithPredecessors(IQueryable<Case> predecessorCases, ref IQueryable<Case> cases)
        {
            cases = from c in cases
                    join cc in this.unitOfWork.DbContext.Set<ConnectedCase>() on c.CaseId equals cc.CaseId
                    join pc in predecessorCases on cc.PredecessorCaseId equals pc.CaseId

                    select c;
        }

        public Case GetCaseByInitIncomingDocument(long incomingDocumentId)
        {
            return this.Set()
                 .Where(i => i.IncomingDocumentId == incomingDocumentId).FirstOrDefault();
        }

        public Guid GetCaseIdByNumberYearKindCourt(int caseNumber, int caseYear, long caseKindId, long courtId)
        {
            return this.Set()
                 .Where(i => i.Number == caseNumber && i.CaseYear == caseYear && i.CaseKindId == caseKindId && i.CourtId == courtId).FirstOrDefault().Gid;
        }
    }
}