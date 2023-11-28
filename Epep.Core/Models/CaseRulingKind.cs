using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class CaseRulingKindConfiguration : IEntityTypeConfiguration<CaseRulingKind>
    {
        public void Configure(EntityTypeBuilder<CaseRulingKind> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CaseRulingKindId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.IndexValue)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            builder.ToTable("CaseRulingKinds");
            builder.Property(t => t.CaseRulingKindId).HasColumnName("CaseRulingKindId");
            builder.Property(t => t.CourtTypeId).HasColumnName("CourtTypeId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.IndexValue).HasColumnName("IndexValue");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            builder.HasOne(t => t.CourtType)
                .WithMany(t => t.CaseRulingKinds)
                .HasForeignKey(d => d.CourtTypeId);
        }
    }
}