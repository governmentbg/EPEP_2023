using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class ConnectedCaseType
    {
        public ConnectedCaseType()
        {
            this.ConnectedCases = new List<ConnectedCase>();
        }

        public long ConnectedCaseTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<ConnectedCase> ConnectedCases { get; set; }
    }

    public class ConnectedCaseTypeMap : EntityTypeConfiguration<ConnectedCaseType>
    {
        public ConnectedCaseTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ConnectedCaseTypeId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("ConnectedCaseTypes");
            this.Property(t => t.ConnectedCaseTypeId).HasColumnName("ConnectedCaseTypeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
