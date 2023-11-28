using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            // Primary Key
            builder.HasKey(t => t.AssignmentId);

            // Properties
            builder.Property(t => t.JudgeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Assignor)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Assignments");
            builder.Property(t => t.AssignmentId).HasColumnName("AssignmentId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.IncomingDocumentId).HasColumnName("IncomingDocumentId");
            builder.Property(t => t.JudgeName).HasColumnName("JudgeName");
            builder.Property(t => t.Date).HasColumnName("Date");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.Assignor).HasColumnName("Assignor");
            builder.Property(t => t.BlobKey).HasColumnName("BlobKey");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Case)
                .WithMany(t => t.Assignments)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
            builder.HasOne(t => t.IncomingDocument)
                .WithMany(t => t.Assignments)
                .HasForeignKey(d => d.IncomingDocumentId)
                .IsRequired();
        }
    }
}
