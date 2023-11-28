using System;
using System.Linq;
using System.Linq.Expressions;

using eCase.Data.Core;
using eCase.Domain.Entities;
using System.Collections.Generic;

namespace eCase.Data.Repositories
{
    public interface IUserRepository : IAggregateRepository<User>
    {
        User Find(string username);

        IQueryable<User> FindByDateCreated(DateTime created);

        User FindByActivationCode(string activationCode);
        IQueryable<User> GetUsersForCourt(long courtId);
        IQueryable<User> GetUsersForCourt(long courtId, params long[] roles);
    }

    internal class UserRepository : AggregateRepository<User>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        //protected override Expression<Func<User, object>>[] Includes
        //{
        //    get
        //    {
        //        return new Expression<Func<User, object>>[]
        //            {
        //                e => e.UserGroup,
        //                e => e.Court,
        //                e => e.LawyerRegistration
        //            };
        //    }
        //}

        public User Find(string username)
        {
            return this.Set()
                .Where(u => u.Username == username.ToLower())
                .SingleOrDefault();
        }

        public IQueryable<User> FindByDateCreated(DateTime created)
        {
            return this.Set()
                .Where(u => u.CreateDate == created);
        }

        public User FindByActivationCode(string activationCode)
        {
            return this.Set()
                .Where(u => u.ActivationCode == activationCode)
                .SingleOrDefault();
        }

        public IQueryable<User> GetUsersForCourt(long courtId)
        {
            return GetUsersForCourt(courtId, UserGroup.Lawyer, UserGroup.Person);
        }

        public IQueryable<User> GetUsersForCourt(long courtId, params long[] roles)
        {
            var userIds = new List<long>();

            if (roles.Contains(UserGroup.Lawyer))
            {
                userIds.AddRange(from court in unitOfWork.DbContext.Set<Court>().Where(e => e.CourtId == courtId)
                                 join c in unitOfWork.DbContext.Set<Case>() on court.CourtId equals c.CourtId
                                 join s in unitOfWork.DbContext.Set<Side>() on c.CaseId equals s.CaseId
                                 join la in unitOfWork.DbContext.Set<LawyerAssignment>() on s.SideId equals la.SideId
                                 join lr in unitOfWork.DbContext.Set<LawyerRegistration>() on la.LawyerId equals lr.LawyerId
                                 where la.IsActive == true
                                 select lr.LawyerRegistrationId);
            }

            if (roles.Contains(UserGroup.Person))
            {
                userIds.AddRange(from court in unitOfWork.DbContext.Set<Court>().Where(e => e.CourtId == courtId)
                                 join c in unitOfWork.DbContext.Set<Case>() on court.CourtId equals c.CourtId
                                 join s in unitOfWork.DbContext.Set<Side>() on c.CaseId equals s.CaseId
                                 join pa in unitOfWork.DbContext.Set<PersonAssignment>() on s.SideId equals pa.SideId
                                 where pa.IsActive == true
                                 select pa.PersonRegistrationId);
            }

            userIds = userIds.Distinct().ToList();

            return unitOfWork.DbContext.Set<User>().Where(e => userIds.Contains(e.UserId));
        }
    }
}