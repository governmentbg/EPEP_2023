using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class ScannedFile : IAggregateRoot
    {
        public long ScannedFileId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public Guid BlobKey { get; set; }
        public string MimeType { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Blob Blob { get; set; }
        public virtual Case Case { get; set; }
    }

    public class ScannedFileConfiguration : IEntityTypeConfiguration<ScannedFile>
    {
        public void Configure(EntityTypeBuilder<ScannedFile> builder)
        {
            // Primary Key
            builder.HasKey(t => t.ScannedFileId);

            // Properties
            builder.Property(t => t.MimeType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("ScannedFiles");
            builder.Property(t => t.ScannedFileId).HasColumnName("ScannedFileId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.BlobKey).HasColumnName("BlobKey");
            builder.Property(t => t.MimeType).HasColumnName("MimeType");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Blob)
                .WithMany(t => t.ScannedFiles)
                .HasForeignKey(d => d.BlobKey)
                .IsRequired();
            builder.HasOne(t => t.Case)
                .WithMany(t => t.ScannedFiles)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
        }
    }
}
