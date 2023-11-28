using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class LawyerTypeMap : EntityTypeConfiguration<LawyerType>
    {
        public LawyerTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.LawyerTypeId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("LawyerTypes");
            this.Property(t => t.LawyerTypeId).HasColumnName("LawyerTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
