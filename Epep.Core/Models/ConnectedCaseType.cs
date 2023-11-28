using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class ConnectedCaseTypeConfiguration : IEntityTypeConfiguration<ConnectedCaseType>
    {
        public void Configure(EntityTypeBuilder<ConnectedCaseType> builder)
        {
            // Primary Key
            builder.HasKey(t => t.ConnectedCaseTypeId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("ConnectedCaseTypes");
            builder.Property(t => t.ConnectedCaseTypeId).HasColumnName("ConnectedCaseTypeId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
