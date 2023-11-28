using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class LawyerAssignment : IAggregateRoot//, IEventEmitter
    {
        public LawyerAssignment()
        {
            //((IEventEmitter)this).Events = new List<IDomainEvent>();
        }

        public long LawyerAssignmentId { get; set; }
        public Guid Gid { get; set; }
        public DateTime Date { get; set; }
        public long? SideId { get; set; }
        public long LawyerId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Lawyer Lawyer { get; set; }
        public virtual Side Side { get; set; }

        //ICollection<IDomainEvent> IEventEmitter.Events { get; set; }

        //public void GetCaseAccess(string email, string caseAbbr, string courtName)
        //{
        //    ((IEventEmitter)this).Events.Add(new CaseAccessEvent()
        //    {
        //        Email = email,
        //        CaseAbbr = caseAbbr,
        //        CourtName = courtName
        //    });
        //}
        //public void DenyCaseAccess(string email, string caseAbbr, string courtName)
        //{
        //    ((IEventEmitter)this).Events.Add(new ChangeCaseAccessEvent()
        //    {
        //        Email = email,
        //        CaseAbbr = caseAbbr,
        //        CourtName = courtName
        //    });
        //}
    }

    public class LawyerAssignmentConfiguration : IEntityTypeConfiguration<LawyerAssignment>
    {
        public void Configure(EntityTypeBuilder<LawyerAssignment> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LawyerAssignmentId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("LawyerAssignments");
            builder.Property(t => t.LawyerAssignmentId).HasColumnName("LawyerAssignmentId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.Date).HasColumnName("Date");
            builder.Property(t => t.SideId).HasColumnName("SideId");
            builder.Property(t => t.LawyerId).HasColumnName("LawyerId");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Lawyer)
                .WithMany(t => t.LawyerAssignments)
                .HasForeignKey(d => d.LawyerId)
                .IsRequired();
            builder.HasOne(t => t.Side)
                .WithMany(t => t.LawyerAssignments)
                .HasForeignKey(d => d.SideId);
        }
    }
}
