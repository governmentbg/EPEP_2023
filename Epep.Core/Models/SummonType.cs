using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class SummonType
    {
        public SummonType()
        {
            this.Summons = new List<Summon>();
        }

        public long SummonTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }
    }

    public class SummonTypeConfiguration : IEntityTypeConfiguration<SummonType>
    {
        public void Configure(EntityTypeBuilder<SummonType> builder)
        {
            // Primary Key
            builder.HasKey(t => t.SummonTypeId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("SummonTypes");
            builder.Property(t => t.SummonTypeId).HasColumnName("SummonTypeId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}