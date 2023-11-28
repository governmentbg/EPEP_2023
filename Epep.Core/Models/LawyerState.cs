using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Epep.Core.Models
{
    public partial class LawyerState
    {

        public long LawyerStateId { get; set; }
       
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Lawyer> Lawyers { get; set; }
    }

    public class LawyerStateConfiguration : IEntityTypeConfiguration<LawyerState>
    {
        public void Configure(EntityTypeBuilder<LawyerState> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LawyerStateId);
           

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("LawyerStates");
            builder.Property(t => t.LawyerStateId).HasColumnName("LawyerStateId");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
