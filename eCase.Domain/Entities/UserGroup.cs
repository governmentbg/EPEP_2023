using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class UserGroupMap : EntityTypeConfiguration<UserGroup>
    {
        public UserGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.UserGroupId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("UserGroups");
            this.Property(t => t.UserGroupId).HasColumnName("UserGroupId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
