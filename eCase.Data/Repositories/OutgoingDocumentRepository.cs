using System;
using System.Data.Entity;
using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IOutgoingDocumentRepository : IAggregateRepository<OutgoingDocument>
    {
        IQueryable<OutgoingDocument> GetOutgoingDocuments(long caseId);

        OutgoingDocument GetOutgoingDocument(Guid outgoingDocumentGid);
    }

    internal class OutgoingDocumentRepository : AggregateRepository<OutgoingDocument>, IOutgoingDocumentRepository
    {
        public OutgoingDocumentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IQueryable<OutgoingDocument> GetOutgoingDocuments(long caseId)
        {
            return this.Set().Where(t => t.CaseId == caseId);
        }

        public OutgoingDocument GetOutgoingDocument(Guid outgoingDocumentGid)
        {
            return this.Set()
                .Where(o => o.Gid == outgoingDocumentGid)
                .Include(o => o.Subject)
                .Include(o => o.Subject.Person)
                .Include(o => o.Subject.Entity)
                .FirstOrDefault();
        }
    }
}