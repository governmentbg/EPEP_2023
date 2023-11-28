using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class CaseCode
    {
        public CaseCode()
        {
            this.Cases = new List<Case>();
            this.StatisticCodes = new List<StatisticCode>();
        }

        public long CaseCodeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ViewOrder { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<StatisticCode> StatisticCodes { get; set; }
    }

    public class CaseCodeMap : EntityTypeConfiguration<CaseCode>
    {
        public CaseCodeMap()
        {
            // Primary Key
            this.HasKey(t => t.CaseCodeId);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("CaseCodes");
            this.Property(t => t.CaseCodeId).HasColumnName("CaseCodeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
