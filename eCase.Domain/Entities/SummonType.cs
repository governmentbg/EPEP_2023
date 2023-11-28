using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class SummonTypeMap : EntityTypeConfiguration<SummonType>
    {
        public SummonTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.SummonTypeId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("SummonTypes");
            this.Property(t => t.SummonTypeId).HasColumnName("SummonTypeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}