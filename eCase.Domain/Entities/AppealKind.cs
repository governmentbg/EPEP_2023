using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class AppealKindMap : EntityTypeConfiguration<AppealKind>
    {
        public AppealKindMap()
        {
            // Primary Key
            this.HasKey(t => t.AppealKindId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("AppealKinds");
            this.Property(t => t.AppealKindId).HasColumnName("AppealKindId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
