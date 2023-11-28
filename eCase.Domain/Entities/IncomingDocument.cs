using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;
using eCase.Domain.Entities.Upgrade;

namespace eCase.Domain.Entities
{
    public partial class IncomingDocument : IAggregateRoot
    {
        public IncomingDocument()
        {
            this.Assignments = new List<Assignment>();
            this.Cases = new List<Case>();
        }

        public long IncomingDocumentId { get; set; }
        public Guid Gid { get; set; }
        public long? CaseId { get; set; }
        public long? SubjectId { get; set; }
        public long CourtId { get; set; }
        public int IncomingNumber { get; set; }
        public int IncomingYear { get; set; }
        public DateTime IncomingDate { get; set; }
        public long IncomingDocumentTypeId { get; set; }
        public Guid? BlobKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }

        public long? ElectronicDocumentId { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual Blob Blob { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual Case Case { get; set; }
        public virtual Court Court { get; set; }
        public virtual IncomingDocumentType IncomingDocumentType { get; set; }
        public virtual Subject Subject { get; set; }

        public virtual ElectronicDocument ElectronicDocument { get; set; }
     
    }

    public class IncomingDocumentMap : EntityTypeConfiguration<IncomingDocument>
    {
        public IncomingDocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.IncomingDocumentId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("IncomingDocuments");
            this.Property(t => t.IncomingDocumentId).HasColumnName("IncomingDocumentId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.SubjectId).HasColumnName("SubjectId");
            this.Property(t => t.CourtId).HasColumnName("CourtId");
            this.Property(t => t.IncomingNumber).HasColumnName("IncomingNumber");
            this.Property(t => t.IncomingYear).HasColumnName("IncomingYear");
            this.Property(t => t.IncomingDate).HasColumnName("IncomingDate");
            this.Property(t => t.IncomingDocumentTypeId).HasColumnName("IncomingDocumentTypeId");
            this.Property(t => t.BlobKey).HasColumnName("BlobKey");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasOptional(t => t.Blob)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.BlobKey);
            this.HasOptional(t => t.Case)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.CaseId);
            this.HasRequired(t => t.Court)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.CourtId);
            this.HasRequired(t => t.IncomingDocumentType)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.IncomingDocumentTypeId);
            this.HasOptional(t => t.Subject)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.SubjectId);
            this.HasOptional(t => t.ElectronicDocument)
             .WithMany(t => t.IncomingDocuments)
             .HasForeignKey(d => d.ElectronicDocumentId);
        }
    }
}
