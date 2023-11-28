using System.Data.Entity;
using eCase.Common.Db;

namespace eCase.Domain.Emails
{
    public class EmailsModelDbConfiguration : IDbConfiguration
    {
        public void AddConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new EmailMap());
        }
    }
}
