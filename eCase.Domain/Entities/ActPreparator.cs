using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class ActPreparator : IAggregateRoot
    {
        public long ActPreparatorId { get; set; }
        public Guid Gid { get; set; }
        public long ActId { get; set; }
        public string JudgeName { get; set; }
        public string Role { get; set; }
        public string SubstituteFor { get; set; }
        public string SubstituteReason { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Act Act { get; set; }
    }

    public class ActPreparatorMap : EntityTypeConfiguration<ActPreparator>
    {
        public ActPreparatorMap()
        {
            // Primary Key
            this.HasKey(t => t.ActPreparatorId);

            // Properties
            this.Property(t => t.JudgeName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Role)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.SubstituteFor)
                .HasMaxLength(200);

            this.Property(t => t.SubstituteReason)
                .HasMaxLength(200);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("ActPreparators");
            this.Property(t => t.ActPreparatorId).HasColumnName("ActPreparatorId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.ActId).HasColumnName("ActId");
            this.Property(t => t.JudgeName).HasColumnName("JudgeName");
            this.Property(t => t.Role).HasColumnName("Role");
            this.Property(t => t.SubstituteFor).HasColumnName("SubstituteFor");
            this.Property(t => t.SubstituteReason).HasColumnName("SubstituteReason");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Act)
                .WithMany(t => t.ActPreparators)
                .HasForeignKey(d => d.ActId);
        }
    }
}
