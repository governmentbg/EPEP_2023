using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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
        public string Label { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
    }

    public class CaseKindConfiguration : IEntityTypeConfiguration<CaseKind>
    {
        public void Configure(EntityTypeBuilder<CaseKind> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CaseKindId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("CaseKinds");
            builder.Property(t => t.CaseKindId).HasColumnName("CaseKindId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Label).HasColumnName("Label");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
