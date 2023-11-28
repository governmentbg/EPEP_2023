using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
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

    public class AttachedDocumentMap : EntityTypeConfiguration<AttachedDocument>
    {
        public AttachedDocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.AttachedDocumentId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("AttachedDocuments");
            this.Property(t => t.AttachedDocumentId).HasColumnName("AttachedDocumentId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.AttachmentType).HasColumnName("AttachmentType");
            this.Property(t => t.ParentId).HasColumnName("ParentId");
            this.Property(t => t.FileTitle).HasColumnName("FileTitle");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.BlobKey).HasColumnName("BlobKey");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.AttachedBlob)
                .WithMany()
                .HasForeignKey(d => d.BlobKey);
        }
    }
}
