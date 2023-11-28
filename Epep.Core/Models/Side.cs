using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class Side : IAggregateRoot
    {
        public Side()
        {
            this.Appeals = new List<Appeal>();
            this.LawyerAssignments = new List<LawyerAssignment>();
            this.PersonAssignments = new List<PersonAssignment>();
            this.HearingDocuments = new List<HearingDocument>();
            this.UserAssignments = new List<UserAssignment>();
            this.Summons = new List<Summon>();
        }

        public long SideId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public long SideInvolvementKindId { get; set; }
        public long? SubjectId { get; set; }
        public DateTime InsertDate { get; set; }
        public string ProceduralRelation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual ICollection<Appeal> Appeals { get; set; }
        public virtual Case Case { get; set; }
        public virtual ICollection<LawyerAssignment> LawyerAssignments { get; set; }
        public virtual ICollection<PersonAssignment> PersonAssignments { get; set; }

        public virtual ICollection<HearingDocument> HearingDocuments { get; set; }
        public virtual SideInvolvementKind SideInvolvementKind { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }

        public virtual ICollection<UserAssignment> UserAssignments { get; set; }

        public string Name
        {
            get
            {
                if (this.Subject != null)
                {
                    return this.Subject.Name;
                }

                return string.Empty;
            }
        }
    }

    public class SideConfiguration : IEntityTypeConfiguration<Side>
    {
        public void Configure(EntityTypeBuilder<Side> builder)
        {
            // Primary Key
            builder.HasKey(t => t.SideId);

            // Properties
            builder.Property(t => t.ProceduralRelation)
                .HasMaxLength(200);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Sides");
            builder.Property(t => t.SideId).HasColumnName("SideId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.SideInvolvementKindId).HasColumnName("SideInvolvementKindId");
            builder.Property(t => t.SubjectId).HasColumnName("SubjectId");
            builder.Property(t => t.InsertDate).HasColumnName("InsertDate");
            builder.Property(t => t.ProceduralRelation).HasColumnName("ProceduralRelation");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Case)
                .WithMany(t => t.Sides)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
            builder.HasOne(t => t.SideInvolvementKind)
                .WithMany(t => t.Sides)
                .HasForeignKey(d => d.SideInvolvementKindId)
                .IsRequired();
            builder.HasOne(t => t.Subject)
                .WithMany(t => t.Sides)
                .HasForeignKey(d => d.SubjectId);
        }
    }
}
