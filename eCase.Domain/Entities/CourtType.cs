using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class CourtTypeMap : EntityTypeConfiguration<CourtType>
    {
        public CourtTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.CourtTypeId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("CourtTypes");
            this.Property(t => t.CourtTypeId).HasColumnName("CourtTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
