

using Microsoft.EntityFrameworkCore;

namespace Epep.Core.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Legacy EntityConfiguration
            builder.ApplyConfiguration(new ActConfiguration());
            builder.ApplyConfiguration(new ActKindConfiguration());
            builder.ApplyConfiguration(new ActPreparatorConfiguration());
            builder.ApplyConfiguration(new AppealConfiguration());
            builder.ApplyConfiguration(new AppealKindConfiguration());
            builder.ApplyConfiguration(new AssignmentConfiguration());
            builder.ApplyConfiguration(new AttachedDocumentConfiguration());
            builder.ApplyConfiguration(new BlobConfiguration());
            builder.ApplyConfiguration(new BlobContentLocationConfiguration());
            builder.ApplyConfiguration(new CaseConfiguration());
            builder.ApplyConfiguration(new CaseCodeConfiguration());
            builder.ApplyConfiguration(new CaseKindConfiguration());
            builder.ApplyConfiguration(new CaseRulingConfiguration());
            builder.ApplyConfiguration(new CaseRulingKindConfiguration());
            builder.ApplyConfiguration(new CaseTypeConfiguration());
            builder.ApplyConfiguration(new ConnectedCaseConfiguration());
            builder.ApplyConfiguration(new ConnectedCaseTypeConfiguration());
            builder.ApplyConfiguration(new CourtConfiguration());
            builder.ApplyConfiguration(new CourtTypeConfiguration());
            builder.ApplyConfiguration(new EmailConfiguration());
            builder.ApplyConfiguration(new EntityConfiguration());
            builder.ApplyConfiguration(new HearingConfiguration());
            builder.ApplyConfiguration(new HearingDocumentConfiguration());
            builder.ApplyConfiguration(new HearingParticipantConfiguration());
            builder.ApplyConfiguration(new IncomingDocumentConfiguration());
            builder.ApplyConfiguration(new IncomingDocumentTypeConfiguration());
            builder.ApplyConfiguration(new LawyerConfiguration());
            builder.ApplyConfiguration(new LawyerAssignmentConfiguration());
            builder.ApplyConfiguration(new LawyerRegistrationConfiguration());
            builder.ApplyConfiguration(new LawyerStateConfiguration());
            builder.ApplyConfiguration(new LawyerTypeConfiguration());
            builder.ApplyConfiguration(new LogConfiguration());
            builder.ApplyConfiguration(new OutgoingDocumentConfiguration());
            builder.ApplyConfiguration(new OutgoingDocumentTypeConfiguration());
            builder.ApplyConfiguration(new PersonConfiguration());
            builder.ApplyConfiguration(new PersonAssignmentConfiguration());
            builder.ApplyConfiguration(new PersonRegistrationConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new ReporterConfiguration());
            builder.ApplyConfiguration(new ScannedFileConfiguration());
            builder.ApplyConfiguration(new SideConfiguration());
            builder.ApplyConfiguration(new SideInvolvementKindConfiguration());
            builder.ApplyConfiguration(new StatisticCodeConfiguration());
            builder.ApplyConfiguration(new SubjectConfiguration());
            builder.ApplyConfiguration(new SummonConfiguration());
            builder.ApplyConfiguration(new SummonTypeConfiguration());
            builder.ApplyConfiguration(new UserAssignmentConfiguration());
            builder.ApplyConfiguration(new UserCaseFocusConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserGroupConfiguration());
            builder.ApplyConfiguration(new UserOrganizationCaseConfiguration());
            builder.ApplyConfiguration(new UserRegistrationConfiguration());
            #endregion


            builder.ApplyConfiguration(new MoneyObligationConfiguration());

        }
        #region Legacy entities
        //public DbSet<Email> Emails { get; set; }

      
        #endregion

        #region Update entities
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuditLogOperation> AuditLogOperations { get; set; }
        public DbSet<CodeMapping> CodeMappings { get; set; }
        public DbSet<ElectronicDocument> ElectronicDocuments { get; set; }
        public DbSet<ElectronicDocumentType> ElectronicDocumentTypes { get; set; }
        public DbSet<ElectronicDocumentSide> ElectronicDocumentSides { get; set; }
        public DbSet<ElectronicPayment> ElectronicPayments { get; set; }
        public DbSet<MoneyObligation> MoneyObligations { get; set; }
        public DbSet<MoneyObligationType> MoneyObligationTypes { get; set; }
        public DbSet<MoneyCurrency> MoneyCurrencies { get; set; }
        public DbSet<MoneyPricelist> MoneyPricelists { get; set; }
        public DbSet<MoneyPricelistDocument> MoneyPricelistDocuments { get; set; }
        public DbSet<MoneyPricelistValue> MoneyPricelistValues { get; set; }
        public DbSet<UserCaseFocus> UserCaseFocuses { get; set; }
        public DbSet<UserRegistrationType> UserRegistrationTypes { get; set; }
        public DbSet<UserRegistration> UserRegistrations { get; set; }
        public DbSet<UserAssignment> UserAssignments { get; set; }
        public DbSet<UserVacation> UserVacations { get; set; }
        public DbSet<UserVacationType> UserVacationTypes { get; set; }
        public DbSet<LawyerBar> LawyerBars { get; set; }
        #endregion

    }
}
