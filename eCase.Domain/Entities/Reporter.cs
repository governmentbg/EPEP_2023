using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Reporter : IAggregateRoot
    {
        public long ReporterId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public string JudgeName { get; set; }
        public DateTime DateAssigned { get; set; }
        public DateTime? DateReplaced { get; set; }
        public string ReasonReplaced { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Case Case { get; set; }
    }

    public class ReporterMap : EntityTypeConfiguration<Reporter>
    {
        public ReporterMap()
        {
            // Primary Key
            this.HasKey(t => t.ReporterId);

            // Properties
            this.Property(t => t.JudgeName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.ReasonReplaced)
                .HasMaxLength(200);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Reporters");
            this.Property(t => t.ReporterId).HasColumnName("ReporterId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.JudgeName).HasColumnName("JudgeName");
            this.Property(t => t.DateAssigned).HasColumnName("DateAssigned");
            this.Property(t => t.DateReplaced).HasColumnName("DateReplaced");
            this.Property(t => t.ReasonReplaced).HasColumnName("ReasonReplaced");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Case)
                .WithMany(t => t.Reporters)
                .HasForeignKey(d => d.CaseId);
        }
    }
}
