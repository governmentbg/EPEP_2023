using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class CaseRulingKind
    {
        public CaseRulingKind()
        {
            this.CaseRulings = new List<CaseRuling>();
        }

        public long CaseRulingKindId { get; set; }
        public long? CourtTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string IndexValue { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual CourtType CourtType { get; set; }
        public virtual ICollection<CaseRuling> CaseRulings { get; set; }
    }

    public class CaseRulingKindMap : EntityTypeConfiguration<CaseRulingKind>
    {
        public CaseRulingKindMap()
        {
            // Primary Key
            this.HasKey(t => t.CaseRulingKindId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.IndexValue)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("CaseRulingKinds");
            this.Property(t => t.CaseRulingKindId).HasColumnName("CaseRulingKindId");
            this.Property(t => t.CourtTypeId).HasColumnName("CourtTypeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IndexValue).HasColumnName("IndexValue");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            this.HasOptional(t => t.CourtType)
                .WithMany(t => t.CaseRulingKinds)
                .HasForeignKey(d => d.CourtTypeId);
        }
    }
}