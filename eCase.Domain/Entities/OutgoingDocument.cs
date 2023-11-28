using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
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

    public class OutgoingDocumentMap : EntityTypeConfiguration<OutgoingDocument>
    {
        public OutgoingDocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.OutgoingDocumentId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("OutgoingDocuments");
            this.Property(t => t.OutgoingDocumentId).HasColumnName("OutgoingDocumentId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.SubjectId).HasColumnName("SubjectId");
            this.Property(t => t.OutgoingNumber).HasColumnName("OutgoingNumber");
            this.Property(t => t.OutgoingDate).HasColumnName("OutgoingDate");
            this.Property(t => t.OutgoingDocumentTypeId).HasColumnName("OutgoingDocumentTypeId");
            this.Property(t => t.BlobKey).HasColumnName("BlobKey");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasOptional(t => t.Blob)
                .WithMany(t => t.OutgoingDocuments)
                .HasForeignKey(d => d.BlobKey);
            this.HasOptional(t => t.Case)
                .WithMany(t => t.OutgoingDocuments)
                .HasForeignKey(d => d.CaseId);
            this.HasRequired(t => t.OutgoingDocumentType)
                .WithMany(t => t.OutgoingDocuments)
                .HasForeignKey(d => d.OutgoingDocumentTypeId);
            this.HasOptional(t => t.Subject)
                .WithMany(t => t.OutgoingDocuments)
                .HasForeignKey(d => d.SubjectId);
        }
    }
}
