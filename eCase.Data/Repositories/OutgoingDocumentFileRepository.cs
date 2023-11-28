using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace eCase.Data.Repositories
{
    public interface IOutgoingDocumentFileRepository : IAggregateRepository<OutgoingDocumentFile>
    {
        OutgoingDocumentFile GetOutgoingDocumentFile(long outgoingDocumentId);
    }

    internal class OutgoingDocumentFileRepository : AggregateRepository<OutgoingDocumentFile>, IOutgoingDocumentFileRepository
    {
        public OutgoingDocumentFileRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override Expression<Func<OutgoingDocumentFile, object>>[] Includes
        {
            get
            {
                return new Expression<Func<OutgoingDocumentFile, object>>[]
                    {
                        e => e.OutgoingDocument
                    };
            }
        }

        public OutgoingDocumentFile GetOutgoingDocumentFile(long outgoingDocumentId)
        {
            return this.Set()
                    .Where(t => t.OutgoingDocumentId == outgoingDocumentId)
                    .SingleOrDefault();
        }
    }
}