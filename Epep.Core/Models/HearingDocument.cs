using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class HearingDocument : IAggregateRoot
    {
        public long HearingDocumentId { get; set; }
        public Guid Gid { get; set; }
        public long HearingId { get; set; }
        public long SideId { get; set; }
        public string HearingDocumentKind { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Hearing Hearing { get; set; }
        public virtual Side Side { get; set; }
    }

    public class HearingDocumentConfiguration : IEntityTypeConfiguration<HearingDocument>
    {
        public void Configure(EntityTypeBuilder<HearingDocument> builder)
        {
            // Primary Key
            builder.HasKey(t => t.HearingDocumentId);

            // Properties
            builder.Property(t => t.HearingDocumentKind)
                .IsRequired()
                .HasMaxLength(200);


            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("HearingDocuments");
            builder.Property(t => t.HearingDocumentId).HasColumnName("HearingDocumentId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.HearingId).HasColumnName("HearingId");
            builder.Property(t => t.SideId).HasColumnName("SideId");
            builder.Property(t => t.HearingDocumentKind).HasColumnName("HearingDocumentKind");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Hearing)
                .WithMany(t => t.HearingDocuments)
                .HasForeignKey(d => d.HearingId)
                .IsRequired();

            builder.HasOne(t => t.Side)
                .WithMany(t => t.HearingDocuments)
                .HasForeignKey(d => d.SideId)
                .IsRequired();
        }
    }
}
