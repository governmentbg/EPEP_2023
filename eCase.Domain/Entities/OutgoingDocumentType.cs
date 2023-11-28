using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class OutgoingDocumentTypeMap : EntityTypeConfiguration<OutgoingDocumentType>
    {
        public OutgoingDocumentTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.OutgoingDocumentTypeId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("OutgoingDocumentTypes");
            this.Property(t => t.OutgoingDocumentTypeId).HasColumnName("OutgoingDocumentTypeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
