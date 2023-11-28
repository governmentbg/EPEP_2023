using System;
using System.Linq.Expressions;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IAssignmentRepository : IAggregateRepository<Assignment>
    {
        bool CheckPermission(Guid assignmentGid, long userId);
    }

    internal class AssignmentRepository : AggregateRepository<Assignment>, IAssignmentRepository
    {
        private ICaseRepository _caseRepository;

        public AssignmentRepository(IUnitOfWork unitOfWork, ICaseRepository caseRepository)
            : base(unitOfWork)
        {
            _caseRepository = caseRepository;
        }

        public bool CheckPermission(Guid assignmentGid, long userId)
        {
            var assignment = this.FindByGid(assignmentGid);

            if (assignment != null)
            {
                return _caseRepository.CheckPermission(assignment.CaseId, userId);
            }

            return false;
        }
    }
}