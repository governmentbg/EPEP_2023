using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    public partial class UserAssignment : IAggregateRoot
    {
        public UserAssignment()
        {
        }

        public long UserAssignmentId { get; set; }
        public Guid Gid { get; set; }

        public Guid? LegacyGid { get; set; }
        public long UserRegistrationId { get; set; }
        public long CaseId { get; set; }
        public long SideId { get; set; }
        public DateTime Date { get; set; }
        public int? AssignmentRoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual UserRegistration UserRegistration { get; set; }
        public virtual Case Case { get; set; }
        public virtual Side Side { get; set; }

        [ForeignKey(nameof(AssignmentRoleId))]
        public virtual UserAssignmentRole AssignmentRole { get; set; }
    }

    public class UserAssignmentConfiguration : IEntityTypeConfiguration<UserAssignment>
    {
        public void Configure(EntityTypeBuilder<UserAssignment> builder)
        {
            // Primary Key
            builder.HasKey(t => t.UserAssignmentId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("UserAssignments");
            builder.Property(t => t.UserAssignmentId).HasColumnName("UserAssignmentId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.LegacyGid).HasColumnName("LegacyGid");
            builder.Property(t => t.UserRegistrationId).HasColumnName("UserRegistrationId");
            builder.Property(t => t.SideId).HasColumnName("SideId");
            builder.Property(t => t.Date).HasColumnName("Date");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.UserRegistration)
                .WithMany(t => t.Assignments)
                .HasForeignKey(d => d.UserRegistrationId)
                .IsRequired();

            builder.HasOne(t => t.Side)
                .WithMany(t => t.UserAssignments)
                .HasForeignKey(d => d.SideId)
                .IsRequired();

            builder.HasOne(t => t.Case)
               .WithMany(t => t.UserAssignments)
               .HasForeignKey(d => d.CaseId)
               .IsRequired();
        }
    }
}
