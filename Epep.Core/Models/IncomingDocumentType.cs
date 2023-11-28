using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class IncomingDocumentType
    {
        public IncomingDocumentType()
        {
            this.IncomingDocuments = new List<IncomingDocument>();
        }

        public long IncomingDocumentTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<IncomingDocument> IncomingDocuments { get; set; }
    }

    public class IncomingDocumentTypeConfiguration : IEntityTypeConfiguration<IncomingDocumentType>
    {
        public void Configure(EntityTypeBuilder<IncomingDocumentType> builder)
        {
            // Primary Key
            builder.HasKey(t => t.IncomingDocumentTypeId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("IncomingDocumentTypes");
            builder.Property(t => t.IncomingDocumentTypeId).HasColumnName("IncomingDocumentTypeId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
