using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class CourtType
    {
        public CourtType()
        {
            this.CaseRulingKinds = new List<CaseRulingKind>();
            this.Courts = new List<Court>();
        }

        public long CourtTypeId { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<CaseRulingKind> CaseRulingKinds { get; set; }
        public virtual ICollection<Court> Courts { get; set; }
    }

    public class CourtTypeConfiguration : IEntityTypeConfiguration<CourtType>
    {
        public void Configure(EntityTypeBuilder<CourtType> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CourtTypeId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("CourtTypes");
            builder.Property(t => t.CourtTypeId).HasColumnName("CourtTypeId");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
