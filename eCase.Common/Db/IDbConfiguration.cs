using System.Data.Entity;

namespace eCase.Common.Db
{
    public interface IDbConfiguration
    {
        void AddConfiguration(DbModelBuilder modelBuilder);
    }
}
