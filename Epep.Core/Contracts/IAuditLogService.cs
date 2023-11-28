using Epep.Core.ViewModels.Common;

namespace Epep.Core.Contracts
{
    public interface IAuditLogService
    {
        Task<bool> SaveAuditLog(int operationId, string objectInfo, string clientIp, string requestUrl, string actionInfo = null);
        IQueryable<AuditLogVM> Select(AuditLogFilterVM filter);
    }
}
