using Epep.Core.Models;
using System.Linq.Expressions;

namespace Epep.Core.Contracts
{
    public interface IBaseService
    {
        Task<T> GetByIdAsync<T>(object id) where T : class;
        Task<T> GetByGidAsync<T>(Guid gid) where T : class, IGidRoot;

        Task<Tprop> GetPropById<T, Tprop>(Expression<Func<T, bool>> where, Expression<Func<T, Tprop>> select) where T : class;
    }
}
