using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public class UserOrganizationCase : IGidRoot
    {
        public long UserOrganizationCaseId { get; set; }
        public Guid Gid { get; set; }
        public long OrganizationUserId { get; set; }
        public long UserRegistrationId { get; set; }
        public long CaseId { get; set; }
        public bool IsActive { get; set; }
        public bool NotificateUser { get; set; }

        public long UserWrtId { get; set; }
        public DateTime DateWrt { get; set; }

        public virtual UserRegistration OrganizationUser { get; set; }
        public virtual UserRegistration UserRegistration { get; set; }
        public virtual UserRegistration UserWrt { get; set; }
        public virtual Case Case { get; set; }
    }

    public class UserOrganizationCaseConfiguration : IEntityTypeConfiguration<UserOrganizationCase>
    {
        public void Configure(EntityTypeBuilder<UserOrganizationCase> builder)
        {
            builder.HasKey("UserOrganizationCaseId");
            builder.ToTable("UserOrganizationCases");

            // Relationships

            builder.HasOne(t => t.OrganizationUser)
               .WithMany(t => t.OrganizationUserForCases)
               .HasForeignKey(d => d.OrganizationUserId)
               .IsRequired();

            builder.HasOne(t => t.UserRegistration)
                .WithMany(t => t.OrganizationCases)
                .HasForeignKey(d => d.UserRegistrationId)
                .IsRequired();

            builder.HasOne(t => t.UserWrt)
                .WithMany(t => t.UserWrtOrganizationCases)
                .HasForeignKey(d => d.UserWrtId)
                .IsRequired();


            builder.HasOne(t => t.Case)
                .WithMany(t => t.OrganizationCases)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
        }
    }
}
