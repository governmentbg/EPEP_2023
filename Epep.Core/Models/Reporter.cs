using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class ReporterConfiguration : IEntityTypeConfiguration<Reporter>
    {
        public void Configure(EntityTypeBuilder<Reporter> builder)
        {
            // Primary Key
            builder.HasKey(t => t.ReporterId);

            // Properties
            builder.Property(t => t.JudgeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.ReasonReplaced)
                .HasMaxLength(200);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Reporters");
            builder.Property(t => t.ReporterId).HasColumnName("ReporterId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.JudgeName).HasColumnName("JudgeName");
            builder.Property(t => t.DateAssigned).HasColumnName("DateAssigned");
            builder.Property(t => t.DateReplaced).HasColumnName("DateReplaced");
            builder.Property(t => t.ReasonReplaced).HasColumnName("ReasonReplaced");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Case)
                .WithMany(t => t.Reporters)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
        }
    }
}
