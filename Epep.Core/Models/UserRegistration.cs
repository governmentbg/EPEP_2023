using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    public class UserRegistration : IdentityUser<long>, IGidRoot
    {
        [PersonalData]
        [Display(Name = "Идентификатор")]
        public string EGN { get; set; }
        public bool IsComfirmedEGN { get; set; }

        public long? OrganizationUserId { get; set; }

        [PersonalData]
        [Display(Name = "Идентификатор")]
        public string UIC { get; set; }
        public bool IsComfirmedUIC { get; set; }

        public Guid Gid { get; set; }

        [PersonalData]
        [Display(Name = "Имена")]
        public string FullName { get; set; }

        public int UserTypeId { get; set; }
        public long? CourtId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public DateTime? ComfirmedDate { get; set; }
        public long? ComfirmedUserId { get; set; }
        public DateTime? DeniedDate { get; set; }
        public string DeniedDescription { get; set; }
        public string NotificationEmail { get; set; }
        public string RegCertificateInfo { get; set; }

        public Guid? DeviceGid { get; set; }

        public virtual ICollection<UserAssignment> Assignments { get; set; }
        public virtual ICollection<UserCaseFocus> CaseFocuses { get; set; }

        [ForeignKey(nameof(OrganizationUserId))]
        public virtual UserRegistration OrganizationRegistration { get; set; }
        [ForeignKey(nameof(ComfirmedUserId))]
        public virtual UserRegistration ComfirmedUser { get; set; }
        public virtual ICollection<UserOrganizationCase> OrganizationUserForCases { get; set; }
        public virtual ICollection<UserOrganizationCase> OrganizationCases { get; set; }
        public virtual ICollection<UserOrganizationCase> UserWrtOrganizationCases { get; set; }

        [ForeignKey(nameof(UserTypeId))]
        public virtual UserRegistrationType UserRegistrationType { get; set; }

        [ForeignKey(nameof(CourtId))]
        public virtual Court Court { get; set; }

        [NotMapped]
        public int LoginUserType { get; set; }
        [NotMapped]
        public string CertNo { get; set; }
    }

    public class UserRegistrationConfiguration : IEntityTypeConfiguration<UserRegistration>
    {
        public void Configure(EntityTypeBuilder<UserRegistration> builder)
        {
            builder.ToTable("UserRegistrations");

            // Properties
            builder.Property(t => t.PasswordHash)
                .HasMaxLength(200);

            builder.Property(t => t.PhoneNumber)
                .HasMaxLength(200);

            builder.Property(t => t.EGN)
               .HasMaxLength(20);

            builder.Property(t => t.UIC)
               .HasMaxLength(20);

            builder.Property(t => t.FullName)
              .HasMaxLength(500);

            builder.Property(t => t.DeniedDescription)
              .HasMaxLength(1000);

            builder.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
