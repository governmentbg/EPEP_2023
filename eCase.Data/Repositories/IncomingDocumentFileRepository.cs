using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace eCase.Data.Repositories
{
    public interface IIncomingDocumentFileRepository : IAggregateRepository<IncomingDocumentFile>
    {
        IncomingDocumentFile GetIncommingDocumentFile(long incommingDocumentId);
    }

    internal class IncomingDocumentFileRepository : AggregateRepository<IncomingDocumentFile>, IIncomingDocumentFileRepository
    {
        public IncomingDocumentFileRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override Expression<Func<IncomingDocumentFile, object>>[] Includes
        {
            get
            {
                return new Expression<Func<IncomingDocumentFile, object>>[]
                    {
                        e => e.IncomingDocument
                    };
            }
        }

        public IncomingDocumentFile GetIncommingDocumentFile(long incommingDocumentId)
        {
            return this.Set()
                        .Where(t => t.IncomingDocumentId == incommingDocumentId)
                        .SingleOrDefault();
        }
    }
}