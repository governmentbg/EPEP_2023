using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public class UserCaseFocus
    {
        public long UserCaseFocusId { get; set; }
        public long UserRegistrationId { get; set; }
        public long CaseId { get; set; }
        public int Focus { get; set; }
        public DateTime? DateWrt { get; set; }

        public virtual UserRegistration UserRegistration { get; set; }
        public virtual Case Case { get; set; }
    }

    public class UserCaseFocusConfiguration : IEntityTypeConfiguration<UserCaseFocus>
    {
        public void Configure(EntityTypeBuilder<UserCaseFocus> builder)
        {
            builder.HasKey("UserCaseFocusId");
            builder.ToTable("UserCaseFocuses");

            // Relationships
            builder.HasOne(t => t.UserRegistration)
                .WithMany(t => t.CaseFocuses)
                .HasForeignKey(d => d.UserRegistrationId)
                .IsRequired();

            builder.HasOne(t => t.Case)
                .WithMany(t => t.CaseFocuses)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
        }
    }
}
