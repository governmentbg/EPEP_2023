using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class CaseRuling : IAggregateRoot
    {
        public long CaseRulingId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public long? HearingId { get; set; }
        public long? ActId { get; set; }
        public long CaseRulingKindId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Act Act { get; set; }
        public virtual CaseRulingKind CaseRulingKind { get; set; }
        public virtual Case Case { get; set; }
        public virtual Hearing Hearing { get; set; }
    }

    public class CaseRulingConfiguration : IEntityTypeConfiguration<CaseRuling>
    {
        public void Configure(EntityTypeBuilder<CaseRuling> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CaseRulingId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("CaseRulings");
            builder.Property(t => t.CaseRulingId).HasColumnName("CaseRulingId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.HearingId).HasColumnName("HearingId");
            builder.Property(t => t.ActId).HasColumnName("ActId");
            builder.Property(t => t.CaseRulingKindId).HasColumnName("CaseRulingKindId");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Act)
                .WithMany(t => t.CaseRulings)
                .HasForeignKey(d => d.ActId);
            builder.HasOne(t => t.CaseRulingKind)
                .WithMany(t => t.CaseRulings)
                .HasForeignKey(d => d.CaseRulingKindId)
                .IsRequired();
            builder.HasOne(t => t.Case)
                .WithMany(t => t.CaseRulings)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
            builder.HasOne(t => t.Hearing)
                .WithMany(t => t.CaseRulings)
                .HasForeignKey(d => d.HearingId);
        }
    }
}
