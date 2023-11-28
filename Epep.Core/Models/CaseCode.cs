using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Epep.Core.Models
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

    public class CaseCodeConfiguration : IEntityTypeConfiguration<CaseCode>
    {
        public void Configure(EntityTypeBuilder<CaseCode> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CaseCodeId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("CaseCodes");
            builder.Property(t => t.CaseCodeId).HasColumnName("CaseCodeId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
