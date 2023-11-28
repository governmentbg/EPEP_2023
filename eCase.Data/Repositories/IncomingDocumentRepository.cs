using System;
using System.Data.Entity;
using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IIncomingDocumentRepository : IAggregateRepository<IncomingDocument>
    {
        IQueryable<IncomingDocument> GetIncomingDocuments(long caseId);

        IncomingDocument GetIncomingDocument(Guid incomingDocumentGid);

        Guid? GetIncomingDocumentCaseId(int incomingNumber, int incomingYear, long courtId);

        IncomingDocument GetInitIncomingDocument(long incomingDocumentId);
    }

    internal class IncomingDocumentRepository : AggregateRepository<IncomingDocument>, IIncomingDocumentRepository
    {
        public IncomingDocumentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IQueryable<IncomingDocument> GetIncomingDocuments(long caseId)
        {
            return this.Set().Where(t => t.CaseId == caseId);
        }

        public IncomingDocument GetIncomingDocument(Guid incomingDocumentGid)
        {
            return this.Set()
                .Where(i => i.Gid == incomingDocumentGid)
                .Include(t => t.Subject)
                .Include(t => t.Subject.Person)
                .Include(t => t.Subject.Entity)
                .FirstOrDefault();
        }

        public Guid? GetIncomingDocumentCaseId(int incomingNumber, int incomingYear, long courtId)
        {
            return this.Set()
                .Where(i => i.IncomingNumber == incomingNumber && i.IncomingYear == incomingYear && i.CourtId == courtId)
                .Include(i => i.Case)
                .FirstOrDefault().Case.Gid;
        }

        public IncomingDocument GetInitIncomingDocument(long incomingDocumentId)
        {
            return this.Set()
                 .Where(i => i.IncomingDocumentId == incomingDocumentId)
                 .Include(e => e.Subject)
                 .Include(e => e.IncomingDocumentType).FirstOrDefault();
        }
    }
}