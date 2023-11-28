using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class CaseKind
    {
        public CaseKind()
        {
            this.Cases = new List<Case>();
        }

        public long CaseKindId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
    }

    public class CaseKindMap : EntityTypeConfiguration<CaseKind>
    {
        public CaseKindMap()
        {
            // Primary Key
            this.HasKey(t => t.CaseKindId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("CaseKinds");
            this.Property(t => t.CaseKindId).HasColumnName("CaseKindId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
