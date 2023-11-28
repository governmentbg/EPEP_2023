using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Assignment : IAggregateRoot
    {
        public long AssignmentId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public long IncomingDocumentId { get; set; }
        public string JudgeName { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Assignor { get; set; }
        public Guid? BlobKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Case Case { get; set; }
        public virtual Blob Blob { get; set; }
        public virtual IncomingDocument IncomingDocument { get; set; }
    }

    public class AssignmentMap : EntityTypeConfiguration<Assignment>
    {
        public AssignmentMap()
        {
            // Primary Key
            this.HasKey(t => t.AssignmentId);

            // Properties
            this.Property(t => t.JudgeName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Assignor)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Assignments");
            this.Property(t => t.AssignmentId).HasColumnName("AssignmentId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.IncomingDocumentId).HasColumnName("IncomingDocumentId");
            this.Property(t => t.JudgeName).HasColumnName("JudgeName");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Assignor).HasColumnName("Assignor");
            this.Property(t => t.BlobKey).HasColumnName("BlobKey");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Case)
                .WithMany(t => t.Assignments)
                .HasForeignKey(d => d.CaseId);
            this.HasRequired(t => t.IncomingDocument)
                .WithMany(t => t.Assignments)
                .HasForeignKey(d => d.IncomingDocumentId);
        }
    }
}
