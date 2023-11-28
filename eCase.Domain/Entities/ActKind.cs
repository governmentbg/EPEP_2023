using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class ActKindMap : EntityTypeConfiguration<ActKind>
    {
        public ActKindMap()
        {
            // Primary Key
            this.HasKey(t => t.ActKindId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("ActKinds");
            this.Property(t => t.ActKindId).HasColumnName("ActKindId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
