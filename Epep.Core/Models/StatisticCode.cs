using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class StatisticCodeConfiguration : IEntityTypeConfiguration<StatisticCode>
    {
        public void Configure(EntityTypeBuilder<StatisticCode> builder)
        {
            // Primary Key
            builder.HasKey(t => t.StatisticCodeId);

            // Properties
            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("StatisticCodes");
            builder.Property(t => t.StatisticCodeId).HasColumnName("StatisticCodeId");
            builder.Property(t => t.CaseCodeId).HasColumnName("CaseCodeId");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            builder.HasOne(t => t.CaseCode)
                .WithMany(t => t.StatisticCodes)
                .HasForeignKey(d => d.CaseCodeId)
                .IsRequired();
        }
    }
}
