using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class CaseType
    {
        public CaseType()
        {
            this.Cases = new List<Case>();
        }

        public long CaseTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
    }

    public class CaseTypeMap : EntityTypeConfiguration<CaseType>
    {
        public CaseTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.CaseTypeId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("CaseTypes");
            this.Property(t => t.CaseTypeId).HasColumnName("CaseTypeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
