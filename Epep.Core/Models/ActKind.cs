using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace Epep.Core.Models
{
    public partial class ActKind
    {
        public ActKind()
        {
            this.Acts = new List<Act>();
        }

        public long ActKindId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Act> Acts { get; set; }
    }

    public class ActKindConfiguration : IEntityTypeConfiguration<ActKind>
    {
        public void Configure(EntityTypeBuilder<ActKind> builder)
        {

            // Primary Key
            builder.HasKey(t => t.ActKindId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("ActKinds");
            builder.Property(t => t.ActKindId).HasColumnName("ActKindId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
