using System.Data.Entity;

namespace eCase.Domain.Entities.Upgrade
{
    internal static class DbContextExtensions
    {

        /// <summary>
        /// Регистрира надградените таблици в действащия контекст на LoggingDbContext
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureUpgradeEntityes(this DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserAssignmentMap());

            modelBuilder.Entity<ElectronicDocument>();
            modelBuilder.Entity<ElectronicDocumentSide>();
            modelBuilder.Entity<ElectronicDocumentType>();
            modelBuilder.Entity<UserAssignmentRole>();
            modelBuilder.Entity<UserRegistration>();
            modelBuilder.Entity<UserOrganizationCase>();
            modelBuilder.Entity<UserVacation>();
            modelBuilder.Entity<UserVacationType>();
            modelBuilder.Entity<MoneyCurrency>();
            modelBuilder.Entity<MoneyPricelist>();
        }
    }
}
