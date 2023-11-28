using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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
        public virtual ICollection<Side> Sides { get; set; }
    }

    public class SideInvolvementKindMap : EntityTypeConfiguration<SideInvolvementKind>
    {
        public SideInvolvementKindMap()
        {
            // Primary Key
            this.HasKey(t => t.SideInvolvementKindId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("SideInvolvementKinds");
            this.Property(t => t.SideInvolvementKindId).HasColumnName("SideInvolvementKindId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
