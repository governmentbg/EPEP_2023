using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class AttachedDocument : IAggregateRoot
    {
        public AttachedDocument()
        {
        }

        public long AttachedDocumentId { get; set; }
        public Guid Gid { get; set; }
        public int AttachmentType { get; set; }

        public long ParentId { get; set; }
        public string FileTitle { get; set; }
        public string FileName { get; set; }

        public Guid BlobKey { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        
        public virtual Blob AttachedBlob { get; set; }
    }

    public class AttachedDocumentConfiguration : IEntityTypeConfiguration<AttachedDocument>
    {
        public void Configure(EntityTypeBuilder<AttachedDocument> builder)
        {
            // Primary Key
            builder.HasKey(t => t.AttachedDocumentId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("AttachedDocuments");
            builder.Property(t => t.AttachedDocumentId).HasColumnName("AttachedDocumentId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.AttachmentType).HasColumnName("AttachmentType");
            builder.Property(t => t.ParentId).HasColumnName("ParentId");
            builder.Property(t => t.FileTitle).HasColumnName("FileTitle");
            builder.Property(t => t.FileName).HasColumnName("FileName");
            builder.Property(t => t.BlobKey).HasColumnName("BlobKey");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.AttachedBlob)
                .WithMany()
                .HasForeignKey(d => d.BlobKey)
                .IsRequired();
        }
    }
}
