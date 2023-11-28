using System.Data.Entity;

namespace eCase.Common.Db
{
    public interface IDbContextInitializer
    {
        void InitializeContext(DbContext context);
    }
}
