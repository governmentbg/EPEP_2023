using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class StatisticCode
    {
        public StatisticCode()
        {
            this.Cases = new List<Case>();
        }

        public long StatisticCodeId { get; set; }
        public long CaseCodeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual CaseCode CaseCode { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
    }

    public class StatisticCodeMap : EntityTypeConfiguration<StatisticCode>
    {
        public StatisticCodeMap()
        {
            // Primary Key
            this.HasKey(t => t.StatisticCodeId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("StatisticCodes");
            this.Property(t => t.StatisticCodeId).HasColumnName("StatisticCodeId");
            this.Property(t => t.CaseCodeId).HasColumnName("CaseCodeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            this.HasRequired(t => t.CaseCode)
                .WithMany(t => t.StatisticCodes)
                .HasForeignKey(d => d.CaseCodeId);
        }
    }
}
