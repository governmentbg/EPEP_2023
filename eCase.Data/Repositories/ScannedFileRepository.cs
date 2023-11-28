using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IScannedFileRepository : IAggregateRepository<ScannedFile>
    {
        IQueryable<ScannedFile> GetScannedFiles(long caseId);
    }

    internal class ScannedFileRepository : AggregateRepository<ScannedFile>, IScannedFileRepository
    {
        public ScannedFileRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IQueryable<ScannedFile> GetScannedFiles(long caseId)
        {
            return this.Set().Where(t => t.CaseId == caseId);
        }
    }
}