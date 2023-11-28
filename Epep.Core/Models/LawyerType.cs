using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Epep.Core.Models
{
    public partial class LawyerType
    {
        public LawyerType()
        {
            this.Lawyers = new List<Lawyer>();
        }

        public long LawyerTypeId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Lawyer> Lawyers { get; set; }
    }

    public class LawyerTypeConfiguration : IEntityTypeConfiguration<LawyerType>
    {
        public void Configure(EntityTypeBuilder<LawyerType> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LawyerTypeId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("LawyerTypes");
            builder.Property(t => t.LawyerTypeId).HasColumnName("LawyerTypeId");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
