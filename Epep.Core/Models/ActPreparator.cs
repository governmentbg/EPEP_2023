using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class ActPreparatorConfiguration : IEntityTypeConfiguration<ActPreparator>
    {

        public void Configure(EntityTypeBuilder<ActPreparator> builder)
        {
            // Primary Key
            builder.HasKey(t => t.ActPreparatorId);

            // Properties
            builder.Property(t => t.JudgeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Role)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.SubstituteFor)
                .HasMaxLength(200);

            builder.Property(t => t.SubstituteReason)
                .HasMaxLength(200);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("ActPreparators");
            builder.Property(t => t.ActPreparatorId).HasColumnName("ActPreparatorId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.ActId).HasColumnName("ActId");
            builder.Property(t => t.JudgeName).HasColumnName("JudgeName");
            builder.Property(t => t.Role).HasColumnName("Role");
            builder.Property(t => t.SubstituteFor).HasColumnName("SubstituteFor");
            builder.Property(t => t.SubstituteReason).HasColumnName("SubstituteReason");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Act)
                .WithMany(t => t.ActPreparators)
                .HasForeignKey(d => d.ActId)
                .IsRequired();
        }
    }
}
