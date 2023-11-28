using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class SideInvolvementKind
    {
        public SideInvolvementKind()
        {
            this.Sides = new List<Side>();
        }

        public long SideInvolvementKindId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public int? SideType { get; set; }
        public virtual ICollection<Side> Sides { get; set; }
    }

    public class SideInvolvementKindConfiguration : IEntityTypeConfiguration<SideInvolvementKind>
    {
        public void Configure(EntityTypeBuilder<SideInvolvementKind> builder)
        {
            // Primary Key
            builder.HasKey(t => t.SideInvolvementKindId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("SideInvolvementKinds");
            builder.Property(t => t.SideInvolvementKindId).HasColumnName("SideInvolvementKindId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
            builder.Property(t => t.SideType).HasColumnName("SideType");
        }
    }
}
