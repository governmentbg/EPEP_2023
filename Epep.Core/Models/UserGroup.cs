using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;

namespace Epep.Core.Models
{
    public partial class UserGroup
    {
        [Description("Супер администратор")]
        public const long SuperAdmin = 1;
        [Description("Администратор на съд")]
        public const long CourtAdmin = 2;
        [Description("Адвокат")]
        public const long Lawyer = 3;
        [Description("Системен администратор")]
        public const long SystemAdmin = 4;
        [Description("Физическо лице")]
        public const long Person = 5;
        [Description("Адвокат и Физическо лице")]
        public const long LawyerAndPerson = 6;

        public UserGroup()
        {
            this.Users = new List<User>();
            this.Roles = new List<Role>();
        }

        public long UserGroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }

    public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            // Primary Key
            builder.HasKey(t => t.UserGroupId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            // Table & Column Mappings
            builder.ToTable("UserGroups");
            builder.Property(t => t.UserGroupId).HasColumnName("UserGroupId");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
