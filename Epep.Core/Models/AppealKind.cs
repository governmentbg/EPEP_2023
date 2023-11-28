using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class AppealKind
    {
        public AppealKind()
        {
            this.Appeals = new List<Appeal>();
        }

        public long AppealKindId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Appeal> Appeals { get; set; }
    }

    public class AppealKindConfiguration : IEntityTypeConfiguration<AppealKind>
    {
        public void Configure(EntityTypeBuilder<AppealKind> builder)
        {
            // Primary Key
            builder.HasKey(t => t.AppealKindId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("AppealKinds");
            builder.Property(t => t.AppealKindId).HasColumnName("AppealKindId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
