using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUserContext userContext;
        private readonly IRepository repo;
        public AuditLogService(
            IRepository _repo,
            IUserContext _userContext)
        {
            repo = _repo;
            userContext = _userContext;
        }
        public async Task<bool> SaveAuditLog(int operationId, string objectInfo, string clientIp, string requestUrl, string actionInfo = null)
        {
            try
            {
                var entity = new AuditLog()
                {
                    DateWrt = DateTime.Now,
                    UserId = userContext.UserId,
                    OperationId = operationId,
                    ObjectInfo = objectInfo,
                    ActionInfo = actionInfo,
                    ClientIP = clientIp,
                    RequestUrl = requestUrl
                };
                await repo.AddAsync(entity);
                await repo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IQueryable<AuditLogVM> Select(AuditLogFilterVM filter)
        {
            if (filter == null)
            {
                return null;
            }
            filter.UpdateNullables();
            Expression<Func<AuditLog, bool>> whereUser = x => true;
            if (!string.IsNullOrEmpty(filter.UserName))
            {
                whereUser = x => EF.Functions.Like(x.User.FullName, filter.UserName.ToPaternSearch());
            }
            Expression<Func<AuditLog, bool>> whereObject = x => true;
            if (!string.IsNullOrEmpty(filter.Object))
            {
                whereObject = x => EF.Functions.Like(x.ObjectInfo, filter.Object.ToPaternSearch());
            }

            return repo.AllReadonly<AuditLog>()
                            .Include(x => x.User)
                            .Include(x => x.Operation)
                            .Where(x => x.DateWrt >= filter.DateFrom)
                            .Where(x => x.DateWrt <= filter.DateTo.MakeEndDate())
                            .Where(whereUser)
                            .Where(whereObject)
                            .OrderByDescending(x => x.Id)
                            .Select(x => new AuditLogVM
                            {
                                UserFullName = x.User.FullName,
                                DateWrt = x.DateWrt,
                                OperationName = x.Operation.Name,
                                ObjectInfo = x.ObjectInfo,
                                ActionInfo = x.ActionInfo
                            }).AsQueryable();
        }

    }
}

