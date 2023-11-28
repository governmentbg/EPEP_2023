using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class OutgoingDocument : IAggregateRoot
    {
        public long OutgoingDocumentId { get; set; }
        public Guid Gid { get; set; }
        public long? CaseId { get; set; }
        public long? SubjectId { get; set; }
        public int OutgoingNumber { get; set; }
        public DateTime? OutgoingDate { get; set; }
        public long OutgoingDocumentTypeId { get; set; }
        public Guid? BlobKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Blob Blob { get; set; }
        public virtual Case Case { get; set; }
        public virtual OutgoingDocumentType OutgoingDocumentType { get; set; }
        public virtual Subject Subject { get; set; }
    }

    public class OutgoingDocumentConfiguration : IEntityTypeConfiguration<OutgoingDocument>
    {
        public void Configure(EntityTypeBuilder<OutgoingDocument> builder)
        {
            // Primary Key
            builder.HasKey(t => t.OutgoingDocumentId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("OutgoingDocuments");
            builder.Property(t => t.OutgoingDocumentId).HasColumnName("OutgoingDocumentId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.SubjectId).HasColumnName("SubjectId");
            builder.Property(t => t.OutgoingNumber).HasColumnName("OutgoingNumber");
            builder.Property(t => t.OutgoingDate).HasColumnName("OutgoingDate");
            builder.Property(t => t.OutgoingDocumentTypeId).HasColumnName("OutgoingDocumentTypeId");
            builder.Property(t => t.BlobKey).HasColumnName("BlobKey");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Blob)
                .WithMany(t => t.OutgoingDocuments)
                .HasForeignKey(d => d.BlobKey);
            builder.HasOne(t => t.Case)
                .WithMany(t => t.OutgoingDocuments)
                .HasForeignKey(d => d.CaseId);
            builder.HasOne(t => t.OutgoingDocumentType)
                .WithMany(t => t.OutgoingDocuments)
                .HasForeignKey(d => d.OutgoingDocumentTypeId)
                .IsRequired();
            builder.HasOne(t => t.Subject)
                .WithMany(t => t.OutgoingDocuments)
                .HasForeignKey(d => d.SubjectId);
        }
    }
}
