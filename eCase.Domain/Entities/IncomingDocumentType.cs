using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class IncomingDocumentTypeMap : EntityTypeConfiguration<IncomingDocumentType>
    {
        public IncomingDocumentTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.IncomingDocumentTypeId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("IncomingDocumentTypes");
            this.Property(t => t.IncomingDocumentTypeId).HasColumnName("IncomingDocumentTypeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
