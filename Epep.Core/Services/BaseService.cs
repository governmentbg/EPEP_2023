using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class BaseService : IBaseService
    {


        protected IRepository repo;
        protected ILogger logger;
        protected DateTime dtNow
        {
            get
            {
                return DateTime.Now;
            }
        }
        protected DateTime dtTomorow
        {
            get
            {
                return DateTime.Now.AddDays(1);
            }
        }

        public async Task<T> GetByIdAsync<T>(object id) where T : class
        {
            return await repo.GetByIdAsync<T>(id);
        }

        public async Task<Tprop> GetPropById<T, Tprop>(Expression<Func<T, bool>> where, Expression<Func<T, Tprop>> select) where T : class
        {
            return await repo.GetPropByIdAsync<T, Tprop>(where, select);
        }

        public async Task<T> GetByGidAsync<T>(Guid gid) where T : class, IGidRoot
        {
            return await repo.All<T>().Where(x => x.Gid == gid).FirstOrDefaultAsync();
        }

        protected string formatAuditRow(string title, string newVal, string oldVal = "", bool hasOldValue = false)
        {
            if (!hasOldValue)
            {
                return $"{title}: {newVal};";
            }

            return $"{title}: {newVal}({oldVal});";

        }
    }
}
