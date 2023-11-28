using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
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

    public class ScannedFileMap : EntityTypeConfiguration<ScannedFile>
    {
        public ScannedFileMap()
        {
            // Primary Key
            this.HasKey(t => t.ScannedFileId);

            // Properties
            this.Property(t => t.MimeType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("ScannedFiles");
            this.Property(t => t.ScannedFileId).HasColumnName("ScannedFileId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.BlobKey).HasColumnName("BlobKey");
            this.Property(t => t.MimeType).HasColumnName("MimeType");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Blob)
                .WithMany(t => t.ScannedFiles)
                .HasForeignKey(d => d.BlobKey);
            this.HasRequired(t => t.Case)
                .WithMany(t => t.ScannedFiles)
                .HasForeignKey(d => d.CaseId);
        }
    }
}
