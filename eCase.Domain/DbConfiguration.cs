using System.Data.Entity;

using eCase.Common.Db;
using eCase.Domain.Entities;
using eCase.Domain.BlobStorage;
using eCase.Domain.Entities.Upgrade;

namespace eCase.Domain
{
    public class DbConfiguration : IDbConfiguration
    {
        public void AddConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ActKindMap());
            modelBuilder.Configurations.Add(new ActPreparatorMap());
            modelBuilder.Configurations.Add(new ActMap());
            modelBuilder.Configurations.Add(new AppealKindMap());
            modelBuilder.Configurations.Add(new AppealMap());
            modelBuilder.Configurations.Add(new AssignmentMap());
            modelBuilder.Configurations.Add(new AttachedDocumentMap());
            modelBuilder.Configurations.Add(new BlobContentLocationMap());
            modelBuilder.Configurations.Add(new BlobMap());
            modelBuilder.Configurations.Add(new CaseCodeMap());
            modelBuilder.Configurations.Add(new CaseKindMap());
            modelBuilder.Configurations.Add(new CaseRulingKindMap());
            modelBuilder.Configurations.Add(new CaseRulingMap());
            modelBuilder.Configurations.Add(new CaseMap());
            modelBuilder.Configurations.Add(new CaseTypeMap());
            modelBuilder.Configurations.Add(new ConnectedCaseMap());
            modelBuilder.Configurations.Add(new ConnectedCaseTypeMap());
            modelBuilder.Configurations.Add(new CourtMap());
            modelBuilder.Configurations.Add(new CourtTypeMap());
            modelBuilder.Configurations.Add(new EntityMap());
            modelBuilder.Configurations.Add(new HearingMap());
            modelBuilder.Configurations.Add(new HearingParticipantMap());
            modelBuilder.Configurations.Add(new HearingDocumentMap());
            modelBuilder.Configurations.Add(new IncomingDocumentMap());
            modelBuilder.Configurations.Add(new IncomingDocumentTypeMap());
            modelBuilder.Configurations.Add(new LawyerAssignmentMap());
            modelBuilder.Configurations.Add(new LawyerRegistrationMap());
            modelBuilder.Configurations.Add(new LawyerMap());
            modelBuilder.Configurations.Add(new LawyerTypeMap());
            modelBuilder.Configurations.Add(new LogMap());
            modelBuilder.Configurations.Add(new OutgoingDocumentMap());
            modelBuilder.Configurations.Add(new OutgoingDocumentTypeMap());
            modelBuilder.Configurations.Add(new PersonAssignmentMap());
            modelBuilder.Configurations.Add(new PersonRegistrationMap());
            modelBuilder.Configurations.Add(new PersonMap());
            modelBuilder.Configurations.Add(new ReporterMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new ScannedFileMap());
            modelBuilder.Configurations.Add(new SideInvolvementKindMap());
            modelBuilder.Configurations.Add(new SideMap());
            modelBuilder.Configurations.Add(new StatisticCodeMap());
            modelBuilder.Configurations.Add(new SubjectMap());
            modelBuilder.Configurations.Add(new SummonMap());
            modelBuilder.Configurations.Add(new SummonTypeMap());
            modelBuilder.Configurations.Add(new UserGroupMap());
            modelBuilder.Configurations.Add(new UserMap());

            modelBuilder.Configurations.Add(new BlobContentMap());

            //Update entities
            modelBuilder.ConfigureUpgradeEntityes();
        }
    }
}
