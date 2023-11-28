using Epep.Core.ViewModels.Case;

namespace Epep.Core.Contracts
{
    public interface ISummonService : IBaseService
    {
        Task<SummonDocumentVM> GetSummonDocumentInfo(Guid gid);
        IQueryable<SummonVM> SelectSummonsByUser(FilterSummonVM filter);
        Task<bool> SetSummonAsRead(Guid gid, DateTime readTime, byte[] reportBytes, byte[] timestampBytes);
    }
}
