using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class IncomingDocumentConfiguration : IEntityTypeConfiguration<IncomingDocument>
    {
        public void Configure(EntityTypeBuilder<IncomingDocument> builder)
        {
            // Primary Key
            builder.HasKey(t => t.IncomingDocumentId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("IncomingDocuments");
            builder.Property(t => t.IncomingDocumentId).HasColumnName("IncomingDocumentId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.SubjectId).HasColumnName("SubjectId");
            builder.Property(t => t.CourtId).HasColumnName("CourtId");
            builder.Property(t => t.IncomingNumber).HasColumnName("IncomingNumber");
            builder.Property(t => t.IncomingYear).HasColumnName("IncomingYear");
            builder.Property(t => t.IncomingDate).HasColumnName("IncomingDate");
            builder.Property(t => t.IncomingDocumentTypeId).HasColumnName("IncomingDocumentTypeId");
            builder.Property(t => t.BlobKey).HasColumnName("BlobKey");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Blob)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.BlobKey);
            builder.HasOne(t => t.Case)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.CaseId);
            builder.HasOne(t => t.Court)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.CourtId)
                .IsRequired();
            builder.HasOne(t => t.IncomingDocumentType)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.IncomingDocumentTypeId)
                .IsRequired();
            builder.HasOne(t => t.Subject)
                .WithMany(t => t.IncomingDocuments)
                .HasForeignKey(d => d.SubjectId);
            builder.HasOne(t => t.ElectronicDocument)
               .WithMany(t => t.IncomingDocuments)
               .HasForeignKey(d => d.ElectronicDocumentId);
        }
    }
}
