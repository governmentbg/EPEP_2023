using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class OutgoingDocumentType
    {
        public OutgoingDocumentType()
        {
            this.OutgoingDocuments = new List<OutgoingDocument>();
        }

        public long OutgoingDocumentTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<OutgoingDocument> OutgoingDocuments { get; set; }
    }

    public class OutgoingDocumentTypeConfiguration : IEntityTypeConfiguration<OutgoingDocumentType>
    {
        public void Configure(EntityTypeBuilder<OutgoingDocumentType> builder)
        {
            // Primary Key
            builder.HasKey(t => t.OutgoingDocumentTypeId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("OutgoingDocumentTypes");
            builder.Property(t => t.OutgoingDocumentTypeId).HasColumnName("OutgoingDocumentTypeId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
